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

            public static int s_Count = 0;

            public SegmentType Type { get; set; } = SegmentType.Max;
            public string Path { get; set; } = string.Empty;
            public string Alias { get; set; } = string.Empty;
            public int Id { get; set; }
            public int CachedSegmentCount { get; set; } = 10;
            public List<Filter> Filters { get; set; } = new();
            public Regex? Regex { get; set; }

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


        public SegmentGroup[] SegGroupMap { get; private set; } = new SegmentGroup[30];

        public SegmentGroup GetSegmentGorupById(int _id)
        {
            if (_id >= SegmentGroup.s_Count)
                throw new Exception($"{_id}은 올바른 세그먼트 그룹 인덱스가 아닙니다.");

            if (SegGroupMap[_id] == null)
                throw new Exception($"{_id}에 해당하는 그룹이 없습니다.");

            return SegGroupMap[_id];
        }

        public SegmentGroup GetSegmentGroup(SegmentType _type, string _alias)
        {
            for (int i = 0; i < SegmentGroup.s_Count; ++i)
            {
                if (SegGroupMap[i].Type == _type && SegGroupMap[i].Alias == _alias)
                    return SegGroupMap[i];
            }

            throw new Exception($"{_type}/{_alias} 해당하는 세그먼트 그룹을 찾을 수 없습니다");
        }

        public void Validate()
        {
            Dictionary<SegmentType, HashSet<string>> dict = new ();
            dict.Add(SegmentType.Changelist, new HashSet<string>());

            for (int i = 0; i < SegmentGroup.s_Count; ++i)
            {
                var group = SegGroupMap[i];
                if (dict[group.Type].Add(group.Alias) == false)
                    throw new Exception($"{group.Type}타입의 세그먼트 그룹에는 {group.Alias}가 이미 존재합니다.");
            }

        }

        public void CopyFrom(P4VConfig _config)
        {
            Uri = _config.Uri;
            UserName = _config.UserName;
            Workspace = _config.Workspace;
            ReadDelay = _config.ReadDelay;
            RefreshSegmentCount = _config.RefreshSegmentCount;
            SegmentSize = _config.SegmentSize;

            SegGroupMap = new SegmentGroup[30];

            for (int i = 0; i < SegmentGroup.s_Count; ++i)
                SegGroupMap[i] = _config.SegGroupMap[i];
        }
    }

}
