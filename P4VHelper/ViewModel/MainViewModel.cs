// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using P4VHelper.Base.Logger;
using P4VHelper.Command.MainView;
using P4VHelper.Logger;
using P4VHelper.Model.Main;
using P4VHelper.View;

namespace P4VHelper.ViewModel
{
    public class MainViewModel : Base.ViewModel
    {
        public MainView View { get; set; }
        public MainCommander Commander { get; }
        public ObservableCollection<Changelist> HistoryChangelists { get; }

        public MainViewModel()
        {
            Commander = new (this);
            HistoryChangelists = new ();
        }

        public void Loaded()
        {
            Logger = new MainLogger(View.LogListBox);
            Logger.Add(new DebugLogger());
        }
    }
}
