// jdyun 24/05/01(수) - 근로자의 날
using P4VHelper.Engine.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using P4VHelper.API;
using P4VHelper.Engine.Search;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Param;
using P4VHelper.Model.TaskList;
using System.Windows.Controls.Primitives;
using P4VHelper.Extension;
using P4VHelper.Model;
using System.Data.Common;
using System.Reflection;
using System;
using P4VHelper.Customize.Control;

namespace P4VHelper.View
{
    public partial class MainView : Window
    {
        private void HistoryTabTimer(object? _sender, EventArgs _e)
        {
            DateTime now = DateTime.Now;
            string segStr = SegmentType.Changelist.ToString();

            // set from text changed event
            if (ViewModel.GetVar<bool>("HISTORY_SEARCH_DIRTY") &&
                ViewModel.GetVar<DateTime>("HISTORY_SEARCH_TIME") + TimeSpan.FromMilliseconds(50) <= now)
            {
                ViewModel.SetVar("HISTORY_SEARCH_DIRTY", false);
                string text = ViewModel.GetVar<string>("HISTORY_SEARCH_TEXT");
                ViewModel.SearchState.InputText = text;
                ViewModel.Commander.SearchHistory.Execute(text);
            }

            // set from search background task
            if (ViewModel.GetVar<bool>($"{segStr}_SEARCH_FINISHED"))
            {
                if (ViewModel.HistorySearchResult.ScrollableItemCount > 0)
                {
                    ViewModel.HistorySearchResult.ViewMoreItems(50);
                }

                ViewModel.SetVar($"{segStr}_SEARCH_FINISHED", false);
            }
        }

        private void HistorySearchTextBox_OnPreviewTextInput(object _sender, TextCompositionEventArgs _e)
        {
            var member = (P4VChangelist.Member)ViewModel.SearchState.Member;
            if (member == P4VChangelist.Member.Revision)
            {
                Regex regex = new Regex("[^0-9]+");
                _e.Handled = regex.IsMatch(_e.Text);
            }
        }

        private void HistorySearchTextBox_OnTextChanged(object _sender, TextChangedEventArgs _e)
        {
            if (!IsLoaded)
                return;

            // 변경사항이 있고 0.1초뒤에 검색 수행
            // ViewModel.SetVar("HISTORY_SEARCH_DIRTY", true);
            // ViewModel.SetVar("HISTORY_SEARCH_TIME", DateTime.Now);
            // ViewModel.SetVar("HISTORY_SEARCH_TEXT", HistorySearchTextBox.Text);

            // 즉시 검색 수행
            ViewModel.SearchState.InputText = HistorySearchTextBox.Text;
            ViewModel.Commander.SearchHistory.Execute(HistorySearchTextBox.Text);
        }

        private void HistoryMemberComboBox_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            if (HistoryMemberComboBox.SelectedItem == null)
                return;

            ViewModel.SearchState.Member = (int)HistoryMemberComboBox.SelectedItem;

            // @참고: https://stackoverflow.com/questions/3659858/in-c-sharp-wpf-why-is-my-tabcontrols-selectionchanged-event-firing-too-often
            // 버블링 이벤트 발생 방지
            _e.Handled = true;
        }

        private async void HistoryAliasComboBox_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            P4VConfig.SegmentGroup prevConfig = null;
            P4VConfig.SegmentGroup curConfig = HistoryAliasComboBox.SelectedItem as P4VConfig.SegmentGroup;
            if (curConfig == null)
                return;

            await OnDepotChanged(ViewModel.SearchState.Alias, curConfig.Alias);
            _e.Handled = true; // 버블링 이벤트 발생 방지

            if (_e.RemovedItems.Count > 0)
            {
                prevConfig = _e.RemovedItems[0] as P4VConfig.SegmentGroup;
                Debug.Assert(prevConfig != null);
            }

            if (prevConfig == null)
                return;
        }

        private void HistoryRuleComboBox_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            if (HistoryRuleComboBox.SelectedItem == null)
                return;

            Rule prevRule = Rule.Max;
            Rule curRule = (Rule)HistoryRuleComboBox.SelectedItem;
            ViewModel.SearchState.Rule = curRule;
            _e.Handled = true;  // 버블링 이벤트 발생 방지

            if (_e.RemovedItems.Count > 0)
            {
                prevRule = (Rule)_e.RemovedItems[0];
                Debug.Assert(prevRule != Rule.Max);
            }

            if (prevRule == Rule.Max)
                return;
        }

        private void HistorySearchResultDataGrid_OnScrollChanged(object _sender, ScrollChangedEventArgs _e)
        {
            ScrollViewer scrollViewer = HistorySearchResultDataGrid.GetScrollViewer();
            if (scrollViewer == null)
            {
                Debug.Assert(false);
                return;
            }

            ScrollBar scrollBar = scrollViewer.GetScrollBar();
            if (scrollBar == null)
            {
                Debug.Assert(false);
                return;
            }

            #region DEBUG PRINT
            // Debug.WriteLine($"{nameof(_e.ExtentHeight)}: {_e.ExtentHeight}");
            // Debug.WriteLine($"{nameof(_e.ExtentHeightChange)}: {_e.ExtentHeightChange}");
            // Debug.WriteLine($"{nameof(_e.ViewportHeight)}: {_e.ViewportHeight}");
            // Debug.WriteLine($"{nameof(_e.ViewportHeightChange)}: {_e.ViewportHeightChange}");
            // Debug.WriteLine($"{nameof(_e.VerticalOffset)}: {_e.VerticalOffset}");
            // Debug.WriteLine($"{nameof(_e.VerticalChange)}: {_e.VerticalChange}");
            #endregion

            if (_e.VerticalChange == 0)
                return;

            // 위로 간 경우
            if (_e.VerticalChange < 0)
                return;

            P4VChangelist lastChangelist = LastChangelist();

            if (lastChangelist == null)
            {
                Debug.Assert(false);
                return;
            }
        }

        public P4VChangelist LastChangelist()
        {
            return HistorySearchResultDataGrid.Items[^1] as P4VChangelist;
        }

        public List<P4VChangelist> SelectedChangelists()
        {
            return HistorySearchResultDataGrid.SelectedItems.Cast<P4VChangelist>().ToList();
        }

        public P4VChangelist SelectedChangelist()
        {
            return HistorySearchResultDataGrid.SelectedItem as P4VChangelist;
        }

        // 키다운은 터널링해야 인식함;
        private void HistorySearchResultDataGrid_OnPreviewKeyDown(object _sender, KeyEventArgs _e)
        {
            if (_e.Key != Key.Down && _e.Key != Key.PageDown)
                return;

            // 디텍트 스크롤바 끝자락
            // @참고: https://stackoverflow.com/questions/1301411/detect-when-wpf-listview-scrollbar-is-at-the-bottom
            if (HistorySearchResultDataGrid.IsScrollEnd && LastChangelist() == HistorySearchResultDataGrid.SelectedItem)
            {
                ViewModel.HistorySearchResult.ViewMoreItems(ViewModel.Config.ScrollLimit, false);
                HistorySearchResultDataGrid.Focus();
                _e.Handled = true;
            }
        }
    }
}
