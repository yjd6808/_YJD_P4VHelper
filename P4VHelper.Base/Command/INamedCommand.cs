using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Command
{
    public interface INamedCommand
    {
        public string Name => GetType().Name;
        public string Description { get; }
    }
}
