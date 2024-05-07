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
            param.Limit = ViewModel.Config.SearchLimit;
            param.Input = _text;
            param.Rule = ViewModel.SearchState.Rule;
            param.Alias = ViewModel.SearchState.Alias;
            param.Member = ViewModel.SearchState.Member;
            param.Handler += OnSegmentSearched;
            ViewModel.HistorySearchResult.Param = param;
            ViewModel.TaskMgr.Run(new Search(param, ViewModel.TabName), _removeSameClass: true);
        }

        private void OnSegmentSearched(ref List<ISegmentElement> _result, SearchParam _param)
        {
            List<ISegmentElement> refCopy = _result;
            List<P4VChangelist> changelists = refCopy.OfType<P4VChangelist>().ToList(); // TODO: 캐스팅 제거
            ViewModel.HistorySearchResult.Add(changelists);
        }
    }
}
