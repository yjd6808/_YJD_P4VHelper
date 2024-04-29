// jdyun 24/04/06(토)

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Gma.DataStructures.StringSearch;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using P4VHelper.ViewModel;
using P4VHelper.Customize.Converter;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Search;
using P4VHelper.Model.TaskList;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using P4VHelper.API;
using P4VHelper.Base;
using P4VHelper.Engine.Collection;

namespace P4VHelper.View
{
    public partial class MainView : Window
    {
        public MainViewModel ViewModel { get; }

        public MainView()
        {
            ViewModel = new MainViewModel(this);

            InitializeComponent();
        }

        private async void OnLoaded(object _sender, RoutedEventArgs _e)
        {
            await ViewModel.Loaded();

            //ChangelistAliasComboBox.ItemsSource = ViewModel.Config.P4VConfig._segMap[SegmentType.Changelist].Alias

            ViewModel.TaskMgr.Run(new Load("depot", int.MaxValue, true, LoadArgs.Changelist.Create(_forceServer: true), SaveArgs.Changelist.Create(_forceServer: true)));


        }

        private void OnClosing(object? _sender, CancelEventArgs _e)
        {
            if (ViewModel.TaskMgr.RunningThreadCount > 0)
            {
                if (MessageBox.Show("아직 실행중인 작업이 있습니다.\n정말로 종료하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ViewModel.TaskMgr.Stop();
                    return;
                }
                _e.Cancel = true;
            }

            ViewModel.TaskMgr.Stop();
        }

        private void HistorySearchTextBox_OnPreviewTextInput(object _sender, TextCompositionEventArgs _e)
        {
            if (HistoryMemberComboBox.SelectedItem == null)
                return;

            var member = (P4VChangelist.Member)HistoryMemberComboBox.SelectedItem;
            if (member == P4VChangelist.Member.Revision)
            {
                Regex regex = new Regex("[^0-9]+");
                _e.Handled = regex.IsMatch(_e.Text);
            }
        }

        private void HistorySearchTextBox_OnTextChanged(object _sender, TextChangedEventArgs _e)
        {
            if (HistorySearchTextBox.Text.Length == 0)
            {
                ViewModel.TaskMgr.Run(new Load("depot", int.MaxValue, true, LoadArgs.Changelist.Create(_forceServer: true), SaveArgs.Changelist.Create(_forceServer: true)));
            }
            else
            {

            }
        }

        private void HistoryMemberComboBox_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {

        }

        private void HistoryAliasComboBox_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            P4VConfig.SegmentGroup prevConfig = null;
            P4VConfig.SegmentGroup curConfig = HistoryAliasComboBox.SelectedItem as P4VConfig.SegmentGroup;
            if (curConfig == null)
                return;

            if (_e.RemovedItems.Count > 0)
            {
                prevConfig = _e.RemovedItems[0] as P4VConfig.SegmentGroup;
                Debug.Assert(prevConfig != null);
            }

            if (prevConfig == null)
                return;


        }
    }
}