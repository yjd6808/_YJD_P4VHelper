// jdyun 24/04/06(토)

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Printing;
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
using System.Windows.Threading;
using Microsoft.Win32.SafeHandles;
using P4VHelper.API;
using P4VHelper.Base;
using P4VHelper.Engine.Collection;
using P4VHelper.Engine.Param;
using P4VHelper.Extension;
using P4VHelper.Model;

namespace P4VHelper.View
{
    public partial class MainView : Window
    {
        public MainViewModel ViewModel { get; }
        public DispatcherTimer Timer { get; }

        public MainView()
        {
            ViewModel = new MainViewModel(this);

            InitializeComponent();

            Timer = new DispatcherTimer();
            Timer.Tick += HistoryTabTimer;
            Timer.Interval = TimeSpan.FromMilliseconds(33);
            Timer.Start();
        }

        private async void OnLoaded(object _sender, RoutedEventArgs _e)
        {
            await ViewModel.Loaded();
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

        private void MainTabControl_OnSelectionChanged(object _sender, SelectionChangedEventArgs _e)
        {
            TabItem tab = MainTabControl.SelectedItem as TabItem;
            if (tab is null)
                return;

            StackPanel tabHeader = tab.Header as StackPanel;
            if (tabHeader is null)
            {
                // 탭 헤더의 컨텐츠는 스택패널로 하기로 정의했으므로, 규칙을 따를 것
                Debug.Assert(false);
                return;
            }

            TextBlock tabHeaderTextBlock = tabHeader.FindChild<TextBlock>();
            if (tabHeaderTextBlock is null)
            {
                // 무조건 존재해야함. 내가 그렇게 하기로 했다.
                Debug.Assert(false);
                return;
            }

            ViewModel.TabName = tabHeaderTextBlock.Text;
        }

        private async Task OnDepotChanged(string _prevAlis, string _curAlias)
        {
            if (!ViewModel.Engine.IsConnected)
            {
                await ViewModel.Engine.ConnectAsync();
            }

            ViewModel.SearchState.Alias = _curAlias;

            // 얼라이어스 변경시 캐시비움
            if (!string.IsNullOrEmpty(_prevAlis))
            {
                foreach (var segGroup in ViewModel.Config.P4VConfig.GetAliasGroup(_prevAlis))
                {
                    SegmentGroup group = ViewModel.Engine.SegmentMgr.GetGroupById(segGroup.Id);
                    group.Clear();
                }
            }

            foreach (var segGroup in ViewModel.Config.P4VConfig.GetAliasGroup(_curAlias))
            {
                SegmentGroup group = ViewModel.Engine.SegmentMgr.GetGroupById(segGroup.Id);
                bool isInDisk = await group.IsInDisk();
                LoadParam param = new LoadParam();
                param.Alias = _curAlias;
                param.Type = segGroup.Type;
                param.Save = isInDisk == false;

                // TODO: 수정필요, 다른 세그먼트 타입에 대해서 로딩 기능 구현을 안해놓음
                if (segGroup.Type != SegmentType.Changelist)
                    continue;

                ViewModel.TaskMgr.Run(new Load(param));
            }
        }

        
    }
}