// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.SearchEngine
{
    internal interface ISearchEngine<T, TList> 
        where T : ISearchTarget
        where TList : ICollection<T>, new()
    {
        /// <summary>
        /// 검색 대상을 넣는다.
        /// </summary>
        /// <param name="target">검색 대상</param>
        void Add(T target);
        bool TryAdd(T target);

        /// <summary>
        /// 주어진 조건에 맞춰 패턴 검색을 수행한다.
        /// </summary>
        /// <param name="rule">검색 규칙</param>
        /// <param name="pattern">검색하고자하는 패턴</param>
        /// <returns>매칭 결과로 반환된 대상 목록</returns>
        TList Search(SearchRule rule, string pattern)
        {
            switch (rule)
            {
                case SearchRule.Regex:
                    return SearchRegex(pattern);
                case SearchRule.Exact:
                    return SearchExact(pattern);
                case SearchRule.Contain:
                    return SearchContain(pattern);
                default:
                    throw new ArgumentOutOfRangeException(nameof(rule), rule, null);
            }
        }
        TList SearchExact(string pattern);
        TList SearchContain(string pattern);
        TList SearchRegex(string pattern);
    }
}
