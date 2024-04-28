using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Collection;

namespace P4VHelper.Model.TaskList
{
    public class Load : BackgroundTask
    {
        private readonly string searcherAlias_;
        private readonly int count_;           // 로딩할 세그먼트 수
        private readonly bool save_;
        private readonly LoadArgs? loadArgs_;
        private readonly SaveArgs? saveArgs_;

        public Load(string _alias, int _count, bool _save, LoadArgs? _loadArgs = null, SaveArgs? _saveArgs = null)
        {
            Notifier = new ProgressNotifer(this);
            Notifier.AddEach();

            searcherAlias_ = _alias;
            count_ = _count;
            save_ = _save;
            loadArgs_ = _loadArgs;
            saveArgs_ = _saveArgs;
        }

        public override string Name => "체인지리스트를 읽어서 로컬에 저장한다.";
        public override bool HasDetailView => false;

        public override void Execute()
        {
            Mgr.ViewModel.Engine.Mgr.Load(SegmentType.Changelist, searcherAlias_, Notifier, count_, save_, loadArgs_, saveArgs_);
        }

        protected override void OnEndDispatched()
        {
            Mgr.ViewModel.Logger?.WriteDebug($"{Name} 작업 완료");
        }
    }
}
