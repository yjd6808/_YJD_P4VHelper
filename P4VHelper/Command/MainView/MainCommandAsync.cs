using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Command;
using P4VHelper.ViewModel;

namespace P4VHelper.Command.MainView
{
    public abstract class MainCommandAsync : CommandAsync
    {
        protected MainViewModel ViewModel { get; }
        public MainCommandAsync(MainViewModel _viewModel, string _description, Action<Exception>? _errorHandler = null) : base(_description, _errorHandler)
        {
            ViewModel = _viewModel;
        }
    }

    public abstract class MainCommandAsync<T> : CommandAsync<T> where T : class
    {
        protected MainViewModel ViewModel { get; }
        public MainCommandAsync(MainViewModel _viewModel, string _description, Action<Exception>? _errorHandler = null) : base(_description, _errorHandler)
        {
            ViewModel = _viewModel;
        }
    }
}
