// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class ICollectionEx
    {
        public static void AddRange<T>(this ICollection<T> collection, ICollection<T> values)
        {
            foreach (var value in values)
                collection.Add(value);
        }
    }
}
