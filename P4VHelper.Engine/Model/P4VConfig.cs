// jdyun 24/04/26(금)
using P4VHelper.Base;

namespace P4VHelper.Engine.Model
{
    public class P4VConfig
    {
        public class Searcher
        {
            public string Path { get; set; }
            public string Alias { get; set; }
            public int CachedSegmentCount { get; set; }
        }

        public string Uri { get; set; }
        public string UserName { get; set; }
        public string Workspace { get; set; }
        public int ReadDelay { get; set; }
        public int RefreshSize { get; set; }
        public int SegmentSize { get; set; }

        public List<Searcher> Searchers { get; } = new();

        public Searcher GetSearcherByAlias(string _alias)
        {
            foreach (Searcher searcher in Searchers)
            {
                if (searcher.Alias == _alias)
                    return searcher;
            }
            
            throw new Exception($"{_alias}이름에 해당하는 서쳐를 찾을 수 없습니다");
        }

        public void Validate()
        {
            HashSet<string> s = new ();
            foreach (Searcher searcher in Searchers)
            {
                if (s.Add(searcher.Alias) == false)
                    throw new Exception($"동일한 이름의 서쳐가 존재합니다. {searcher.Alias}");
            }
        }

        public void CopyFrom(P4VConfig _config)
        {
            Uri = _config.Uri;
            UserName = _config.UserName;
            Workspace = _config.Workspace;
            ReadDelay = _config.ReadDelay;
            RefreshSize = _config.RefreshSize;
            SegmentSize = _config.SegmentSize;

            Searchers.Clear();
            Searchers.AddRange(_config.Searchers);
        }
    }

}
