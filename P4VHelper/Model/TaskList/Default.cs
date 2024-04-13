using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Util;

namespace P4VHelper.Model.TaskList
{
    public class Default : BackgroundTask
    {
        public Default(BackgroundTaskMgr mgr) : base(mgr)
        {
            Notifier = new EachProgressNotifier(this, TotalSubtaskCount);
        }

        public override int TotalSubtaskCount => 0;
        public override string Name => "작업 없음";
        public override bool HasDetailView { get; }
        public override ProgressNotifer Notifier { get; }
        public override void Do()
        {

        }
    }
}
