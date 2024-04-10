// jdyun 24/04/10(수)
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using P4VHelper.Base;

namespace P4VHelper.Model
{
    public class BackgroundTaskMgr : Bindable
    {
        public static BackgroundTaskMgr Instance = new (8);

        private List<BackgroundTaskThread> _threads = new ();
        private BackgroundTask _currentTask;

        public BackgroundTask CurrentTask => _currentTask;
        public ConcurrentQueue<BackgroundTask> TaskQueue { get; } = new();
        public bool HaveRemainTask => TaskQueue.Count > 0;
        public int ThreadCount => _threads.Count;
        public int RunningThreadCount => _threads.Count(x => x.IsTaskRunning);
        

        private BackgroundTaskMgr(int threadCount)
        {
            _currentTask = BackgroundTask.Default;

            for (int i = 0; i < threadCount; ++i)
            {
                BackgroundTaskThread thread = new BackgroundTaskThread(i, this);
                thread.Start();

                _threads.Add(thread);
            }
        }

        public void Stop()
        {
            foreach (var t in _threads)
                t.IsRunning = false;

            Monitor.PulseAll(this);

            foreach (var t in _threads)
                t.Join();
        }

        public BackgroundTask? Pop()
        {
            Monitor.Wait(this);

            if (TaskQueue.TryDequeue(out BackgroundTask? task))
                return task;

            return null;
        }

        public void Add(BackgroundTask task)
        {
            TaskQueue.Enqueue(task);
            Monitor.Pulse(this);
        }

        public void OnTaskBegin(BackgroundTask task)
        {
            if (_currentTask.Completed == false) 
            {
                _currentTask = task;

                OnPropertyChanged(nameof(CurrentTask));
            }
        }

        public void OnTaskEnd(BackgroundTask task)
        {
            if (_currentTask.Completed)
            {
                _currentTask = task;

                OnPropertyChanged(nameof(CurrentTask));
            }
        }
    }
}
