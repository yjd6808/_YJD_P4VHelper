// jdyun 24/04/06(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using P4VHelper.Base.Extension;

namespace P4VHelper.Base.Command
{
    public abstract class CommandAsync : Command
    {
        public Action<Exception>? ErrorHandler { get; }
        public CommandAsync(string _description, Action<Exception>? _errorHandler) : base(_description)
        {
            ErrorHandler = _errorHandler;
        }

        public virtual async Task ExecuteAsync()
        {
            throw new NotImplementedException("구현이 안되었어요");
        }

        public override void Execute()
        {
            if (CanExecute(null))
            {
                ExecuteAsync().StartSafe(ErrorHandler);
            }
        }
    }

    public abstract class CommandAsync<T> : Command<T> where T : class
    {
        public Action<Exception>? ErrorHandler { get; }
        public CommandAsync(string _description, Action<Exception>? _errorHandler) : base(_description)
        {
            ErrorHandler = _errorHandler;
        }

        public virtual async Task ExecuteAsync(T _param)
        {
            throw new NotImplementedException("구현이 안되었어요");
        }

        public override void Execute(T _param)
        {
            if (CanExecute(_param))
            {
                ExecuteAsync(_param).StartSafe(ErrorHandler);
            }
        }
    }
}
