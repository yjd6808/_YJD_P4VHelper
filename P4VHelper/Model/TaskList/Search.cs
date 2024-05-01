// jdyun 24/05/01(수) 근로자의 날

using Microsoft.VisualBasic;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Param;

namespace P4VHelper.Model.TaskList
{
    public class Search : BackgroundTask
    {
        private readonly SearchParam param_;
        private readonly string runTab_;

        public override string ClassId => string.Intern(base.ClassId + runTab_);

        public Search(SearchParam _param, string _runTab)
        {
            param_ = _param;
            Notifier = new ProgressNotifer(this);
            Notifier.AddEach();
            Notifier.AddEach();

            param_.Notifier = Notifier;
            runTab_ = _runTab;
        }

        public override string Description => "검색 중...";
        public override bool HasDetailView => false;

        public override void Execute()
        {
            Mgr.ViewModel.Engine.SegmentMgr.Search(param_);
        }

        protected override void OnEndDispatched()
        {
            Mgr.ViewModel.Logger?.WriteDebug($"{Description} 작업 완료");
        }
    }
}
