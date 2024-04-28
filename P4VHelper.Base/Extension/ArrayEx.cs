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
        public static T[] Create<T>(int _count, Func<T> _genFunc) where T : class
        {
            T[] array = new T[_count];
            for (int i = 0; i < _count; ++i)
                array[i] = _genFunc();
            return array;
        }
    }
}
