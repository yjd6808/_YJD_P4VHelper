using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace P4VHelper.Base.Command
{
    public class Commander
    {
        private bool finalized_;
        private readonly Dictionary<string, INamedCommand> commandMap_ = new();

        public void Add(ICommand _command)
        {
            INamedCommand? namedCommand = _command as INamedCommand;
            if (namedCommand == null)
                throw new Exception("올바르지 않은 커맨드 타입입니다.");

            if (commandMap_.ContainsKey(namedCommand.Name))
                throw new Exception($"{namedCommand.Name} 커맨드가 이미 존재합니다.");

            commandMap_.Add(namedCommand.Name, namedCommand);
        }

        public void Execute(string _commandName, object? _param = null)
        {
            if (finalized_)
                throw new Exception("이미 파이날라이즈드 된 커맨드 센터입니다.");

            if (!commandMap_.ContainsKey(_commandName))
                throw new Exception($"{_commandName} 커맨드를 실행할 수 없습니다.");

            (commandMap_[_commandName]as ICommand).Execute(_param);
        }

        public void Finalize()
        {
            commandMap_.Clear();
            finalized_ = true;
        }
    }
}
