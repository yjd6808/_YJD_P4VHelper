using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Param;

namespace P4VHelper.Model.TaskList
{
    public class Load : BackgroundTask
    {
        private readonly LoadParam param_;

        public Load(LoadParam _param)
        {
            param_ = _param;
            Notifier = new ProgressNotifer(this);
            Notifier.AddEach();

            param_.Notifier = Notifier;
        }

        
        public override string Description => "체인지리스트를 읽어서 로컬에 저장한다.";
        public override bool HasDetailView => false;

        public override void Execute()
        {
            Mgr.ViewModel.Engine.SegmentMgr.Load(param_);
        }

        protected override void OnEndDispatched()
        {
            Mgr.ViewModel.Logger?.WriteDebug($"{Description} 작업 완료");
        }
    }
}
