﻿// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using P4VHelper.Base.Logger;
using P4VHelper.Command.MainView;
using P4VHelper.Engine;
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

        public MainViewModel(MainView _view)
        {
            View = _view;
            Commander = new (this);
            TaskMgr = BackgroundTaskMgr.GetInstance(8, this);
            Engine = P4VEngine.Instance;
            Config = Configuration.Load();
        }

        public async Task Loaded()
        {
            Logger = new MainLogger(View.LogListBox);
            Logger.Add(new DebugLogger());
            await Engine.ConnectAsync(Config.P4VConfig);
        }
    }
}
