﻿using P4VHelper.Base.Notifier;

namespace P4VHelper.Model.TaskList
{
    public class Test : BackgroundTask
    {
        public Test(BackgroundTaskMgr mgr) : base(mgr)
        {
            Notifier = new ProgressNotifer(this);
            Notifier.AddEach();
        }

        public override string Name => "숫자를 많이 센다";
        public override bool HasDetailView => false;

        public override void Execute()
        {
            int max = 1000000000 / 6;
            Notifier.Start(max);

            for (int i = 0; i < max; ++i)
            {
                Notifier.Progress(0);
                if (IsInterruptRequested)
                    break;
            }
        }

        protected override void OnEndDispatched()
        {
            Mgr.ViewModel.Logger?.WriteDebug($"{Name} 작업 완료");
        }
    }
}
