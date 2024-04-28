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
        public MainCommandAsync(MainViewModel _viewModel, string _description, Action<Exception>? _errorHandler) : base(_description, _errorHandler)
        {
            ViewModel = _viewModel;
        }
    }
}
