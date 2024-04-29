// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Search
{
    public enum Rule
    {
        Contain,    // 포함하는 문자열 검색
        Exact,      // 정확히 일치하는 문자열 검색
        StartWith,  // 시작하는 문자열 검색
        EndWith,    // 끝나는 문자열 검색
        Regex,      // 정규식 검색
        Max,
    }
}
