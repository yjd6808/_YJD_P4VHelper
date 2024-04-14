using P4VHelper.Base.Command;
using P4VHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Command.MainView
{
    public class MainCommand : Base.Command.Command
    {
        protected MainViewModel ViewModel { get; }
        public MainCommand(MainViewModel viewModel, string description) : base(description)
        {
            ViewModel = viewModel;
        }
    }

    public class MainCommand<T> : Base.Command.Command<T> where T : class
    {
        protected MainViewModel ViewModel { get; }
        public MainCommand(MainViewModel viewModel, string description) : base(description)
        {
            ViewModel = viewModel;
        }
    }
}
