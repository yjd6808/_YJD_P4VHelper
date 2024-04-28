// jdyun 24/04/21(일)
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Notifier;
using P4VHelper.Engine.Notification;

namespace P4VHelper.Engine.Cache
{
    /// <summary>
    /// 파일 Read는 성능이 많이 안좋으므로... 최신 정보들은 캐싱해놓도록 한다.
    /// </summary>
    public class L3Cache<T> : ISearcher<T> where T : 
        ISearchable, 
        IComparable<T>
    {
        /// <summary>
        /// 신규 로딩된 데이터 목록
        /// </summary>
        private readonly SortedSet<T> recent_;

        /// <summary>
        /// 기존 로딩된 데이터 목록
        /// </summary>
        private readonly List<T> initial_;         

        /// <summary>
        /// 제일 최신 녀석
        /// </summary>
        public T? First
        {
            get
            {
                if (recent_.Count > 0)
                    return recent_.First();

                if (initial_.Count > 0)
                    return initial_.First();

                return default;
            }
        }

        /// <summary>
        /// 제일 오래된 녀석
        /// </summary>
        public T? Last
        {
            get
            {
                if (initial_.Count > 0)
                    return initial_.Last();

                if (recent_.Count > 0)
                    return recent_.Last();

                return default;
            }
        }
        
        public int TotalCount => initial_.Count + recent_.Count;
        public SortedSet<T> Recent => recent_;
        public List<T> Initial => initial_;

        public L3Cache(int _initialCapacity, IComparer<T> _comparer)
        {
            recent_ = new SortedSet<T>(_comparer);
            initial_ = new List<T>(_initialCapacity);
        }

        public void Search(List<T> _list, SearchParam _param)
        {
            foreach (T searchable in recent_)
            {
                if (Matchers.IsMatch(searchable, _param))
                {
                    // notifier.Progress(notifierSlot);
                }
            }


        }
    }
}
