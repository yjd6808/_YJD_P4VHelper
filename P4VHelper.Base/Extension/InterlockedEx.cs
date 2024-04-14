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
            public static int Get(ref int target)
            {
                return Interlocked.CompareExchange(ref target, 0, 0);
            }

            public static int Set(ref int target, int value)
            {
                return Interlocked.Exchange(ref target, value);
            }

            public static int Inc(ref int target)
            {
                return Interlocked.Increment(ref target);
            }

            public static int Dec(ref int target)
            {
                return Interlocked.Decrement(ref target);
            }

            public static int Add(ref int target, int value)
            {
                return Interlocked.Add(ref target, value);
            }
        }

        public static class Bool
        {
            public static bool Get(ref int target)
            {
                return Convert.ToBoolean(Interlocked.CompareExchange(ref target, 1, 1));
            }

            public static int Set(ref int target, bool value)
            {
                if (value)
                    return Interlocked.CompareExchange(ref target, 1, 0);

                return Interlocked.CompareExchange(ref target, 0, 1);
            }
        }
    }
}
