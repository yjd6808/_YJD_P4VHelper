using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.SearchEngine
{
    public interface ISearchTarget : IComparable<ISearchTarget>
    {
        int Id { get; }
        StringBuilder Target { get; set; }
    }
}
