using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base;

namespace P4VHelper.Model
{
    public class BackgroundTaskThread : Bindable
    {
        private Thread _thread;
        private int _id;
        private bool _isTaskRunning;

        public int Id => _id;
        public bool IsRunning { get; set; }
        public bool IsTaskRunning => _isTaskRunning;
        public BackgroundTaskMgr Mgr { get; }

        public BackgroundTaskThread(int id, BackgroundTaskMgr mgr)
        {
            _thread = new Thread(ThreadRoutine);
            _id = id;

            Mgr = mgr;
        }

        public void Start()
        {
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
                    task = Mgr.Pop();
                else
                    break;
                        
                if (task == null)
                    continue;

                Mgr.OnTaskBegin(task);
                task.Execute();
                Mgr.OnTaskEnd(task);
            }
        }
    }
}
