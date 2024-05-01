// jdyun 24/05/01(수) - 근로자의 날
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Customize.Control;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Param;
using P4VHelper.Model.TaskList;
using P4VHelper.ViewModel;

namespace P4VHelper.Command.MainView.List
{
    public class SearchHistory : MainCommand<string>
    {
        public SearchHistory(MainViewModel _viewModel) : base(_viewModel, "히스토리탭에서 수행하는 검색")
        {
        }

        public override void Execute(string _text)
        {
            ViewModel.HistorySearchResult.Clear();
            if (!ViewModel.SearchState.IsValid())
                throw new Exception("서치 스테이트가 유효하지 않습니다.");

            SearchParam param = new SearchParam();
            param.Limit = 500;
            param.Input = _text;
            param.Rule = ViewModel.SearchState.Rule;
            param.Alias = ViewModel.SearchState.Alias;
            param.Member = ViewModel.SearchState.Member;
            param.Handler += OnSegmentSearched;
            ViewModel.TaskMgr.Run(new Search(param, ViewModel.TabName), _removeSameClass: true);
        }

        private void OnSegmentSearched(ref List<ISegmentElement> _result, SearchParam _param)
        {
            List<ISegmentElement> refCopy = _result;

            // double check (BeginInvoke(비동기)로 안하면 한번만 체크해도댐)
            // 만약 double check를 안하고 비동기로 처리해버리면 background task가 interrupt가 되고나서
            // 아이템이 추가되는 괴이한 현상이 발생할 수 있음
            if (!_param.Notifier.IsInterruptRequested)
            {
                ViewModel.View.Dispatcher.BeginInvoke(() =>
                {
                    if (_param.Notifier.IsInterruptRequested)
                        return;

                    IEnumerable<P4VChangelist> changelists = refCopy.OfType<P4VChangelist>();
                    ViewModel.HistorySearchResult.AddRange(changelists);
                });
            }

            // 다른 리스트를 참조하도록 해놔야함.
            _result = new List<ISegmentElement>(64);
        }
    }
}
