// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P4VHelper.Base.Command
{
    public abstract class Command : ICommand, INamedCommand
    {
        public string Description { get; }

        public Command(string _description)
        {
            Description = _description;
        }

        public bool CanExecute(object? _parameter)
        {
            return true;
        }

        public void Execute(object? _parameter)
        {
            Execute();
        }

        public virtual void Execute()
        {
            // TODO: 자식에서 구현
        }

        public event EventHandler? CanExecuteChanged;
    }

    public abstract class Command<T> : ICommand, INamedCommand where T : class
    {
        public string Description { get; }
        public Command(string _description)
        {
            Description = _description;
        }

        public virtual bool CanExecute(object? _parameter)
        {
            if (_parameter is not T)
            {
                return false;
            }

            return true;
        }

        public void Execute(object? _parameter)
        {
            if (_parameter is T param)
            {
                Execute(param);
            }
        }

        public virtual void Execute(T _param)
        {
            // TODO: 자식에서 구현
        }

        public event EventHandler? CanExecuteChanged;
    }
}
