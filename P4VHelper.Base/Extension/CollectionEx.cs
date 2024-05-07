// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class CollectionEx
    {
        // @출처: https://stackoverflow.com/questions/9337039/why-is-there-no-reverseenumerator-in-c
        public static IEnumerable<T> ReverseEx<T>(this IEnumerable<T> coll)
        {
            var quick = coll as IList<T>;
            if (quick == null)
            {
                foreach (T item in coll.Reverse()) 
                    yield return item;
            }
            else
            {
                for (int ix = quick.Count - 1; ix >= 0; --ix)
                {
                    yield return quick[ix];
                }
            }
        }
        public static void AddRange<T>(this ICollection<T> _collection, ICollection<T> _values)
        {
            foreach (var value in _values)
                _collection.Add(value);
        }

        public static int RemoveAllIf<T>(this LinkedList<T> _list, Predicate<T> _predicate)
        {
            if (_list == null)
                throw new ArgumentNullException(nameof(_list));

            if (_predicate == null)
                throw new ArgumentNullException(nameof(_predicate));

            var count = 0;
            var node = _list.First;
            while (node != null)
            {
                var next = node.Next;
                if (_predicate(node.Value))
                {
                    _list.Remove(node);
                    count++;
                }
                node = next;
            }
            return count;
        }

        public static int AddRange<T>(this List<T> _list, List<T> _otherList, int offset, int count)
        {
            if (_otherList == null)
            {
                Debug.Assert(false);
                return 0;
            }

            if (count <= 0)
            {
                Debug.Assert(false);
                return 0;
            }

            if (offset >= _otherList.Count)
            {
                Debug.Assert(false);
                return 0;
            }

            int copyCount = 0;

            for (int i = offset; i < _otherList.Count; ++i)
            {
                _list.Add(_otherList[i]);
                copyCount++;

                if (copyCount >= count)
                    break;
            }

            return copyCount;
        }
    }
}
