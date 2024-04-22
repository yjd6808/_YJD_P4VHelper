// jdyun 24/04/06(토)
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

        public MainViewModel(MainView view)
        {
            View = view;
            Commander = new (this);
            TaskMgr = new BackgroundTaskMgr(8, this);
        }

        public void Loaded()
        {
            Logger = new MainLogger(View.LogListBox);
            Logger.Add(new DebugLogger());
        }
    }
}
