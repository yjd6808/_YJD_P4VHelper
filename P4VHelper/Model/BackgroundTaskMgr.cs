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
using P4VHelper.Model.TaskList;
using P4VHelper.ViewModel;

namespace P4VHelper.Model
{
    public class BackgroundTaskMgr : Bindable
    {
        private List<BackgroundTaskThread> _threads = new ();
        private BackgroundTask _targetedTask;
        private LinkedList<BackgroundTask> _runningTasks = new ();
        private bool _viewDetail;
        private CV _condVar = new ();

        public BackgroundTask TargetedTask => _targetedTask;
        public Queue<BackgroundTask> TaskQueue { get; } = new();

        public BackgroundTask DefaultTask { get; }
        public Dispatcher Dispatcher { get; }
        public MainViewModel ViewModel { get; }
        public bool HaveRemainTask => TaskQueue.Count > 0;
        public int ThreadCount => _threads.Count;
        public int RunningThreadCount => _threads.Count(x => x.IsTaskRunning);

        public bool ViewdDetail
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
            DefaultTask = new Default(this);
            ViewModel = viewModel;
            Dispatcher = viewModel.View.Dispatcher;

            _targetedTask = DefaultTask;

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
                    foreach (var task in _runningTasks)
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
                _condVar.Wait(this, () => callerThread.IsRunning == false || TaskQueue.Count > 0);

                if (TaskQueue.TryDequeue(out BackgroundTask? task))
                {
                    _runningTasks.AddLast(task);
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
                TaskQueue.Enqueue(task);
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
                OnPropertyChanged(nameof(HaveRemainTask));
            });

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
                OnPropertyChanged(nameof(HaveRemainTask));
            });

            _runningTasks.Remove(task);
            task._OnEnd();
        }

        public BackgroundTask? GetRandomRunningTask()
        {
            lock (this)
            {
                BackgroundTask[] arr = _runningTasks.Where((t) => t.State == BackgroundTaskState.Running).ToArray();
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
