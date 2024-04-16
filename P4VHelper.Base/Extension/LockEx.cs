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
        public static T Do<T>(object locker, Func<T> func)
        {
            Debug.Assert(locker != null);
            lock (locker)
            {
                return func();
            }
        }

        public static void Do(object locker, Action action)
        {
            Debug.Assert(locker != null);
            lock (locker)
            {
                action();
            }
        }
    }
}
