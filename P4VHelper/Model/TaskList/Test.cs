using P4VHelper.Base.Util;

namespace P4VHelper.Model.TaskList
{
    public class Test : BackgroundTask
    {
        public Test(BackgroundTaskMgr mgr) : base(mgr)
        {
            Notifier = new EachProgressNotifier(this, TotalSubtaskCount);
        }

        public sealed override int TotalSubtaskCount => int.MaxValue;
        public override string Name => "숫자를 많이 센다";
        public override bool HasDetailView => false;
        public override ProgressNotifer Notifier { get; }
        public override void Do()
        {
        }
    }
}
