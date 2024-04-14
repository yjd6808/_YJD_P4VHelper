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

namespace P4VHelper.Model
{
    public class BackgroundTaskMgr : Bindable
    {
        private List<BackgroundTaskThread> _threads = new ();
        private BackgroundTask _runningTask;
        private bool _viewDetail;
        private CV _condVar = new ();

        public BackgroundTask RunningTask => _runningTask;
        public Queue<BackgroundTask> TaskQueue { get; } = new();
        public ObservableCollection<BackgroundTask> TaskList { get; } = new();
        public Dispatcher Dispatcher { get; }
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


        public BackgroundTaskMgr(int threadCount, Dispatcher dispatcher)
        {
            _runningTask = BackgroundTask.Default;

            Dispatcher = dispatcher;

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
        public void Stop()
        {
            lock (this)
            {
                foreach (var t in _threads)
                {
                }
            }

            _condVar.NotifyAll(this);

            foreach (var t in _threads)
                t.Join();
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
                    return task;
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
            }
            _condVar.NotifyOne(this);
            Dispatcher.Invoke(() => TaskList.Add(task));
        }

        public void OnTaskBegin(BackgroundTask task)
        {
            task._OnBegin();

            if (_runningTask.State == BackgroundTaskState.Finished)
            {
                _runningTask = task;
                _runningTask._OnTargeted();
            }

            Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(RunningThreadCount));
                OnPropertyChanged(nameof(HaveRemainTask));
            });
        }

        public void OnTaskEnd(BackgroundTask task)
        {
            task._OnEnd();

            if (_runningTask == task)
            {
                _runningTask = GetRandomRunningTask() ?? BackgroundTask.Default;
                _runningTask._OnTargeted();
            }

            Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged(nameof(RunningThreadCount));
                OnPropertyChanged(nameof(HaveRemainTask));
            });
        }

        public BackgroundTask? GetRandomRunningTask()
        {
            BackgroundTask[] arr = TaskList.Where((t) => t.State == BackgroundTaskState.Running).ToArray();
            if (arr.Length == 0)
                return null;
            int idx = Random.Shared.Next(0, arr.Length);
            return arr[idx];
        }
    }
}
