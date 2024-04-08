using P4VHelper.Base.Command;
using P4VHelper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Command.MainView
{
    internal class MainCommand : Base.Command.Command
    {
        protected MainViewModel ViewModel { get; }
        public MainCommand(MainViewModel viewModel, string description) : base(description)
        {
            ViewModel = viewModel;
        }
    }
}
