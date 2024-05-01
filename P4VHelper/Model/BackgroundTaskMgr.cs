// jdyun 24/04/10(수)
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Model.TaskList;
using P4VHelper.ViewModel;

namespace P4VHelper.Model
{
    public class BackgroundTaskMgr : Bindable
    {
        public static BackgroundTaskMgr Instance { get; set; }

        private readonly List<BackgroundTaskThread> threads_;
        private BackgroundTask targetedTask_;
        private bool viewDetail_;
        private readonly Cv condVar_;

        public BackgroundTask DefaultTask { get; }
        public Dispatcher Dispatcher { get; }
        public MainViewModel ViewModel { get; }

        /// <summary>
        /// 상태표시줄에 표시할 작업
        /// </summary>
        public BackgroundTask TargetedTask => targetedTask_;

        /// <summary>
        /// 대기중인 작업 목록
        /// </summary>
        public LinkedList<BackgroundTask> WaitingTaskList { get; }

        /// <summary>
        /// 실행중인 작업 목록
        /// </summary>
        public LinkedList<BackgroundTask> RunningTaskList { get; }

        /// <summary>
        /// 대기중인 작업 수
        /// </summary>
        public bool WaitingTaskCount => LockEx.Do(this, () => WaitingTaskList.Count > 0);

        /// <summary>
        /// 실행중이거나 대기중인 작업 수
        /// </summary>
        public int TotalTaskCount
        {
            get
            {
                int totalCount = 0;
                lock (this)
                {
                    totalCount += RunningTaskList.Count;
                    totalCount += WaitingTaskList.Count;
                }
                return totalCount;
            }
        }

        /// <summary>
        /// 실행중인 쓰레드 수
        /// </summary>
        public int ThreadCount
        {
            get => threads_.Count;
        }

        /// <summary>
        /// 작업을 진행중인 쓰레드 수
        /// </summary>
        public int RunningThreadCount
        {
            get => threads_.Count(_x => _x.IsTaskRunning);
        }


        /// <summary>
        /// 작업표시줄 자세히보기 활성화 여부
        /// </summary>
        public bool ViewDetail
        {
            get => viewDetail_;
            set
            {
                viewDetail_ = value;
                OnPropertyChanged();
            }
        }

        private BackgroundTaskMgr(int _threadCount, MainViewModel _viewModel)
        {
            ViewModel = _viewModel;
            Dispatcher = _viewModel.View.Dispatcher;
            DefaultTask = new Default();
            WaitingTaskList = new LinkedList<BackgroundTask>();
            RunningTaskList = new LinkedList<BackgroundTask>();

            threads_ = new List<BackgroundTaskThread>();
            targetedTask_ = DefaultTask;
            viewDetail_ = false;
            condVar_ = new Cv();

            for (int i = 0; i < _threadCount; ++i)
            {
                BackgroundTaskThread thread = new BackgroundTaskThread(i, this);
                thread.Start();
                threads_.Add(thread);
            }
        }

        public static BackgroundTaskMgr GetInstance(int _threadCount, MainViewModel _viewModel)
        {
            if (Instance == null)
            {
                Instance = new BackgroundTaskMgr(_threadCount, _viewModel);
            }
            
            return Instance;
        }

        /// <summary>
        /// 백그라운드 쓰레드를 모두 종료한다.
        /// 어떤 쓰레드에서도 호출 가능
        /// </summary>
        public void Stop(bool _interrupt = true)
        {
            lock (this)
            {
                if (_interrupt)
                {
                    foreach (var task in RunningTaskList)
                    {
                        task._OnInterruptRequested();
                    }
                }

                WaitingTaskList.Clear();

                foreach (var thread in threads_)
                {
                    thread.IsRunning = false;
                }

                condVar_.NotifyAll(this);
            }

            foreach (var t in threads_)
            {
                t.Join();
            }
        }

        public int InterruptIf(Predicate<BackgroundTask> _predicate, bool _containRunningTask = true)
        {
            int count = 0;
            lock (this)
            {
                foreach (var task in RunningTaskList)
                {
                    if (_predicate(task))
                    {
                        task._OnInterruptRequested();
                        count++;
                    }
                }

                count += WaitingTaskList.RemoveAllIf(_predicate);
                return count;
            }
        }

        public void Abort()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 실행할 작업을 가져온다.
        /// 백그라운드 쓰레드에서만 불린다.
        /// </summary>
        /// <param name="callerThread">호출 쓰레드</param>
        /// <param name="task">대기중인 작업</param>
        /// <returns>true 반환시 작업이 있음</returns>
        public BackgroundTask? Pop(BackgroundTaskThread _callerThread)
        {
            lock (this)
            {
                condVar_.Wait(this, () => _callerThread.IsRunning == false || WaitingTaskList.Count > 0);

                if (WaitingTaskList.Count > 0)
                {
                    BackgroundTask task =  WaitingTaskList.First.Value;
                    WaitingTaskList.RemoveFirst();
                    return task;
                }
            }
            return null;
        }

        /// <summary>
        /// 테스크를 백그라운드 쓰레드에서 실행한다.
        /// 어떤 쓰레드에서도 호출 가능
        /// </summary>
        /// <param name="task">실행할 작업</param>
        public void Run(BackgroundTask _task, bool _removeSameClass = false)
        {
            _task._OnWaiting();
            lock (this)
            {
                int removedSameClassTaskCount = 0;
                if (_removeSameClass)
                {
                    removedSameClassTaskCount = InterruptIf(task => task.ClassId == _task.ClassId);

                    if (removedSameClassTaskCount > 0)
                    {
                        int a = 40;
                    }

                }

                WaitingTaskList.AddLast(_task);
                condVar_.NotifyOne(this);
            }
        }

        public void OnTaskBegin(BackgroundTask _task)
        {
            if (targetedTask_.State == BackgroundTaskState.Finished)
            {
                UpdateTarget(_task);
            }

            Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(RunningThreadCount));
                OnPropertyChanged(nameof(TotalTaskCount));
            });

            lock (this)
            {
                RunningTaskList.AddLast(_task);
            }

            _task._OnBegin();
        }

        private void UpdateTarget(BackgroundTask _task)
        {
            targetedTask_ = _task;
            Dispatcher.BeginInvoke(() => OnPropertyChanged(nameof(TargetedTask)));
            targetedTask_._OnTargeted();
        }

        public void OnTaskEnd(BackgroundTask _task)
        {
            lock (this)
            {
                if (targetedTask_ == _task)
                {
                    BackgroundTask nextTask = GetRandomRunningTask() ?? DefaultTask;
                    UpdateTarget(nextTask);
                }

                Dispatcher.BeginInvoke(() =>
                {
                    OnPropertyChanged(nameof(RunningThreadCount));
                    OnPropertyChanged(nameof(TotalTaskCount));
                });

                RunningTaskList.Remove(_task);
            }
            
            _task._OnEnd();
        }

        public BackgroundTask? GetRandomRunningTask()
        {
            BackgroundTask[] arr = RunningTaskList.Where((_t) => _t.State == BackgroundTaskState.Running).ToArray();
            if (arr.Length == 0)
                return null;
            int idx = Random.Shared.Next(0, arr.Length);
            return arr[idx];
        }

        public void SyncTaskToUi(ref ObservableCollection<BackgroundTask> _tasks)
        {
        }
    }
}
