// jdyun 24/04/16(수)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class LockEx
    {
        public static T Do<T>(object _locker, Func<T> _func)
        {
            Debug.Assert(_locker != null);
            lock (_locker)
            {
                return _func();
            }
        }

        public static void Do(object _locker, Action _action)
        {
            Debug.Assert(_locker != null);
            lock (_locker)
            {
                _action();
            }
        }
    }
}
