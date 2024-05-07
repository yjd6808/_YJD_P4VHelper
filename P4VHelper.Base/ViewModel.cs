// jdyun 24/04/06(토)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Logger;

namespace P4VHelper.Base
{
    public class ViewModel : Bindable
    {
        public Logger.Logger? Logger { get; protected set; }
        public Dictionary<string, object?> Vars { get; } = new ();
    }
}
