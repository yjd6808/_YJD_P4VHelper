using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Notifier;

namespace P4VHelper.Model.TaskList
{
    public class Default : BackgroundTask
    {
        public Default()
        {
            state_ = BackgroundTaskState.Finished;
            Notifier = new ProgressNotifer(this);
            Notifier.AddEach();
        }

        public override string Description => "작업 없음";
        public override bool HasDetailView => false;
        public override void Execute()
        {
            throw new Exception("실행 불가능한 테스크 입니다.");
        }
    }
}
