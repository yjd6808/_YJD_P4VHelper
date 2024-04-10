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
        public string Name => GetType().Name;
        public string Description { get; } = string.Empty;
        public Action<Exception>? ErrorHandler { get; }
        public CommandAsync(string description, Action<Exception>? errorHandler) : base(description)
        {
            ErrorHandler = errorHandler;
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
        public string Name => GetType().Name;
        public string Description { get; }
        public Action<Exception>? ErrorHandler { get; }
        public CommandAsync(string description, Action<Exception>? errorHandler) : base(description)
        {
            ErrorHandler = errorHandler;
        }

        public virtual async Task ExecuteAsync(T param)
        {
            throw new NotImplementedException("구현이 안되었어요");
        }

        public override void Execute(T param)
        {
            if (CanExecute(param))
            {
                ExecuteAsync(param).StartSafe(ErrorHandler);
            }
        }
    }
}
