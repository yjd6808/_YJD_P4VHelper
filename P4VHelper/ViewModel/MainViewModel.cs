// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using P4VHelper.Base.Collection;
using P4VHelper.Base.Logger;
using P4VHelper.Command.MainView;
using P4VHelper.Engine;
using P4VHelper.Engine.Model;
using P4VHelper.Logger;
using P4VHelper.Model;
using P4VHelper.View;

namespace P4VHelper.ViewModel
{
    public class MainViewModel : Base.ViewModel
    {
        public MainView View { get; set; }
        public MainCommander Commander { get; }
        public BackgroundTaskMgr TaskMgr { get; }
        public P4VEngine Engine { get; }
        public Configuration Config { get; }
        public SearchState SearchState { get; }
        public SearchResult<P4VChangelist> HistorySearchResult { get; }
        public bool IsLoaded { get; private set; }              // 로딩시 설정됨
        public string TabName { get; set; } = string.Empty;     // 탭 변경시 설정됨

        public MainViewModel(MainView _view)
        {
            View = _view;
            Commander = new(this);
            TaskMgr = BackgroundTaskMgr.GetInstance(8, this);
            Config = Configuration.Load();
            Engine = new P4VEngine(Config.P4VConfig);
            SearchState = new SearchState();
            HistorySearchResult = new SearchResult<P4VChangelist>(this);
        }

        public async Task Loaded()
        {
            IsLoaded = true;
            Logger = new MainLogger(View.LogListBox);
            Logger.Add(new DebugLogger());
        }
    }
}
