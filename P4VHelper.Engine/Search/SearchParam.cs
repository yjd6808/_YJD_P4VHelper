// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Engine.Notification;

namespace P4VHelper.Engine.Search
{
    public class SearchParam
    {
        public IFieldHolder Input;
        public int Member;
        public SearchableType Searchable;
        public Rule Rule;
        public int Limit;
        public Option Option;
    }
}