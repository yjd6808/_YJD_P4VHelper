// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class CollectionEx
    {
        public static void AddRange<T>(this ICollection<T> _collection, ICollection<T> _values)
        {
            foreach (var value in _values)
                _collection.Add(value);
        }
    }
}
