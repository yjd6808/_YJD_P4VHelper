// jdyun 24/04/21(일)
using P4VHelper.Base.Collection;
using P4VHelper.Engine.Model;
using P4VHelper.Engine.Notification;
using P4VHelper.Engine.Search;
using P4VHelper.Engine.Cache;
using System.Data;

namespace P4VHelper.Engine
{
    public class Engine : ISearcher<Changelist>
    {
        private L3Cache<Changelist> _cache;
        private readonly IComparer<Changelist> _comparer;

        public Engine(int capacity)
        {
            _comparer = Changelist.DefaultComparer;
            _cache = new L3Cache<Changelist>(capacity, _comparer);
        }

        public List<Changelist> Search(SearchParam param)
        {
            var results = new List<Changelist>();
            _cache.Search(results, param);
            Search(results, param);
            return results;
        }

        public Task<List<Changelist>> SearchAsync(SearchParam param)
        {
            return Task.Run(() => Search(param));
        }

        public void Search(List<Changelist> list, SearchParam param)
        {

        }
    }
}
