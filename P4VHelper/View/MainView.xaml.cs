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

namespace P4VHelper.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainViewModel ViewModel { get; }

        public MainView()
        {
            ViewModel = new MainViewModel(this);

            InitializeComponent();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Loaded();

            ViewModel.TaskMgr.Run(new Test(ViewModel.TaskMgr));
            await Task.Delay(500);
            ViewModel.TaskMgr.Run(new Test(ViewModel.TaskMgr));
            await Task.Delay(500);
            ViewModel.TaskMgr.Run(new Test(ViewModel.TaskMgr));
            await Task.Delay(500);
            ViewModel.TaskMgr.Run(new Test(ViewModel.TaskMgr));
        }

        private void OnClosing(object? sender, CancelEventArgs e)
        {
            if (ViewModel.TaskMgr.RunningThreadCount > 0)
            {
                if (MessageBox.Show("아직 실행중인 작업이 있습니다.\n정말로 종료하시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ViewModel.TaskMgr.Stop();
                    return;
                }
                e.Cancel = true;
            }

            ViewModel.TaskMgr.Stop();
        }
    }
}