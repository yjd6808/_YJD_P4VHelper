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
        private List<BackgroundTaskThread> _threads;
        private BackgroundTask _targetedTask;
        private bool _viewDetail;
        private CV _condVar;

        public BackgroundTask DefaultTask { get; }
        public Dispatcher Dispatcher { get; }
        public MainViewModel ViewModel { get; }

        /// <summary>
        /// 상태표시줄에 표시할 작업
        /// </summary>
        public BackgroundTask TargetedTask => _targetedTask;

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
        public bool WaitingTaskCount
        {
            get => LockEx.Do(this, () => WaitingTaskList.Count > 0);
        }

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
            get => _threads.Count;
        }

        /// <summary>
        /// 작업을 진행중인 쓰레드 수
        /// </summary>
        public int RunningThreadCount
        {
            get => _threads.Count(x => x.IsTaskRunning);
        }


        /// <summary>
        /// 작업표시줄 자세히보기 활성화 여부
        /// </summary>
        public bool ViewDetail
        {
            get => _viewDetail;
            set
            {
                _viewDetail = value;
                OnPropertyChanged();
            }
        }

        public BackgroundTaskMgr(int threadCount, MainViewModel viewModel)
        {
            ViewModel = viewModel;
            Dispatcher = viewModel.View.Dispatcher;
            DefaultTask = new Default(this);
            WaitingTaskList = new LinkedList<BackgroundTask>();
            RunningTaskList = new LinkedList<BackgroundTask>();

            _threads = new List<BackgroundTaskThread>();
            _targetedTask = DefaultTask;
            _viewDetail = false;
            _condVar = new CV();

            for (int i = 0; i < threadCount; ++i)
            {
                BackgroundTaskThread thread = new BackgroundTaskThread(i, this);
                thread.Start();
                _threads.Add(thread);
            }
        }

        /// <summary>
        /// 백그라운드 쓰레드를 모두 종료한다.
        /// 어떤 쓰레드에서도 호출 가능
        /// </summary>
        public void Stop(bool interrupt = true)
        {
            lock (this)
            {
                if (interrupt)
                    foreach (var task in RunningTaskList)
                        task._OnInterruptRequested();

                foreach (var thread in _threads)
                    thread.IsRunning = false;

                _condVar.NotifyAll(this);
            }

            foreach (var t in _threads)
                t.Join();
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
        public BackgroundTask? Pop(BackgroundTaskThread callerThread)
        {
            lock (this)
            {
                _condVar.Wait(this, () => callerThread.IsRunning == false || WaitingTaskList.Count > 0);

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
        public void Run(BackgroundTask task)
        {
            task._OnWaiting();
            lock (this)
            {
                WaitingTaskList.AddLast(task);
                _condVar.NotifyOne(this);
            }
        }

        public void OnTaskBegin(BackgroundTask task)
        {
            if (_targetedTask.State == BackgroundTaskState.Finished)
            {
                UpdateTarget(task);
            }

            Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(RunningThreadCount));
                OnPropertyChanged(nameof(TotalTaskCount));
            });

            RunningTaskList.AddLast(task);
            task._OnBegin();
        }

        private void UpdateTarget(BackgroundTask task)
        {
            _targetedTask = task;
            Dispatcher.BeginInvoke(() => OnPropertyChanged(nameof(TargetedTask)));
            _targetedTask._OnTargeted();
        }

        public void OnTaskEnd(BackgroundTask task)
        {
            if (_targetedTask == task)
            {
                BackgroundTask nextTask = GetRandomRunningTask() ?? DefaultTask;
                UpdateTarget(nextTask);
            }

            Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(RunningThreadCount));
                OnPropertyChanged(nameof(TotalTaskCount));
            });

            RunningTaskList.Remove(task);
            task._OnEnd();
        }

        public BackgroundTask? GetRandomRunningTask()
        {
            lock (this)
            {
                BackgroundTask[] arr = RunningTaskList.Where((t) => t.State == BackgroundTaskState.Running).ToArray();
                if (arr.Length == 0)
                    return null;
                int idx = Random.Shared.Next(0, arr.Length);
                return arr[idx];
            }
        }

        public void SyncTaskToUI(ref ObservableCollection<BackgroundTask> tasks)
        {
        }
    }
}
