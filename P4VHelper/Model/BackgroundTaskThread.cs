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
        private readonly Thread thread_;
        private int isRunning_;
        private int isTaskRunning_;

        public int Id { get; }
        public bool IsRunning
        {
            get => InterlockedEx.Bool.Get(ref isRunning_);
            set => InterlockedEx.Bool.Set(ref isRunning_, value);
        }

        public bool IsTaskRunning
        {
            get => InterlockedEx.Bool.Get(ref isTaskRunning_);
            private set => InterlockedEx.Bool.Set(ref isTaskRunning_, value);
        }
        public BackgroundTaskMgr Mgr { get; }

        public BackgroundTaskThread(int _id, BackgroundTaskMgr _mgr)
        {
            thread_ = new Thread(ThreadRoutine);

            Id = _id;
            Mgr = _mgr;
        }

        public void Start()
        {
            isRunning_ = 1;
            thread_.Start();
        }

        public void Join()
        {
            thread_.Join();
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
