// jdyun 24/04/21(일)
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Notification;

namespace P4VHelper.Engine.Cache
{
    public class L3Cache<T> : ISearcher<T> where T : class
    {
        private SortedSet<T> _recent;     // 신규 로딩된 데이터 목록
        private List<T> _initial;         // 기존 로딩된 데이터 목록

        /// <summary>
        /// 제일 최신 녀석
        /// </summary>
        public T? First
        {
            get
            {
                if (_recent.Count > 0)
                    return _recent.First();

                if (_initial.Count > 0)
                    return _initial.First();

                return null;
            }
        }

        /// <summary>
        /// 제일 오래된 녀석
        /// </summary>
        public T? Last
        {
            get
            {
                if (_initial.Count > 0)
                    return _initial.Last();

                if (_recent.Count > 0)
                    return _recent.Last();

                return null;
            }
        }

        public SortedSet<T> Recent => _recent;
        public List<T> Initial => _initial;

        public L3Cache(int initialCapacity, IComparer<T> _comparer)
        {
            _recent = new SortedSet<T>(_comparer);
            _initial = new List<T>(initialCapacity);
        }

        public void Search(List<T> list, SearchParam param)
        {


        }
    }
}
