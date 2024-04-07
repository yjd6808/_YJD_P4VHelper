// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.SearchEngine
{
    public enum SearchRule
    {
        Regex,      // 정규식 검색
        Exact,      // 정확히 일치하는 문자열 검색
        Contain,    // 포함하는 물자열 검색
    }
}
