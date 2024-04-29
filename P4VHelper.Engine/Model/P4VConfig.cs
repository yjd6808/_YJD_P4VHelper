// jdyun 24/04/26(금)

using System.Text;
using System.Text.RegularExpressions;
using P4VHelper.Base;
using P4VHelper.Base.Extension;
using P4VHelper.Engine.Collection;

namespace P4VHelper.Engine.Model
{
    public class P4VConfig
    {
        public class SegmentGroup
        {
            public enum FilterMode
            {
                StartWith,
                Contain,
                EndWith,
            }

            public enum FilterType
            {
                String,
                Type,
            }

            public class Filter
            {
                public FilterMode Mode { get; set; }
                public FilterType Type { get; set; }
                public string Value { get; set; } = string.Empty;
            }

            private string path_ = string.Empty;
            private string alias_ = string.Empty;

            public SegmentType Type { get; set; } = SegmentType.Max;
            public SegmentGroup? Ref { get; set; }

            public string Path
            {
                get => IsRef ? Ref.Path : path_;
                set
                {
                    if (IsRef) 
                        throw new Exception("레프 세그먼트 컨피그는 패쓰설정이 불가능함");

                    path_ = value;
                }
            }

            public string Alias
            {
                get => IsRef ? Ref.Path : path_;
                set
                {
                    if (IsRef)
                        throw new Exception("레프 세그먼트 컨피그는 얼라이어스설정이 불가능함");
                    alias_ = value;
                }
            }

            public int Id { get; set; }
            public int CachedSegmentCount { get; set; } = 10;
            public List<Filter> Filters { get; set; } = new();
            public Regex? Regex { get; set; }

            public bool IsRef => Ref != null;

            public void ConstructRegex()
            {
                StringBuilder patternBuilder = new StringBuilder(200);

                for (int i = 0; i < Filters.Count; ++i)
                {
                    Filter filter = Filters[i];
                    string singlePattern = string.Empty;

                    if (filter.Type == FilterType.String)
                        singlePattern = filter.Value;
                    else if (filter.Type == FilterType.Type)
                    {
                        if (filter.Value == "korean")
                            singlePattern = "[가-힣]";
                        else if (filter.Value == "english")
                            singlePattern = "[a-zA-Z]";
                    }

                    if (filter.Mode == FilterMode.StartWith)
                        singlePattern = singlePattern.Insert(0, "^");
                    else if (filter.Mode == FilterMode.EndWith)
                        singlePattern += '$';

                    patternBuilder.Append(singlePattern);

                    if (i < Filters.Count - 1)
                        patternBuilder.Append('|');
                }

                string pattern = patternBuilder.ToString();

                if (Filters.Count != 0)
                {
                    if (string.IsNullOrEmpty(pattern))
                        throw new Exception("레젝스 패턴 만들기 실패");

                    Regex = new Regex(pattern);
                }
            }

            public bool IsMatch(string _value)
            {
                if (Filters.Count != 0)
                {
                    if (Regex == null)
                        throw new Exception("레젝스가 null임");
                }
                else
                {
                    if (Regex == null)
                        return false;
                }

                return Regex.IsMatch(_value);
            }
        }

        public string Uri { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Workspace { get; set; } = string.Empty;
        public int ReadDelay { get; set; } = 500;
        public int SegmentSize { get; set; } = 5000;
        public int RefreshSegmentCount { get; set; } = 1;

        /// <summary>
        /// 그룹 Id를 인덱스로하는 세그먼트 목록
        /// </summary>
        private SegmentGroup[] segMap_ = new SegmentGroup[30];

        /// <summary>
        /// 그룹 세그먼트 갯수
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 세그먼트 타입별로 모아놓은 목록
        /// </summary>
        private readonly Dictionary<SegmentType, HashSet<SegmentGroup>> segTypeMap_ = new();
        
        /// <summary>
        /// History UI에서 원격 리포지터리 Alias 콤보박스를 초기화하는 용도
        /// </summary>
        public IEnumerable<SegmentGroup> ChangelistSegmentGroup => segTypeMap_[SegmentType.Changelist];

        

        public void AddSegmentGroup(SegmentGroup _group)
        {
            _group.Id = Count;
            if (segMap_[Count] != null)
                throw new Exception("이미 할당된 세그먼트가 존재합니다.");

            segTypeMap_.TryAdd(_group.Type, new HashSet<SegmentGroup>());

            if (segTypeMap_[_group.Type].FirstOrDefault((x) => x.Alias == _group.Alias) != null)
                throw new Exception($"{_group.Type}타입의 세그먼트 그룹에는 {_group.Alias}가 이미 존재합니다.");

            segTypeMap_[_group.Type].Add(_group);
            segMap_[Count] = _group;
            Count++;
        }

        public SegmentGroup GetSegmentGroupById(int _id)
        {
            if (_id >= Count)
                throw new Exception($"{_id}은 올바른 세그먼트 그룹 인덱스가 아닙니다.");

            if (segMap_[_id] == null)
                throw new Exception($"{_id}에 해당하는 그룹이 없습니다.");

            return segMap_[_id];
        }

        public SegmentGroup GetSegmentGroup(SegmentType _type, string _alias)
        {
            for (int i = 0; i < Count; ++i)
            {
                if (segMap_[i].Type == _type && segMap_[i].Alias == _alias)
                    return segMap_[i];
            }

            throw new Exception($"{_type}/{_alias} 해당하는 세그먼트 그룹을 찾을 수 없습니다");
        }

        public void Validate()
        {
        }

        public void CopyFrom(P4VConfig _config)
        {
            Uri = _config.Uri;
            UserName = _config.UserName;
            Workspace = _config.Workspace;
            ReadDelay = _config.ReadDelay;
            RefreshSegmentCount = _config.RefreshSegmentCount;
            SegmentSize = _config.SegmentSize;

            segMap_ = new SegmentGroup[30];

            for (int i = 0; i < Count; ++i)
                segMap_[i] = _config.segMap_[i];
        }
    }

}
