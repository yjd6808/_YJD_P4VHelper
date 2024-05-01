// jdyun 24/04/10(수)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class InterlockedEx
    {
        public static class Int
        {
            public static int Get(ref int _target)
            {
                return Interlocked.CompareExchange(ref _target, 0, 0);
            }

            public static int Set(ref int _target, int _value)
            {
                return Interlocked.Exchange(ref _target, _value);
            }

            public static int Inc(ref int _target)
            {
                return Interlocked.Increment(ref _target);
            }

            public static int Dec(ref int _target)
            {
                return Interlocked.Decrement(ref _target);
            }

            public static int Add(ref int _target, int _value)
            {
                return Interlocked.Add(ref _target, _value);
            }

            public static int Cas(ref int _target, int _comparand, int _value)
            {
                return Interlocked.CompareExchange(ref _target, _value, _comparand);
            }

            public static bool GCas(ref int _target, int _comparand, int _value)
            {
                for (;;)
                {
                    int old = Get(ref _target);
                    if (old > _comparand)
                    {
                        if (Cas(ref _target, _comparand, _value) == old)
                            return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public static class Bool
        {
            public static bool Get(ref int _target)
            {
                return Convert.ToBoolean(Interlocked.CompareExchange(ref _target, 1, 1));
            }

            public static int Set(ref int _target, bool _value)
            {
                if (_value)
                    return Interlocked.CompareExchange(ref _target, 1, 0);

                return Interlocked.CompareExchange(ref _target, 0, 1);
            }

            public static bool Cas(ref int _target, bool _comparand, bool _value)
            {
                int comparand = _comparand ? 1 : 0;
                int value = _value ? 1 : 0;
                int result = Interlocked.CompareExchange(ref _target, value, comparand);
                return Convert.ToBoolean(result);
            }
        }
    }
}
