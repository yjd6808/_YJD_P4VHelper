// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class ArrayEx
    {
        public static T[] Create<T>(int count, Func<T> genFunc) where T : class
        {
            T[] array = new T[count];
            for (int i = 0; i < count; ++i)
                array[i] = genFunc();
            return array;
        }
    }
}
