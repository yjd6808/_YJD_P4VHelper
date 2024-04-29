// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Search
{
    public enum Rule
    {
        //[Description("포함")]
        Contain,
        //[Description("완전히 일치")]
        Exact,
        //[Description("시작 포함")]
        StartWith,
        //[Description("마지막 포함")]
        EndWith,
        //[Description("정규표현식")]
        Regex,

        [Browsable(false)]
        Max,
    }
}
