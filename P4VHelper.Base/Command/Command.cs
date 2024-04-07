// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P4VHelper.Base.Command
{
    public abstract class Command : ICommand
    {
        public string Name => GetType().Name;
        public string Description { get; }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Execute();
        }

        public abstract void Execute();

        public event EventHandler? CanExecuteChanged;
    }

    public abstract class Command<T> : ICommand where T : class
    {
        public string Name => GetType().Name;
        public string Description { get; }

        public virtual bool CanExecute(object? parameter)
        {
            if (parameter is not T)
            {
                return false;
            }

            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is T param)
            {
                Execute(param);
            }
        }

        public abstract void Execute(T param);
        public event EventHandler? CanExecuteChanged;
    }
}
