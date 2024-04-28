using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class QueueEx
    {
        public static void AddRange<T>(this Queue<T> _queue, IEnumerable<T> _enu)
        {
            foreach (T obj in _enu)
                _queue.Enqueue(obj);
        }

    }
}
