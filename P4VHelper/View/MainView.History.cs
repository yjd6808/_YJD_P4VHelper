﻿// jdyun 24/05/01(수) - 근로자의 날
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

namespace P4VHelper.View
{
    public partial class MainView : Window
    {
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

            ViewModel.HistorySearchResult.Clear();

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

        
    }
}