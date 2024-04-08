// jdyun 24/04/07(일)
// 무지성 코딩 ㅈㅅ, 폐기물
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Extension;

namespace P4VHelper.Base.SearchEngine
{
    public class Set<T, TList> : ISearchEngine<T, TList> 
        where T : ISearchTarget, new()
        where TList : ICollection<T>, new()
    {
        /// <summary>
        /// 타겟들이 저장될 컨테이너
        /// </summary>
        private SortedSet<T> _set = new();

        /// <summary>
        /// _set에 동일한 타겟이 이미 포함된 경우 피신용도
        /// </summary>
        private Dictionary<int, List<T>> _equals = new();


        public void Add(T target)
        {
            if (_set.Add(target))
                return;

            bool hasDefault = _set.TryGetValue(target, out T defaultTarget);
            if (hasDefault == false)
                throw new Exception("타겟을 넣는데 실패했는데 기존 타겟이 없습니다.");

            if (_equals.ContainsKey(defaultTarget.Id))
            {
                _equals.GetValue(defaultTarget.Id).Add(target);
            }
            else
            {
                var list = new List<T>(1) { target };
                _equals.Add(defaultTarget.Id, list);
            }
        }

        public bool TryAdd(T target)
        {
            if (_set.Add(target))
                return false;

            bool hasDefault = _set.TryGetValue(target, out T defaultTarget);
            if (hasDefault == false)
                return false;

            if (_equals.ContainsKey(defaultTarget.Id))
            {
                _equals.GetValue(defaultTarget.Id).Add(target);
            }
            else
            {
                var list = new List<T>(1) { target };
                _equals.Add(defaultTarget.Id, list);
            }

            return true;
        }

        public TList SearchExact(string pattern)
        {
            TList list = new TList();
            T dummy = new T();
            dummy.Target = new StringBuilder(pattern);
            bool hasValue = _set.TryGetValue(dummy, out T value);
            if (hasValue == false)
                return list;
            
            list.Add(value);
            if (_equals.TryGetValue(value.Id, out List<T> otherValues))
                list.AddRange(otherValues);

            return list;
        }

        public TList SearchContain(string pattern)
        {
            throw new NotImplementedException();
        }

        public TList SearchRegex(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
