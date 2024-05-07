// jdyun 24/04/06(토)
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void SetVar(string _key, object _value)
        {
            if (!Vars.TryAdd(_key, _value))
            {
                Vars[_key] = _value;
            }
        }

        public T GetVar<T>(string _key)
        {
            if (Vars.TryGetValue(_key, out object? value))
            {
                if (value is not T)
                    throw new InvalidOperationException();
                return (T)value;
            }

            return default(T);
        }
    }
}
