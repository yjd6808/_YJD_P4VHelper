// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using P4VHelper.Base.Command;
using P4VHelper.Command.MainView.List;
using P4VHelper.ViewModel;

namespace P4VHelper.Command.MainView
{
    public class MainCommander : Commander
    {
        public MainViewModel ViewModel { get; }
        
        public ICommand Test { get; }
        public ICommand ShowStatusDetail { get; }

        public MainCommander(MainViewModel _viewModel)
        {
            ViewModel = _viewModel;

            Add(Test = new Test(_viewModel));
            Add(ShowStatusDetail = new ShowStatusDetail(_viewModel));
        }
    }
}
