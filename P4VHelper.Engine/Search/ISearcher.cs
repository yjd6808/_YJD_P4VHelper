// jdyun 24/04/21(일)
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Search
{
    public interface ISearcher<T>
    {
        public void Search(List<T> list, SearchParam param);
    }
}
