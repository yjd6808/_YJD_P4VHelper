// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Command;
using P4VHelper.Command.MainView.List;
using P4VHelper.ViewModel;

namespace P4VHelper.Command.MainView
{
    public class MainCommander : Commander
    {
        public MainViewModel ViewModel { get; }
        
        public Base.Command.Command Test { get; }
        public Base.Command.Command ShowStatusDetail { get; }

        public MainCommander(MainViewModel viewModel)
        {
            ViewModel = viewModel;

            Add(Test = new Test(viewModel));
            Add(ShowStatusDetail = new ShowStatusDetail(viewModel));
        }
    }
}
