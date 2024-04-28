// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Search
{
    public interface ISearchable
    {
        SearchableType SearchableType { get; }
        int Key { get; }

        bool Match(SearchParam _param) => Matchers.IsMatch(this, _param);
    }
}
