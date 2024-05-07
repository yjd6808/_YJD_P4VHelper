using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Param;
using System.Diagnostics;
using System.IO;

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
            param_.Handler += (_seg, _exception) =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Mgr.ViewModel.Logger?.WriteDebug($"{Path.GetFileName(_seg.FilePath)}세그먼트 로딩중 오류발생\n{_exception}");
                });
            };

            Mgr.ViewModel.Engine.SegmentMgr.Load(param_);
        }

        protected override void OnEndDispatched()
        {
            Mgr.ViewModel.Logger?.WriteDebug($"{Description} 작업 완료");
        }
    }
}
