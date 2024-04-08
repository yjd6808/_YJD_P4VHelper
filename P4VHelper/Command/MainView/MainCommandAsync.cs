using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Command;
using P4VHelper.ViewModel;

namespace P4VHelper.Command.MainView
{
    internal class MainCommandAsync : CommandAsync
    {
        protected MainViewModel ViewModel { get; }
        public MainCommandAsync(MainViewModel viewModel, string description, Action<Exception>? errorHandler) : base(description, errorHandler)
        {
            ViewModel = viewModel;
        }
    }
}
