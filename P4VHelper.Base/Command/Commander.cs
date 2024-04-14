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
        private bool _finalized;
        private Dictionary<string, INamedCommand> _commandMap = new();

        public void Add(ICommand command)
        {
            INamedCommand? namedCommand = command as INamedCommand;
            if (namedCommand == null)
                throw new Exception("올바르지 않은 커맨드 타입입니다.");

            if (_commandMap.ContainsKey(namedCommand.Name))
                throw new Exception($"{namedCommand.Name} 커맨드가 이미 존재합니다.");

            _commandMap.Add(namedCommand.Name, namedCommand);
        }

        public void Execute(string commandName, object? param = null)
        {
            if (_finalized)
                throw new Exception("이미 파이날라이즈드 된 커맨드 센터입니다.");

            if (!_commandMap.ContainsKey(commandName))
                throw new Exception($"{commandName} 커맨드를 실행할 수 없습니다.");

            (_commandMap[commandName]as ICommand).Execute(param);
        }

        public void Finalize()
        {
            _commandMap.Clear();
            _finalized = true;
        }
    }
}
