// jdyun 24/04/10(수)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base;
using P4VHelper.Base.Extension;

namespace P4VHelper.Model
{
    public class BackgroundTaskThread : Bindable
    {
        private Thread _thread;
        private int _isRunning;
        private int _isTaskRunning;

        public int Id { get; }
        public bool IsRunning
        {
            get => InterlockedEx.Bool.Get(ref _isRunning);
            set => InterlockedEx.Bool.Set(ref _isRunning, value);
        }

        public bool IsTaskRunning
        {
            get => InterlockedEx.Bool.Get(ref _isTaskRunning);
            private set => InterlockedEx.Bool.Set(ref _isTaskRunning, value);
        }
        public BackgroundTaskMgr Mgr { get; }

        public BackgroundTaskThread(int id, BackgroundTaskMgr mgr)
        {
            _thread = new Thread(ThreadRoutine);

            Id = id;
            Mgr = mgr;
        }

        public void Start()
        {
            _isRunning = 1;
            _thread.Start();
        }

        public void Join()
        {
            _thread.Join();
        }

        private void ThreadRoutine()
        {
            for (;;)
            {
                BackgroundTask? task;

                if (IsRunning)
                    task = Mgr.Pop(this);
                else
                    break;
                        
                if (task == null)
                    continue;

                IsTaskRunning = true;
                Mgr.OnTaskBegin(task);
                Mgr.OnTaskEnd(task);
                IsTaskRunning = false;
            }
        }
    }
}
