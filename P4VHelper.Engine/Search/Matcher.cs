// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using P4VHelper.Engine.Collection;

namespace P4VHelper.Engine.Search
{
    public class Matchers
    {
        public static readonly IMatcher[] s_MatcherList;

        static Matchers()
        {
            FieldType maxField = FieldType.Max;
            Rule maxRule = Rule.Max;

            s_MatcherList = new IMatcher[(int)maxField * (int)maxRule];

            IMatcher matcherInt0 = new MatcherIntContain();
            IMatcher matcherInt1 = new MatcherIntExact();
            IMatcher matcherInt2 = new MatcherIntStartWith();
            IMatcher matcherInt3 = new MatcherIntEndWith();
            IMatcher matcherInt4 = new MatcherIntRegex();

            IMatcher matcherString0 = new MatcherStringContain();
            IMatcher matcherString1 = new MatcherStringExact();
            IMatcher matcherString2 = new MatcherStringStartWith();
            IMatcher matcherString3 = new MatcherStringEndWith();
            IMatcher matcherString4 = new MatcherStringRegex();

            s_MatcherList[matcherInt0.Index] = matcherInt0;
            s_MatcherList[matcherInt1.Index] = matcherInt1;
            s_MatcherList[matcherInt2.Index] = matcherInt2;
            s_MatcherList[matcherInt3.Index] = matcherInt3;
            s_MatcherList[matcherInt4.Index] = matcherInt4;

            s_MatcherList[matcherString0.Index] = matcherString0;
            s_MatcherList[matcherString1.Index] = matcherString1;
            s_MatcherList[matcherString2.Index] = matcherString2;
            s_MatcherList[matcherString3.Index] = matcherString3;
            s_MatcherList[matcherString4.Index] = matcherString4;
        }

        public static IMatcher Get(FieldType _type, Rule _rule)
        {
            int index = (int)Rule.Max * (int)_type + (int)_rule;

            if (s_MatcherList[index] == null)
                throw new Exception($"해당 하는 매쳐가 존재하지 않습니다 Field:{_type} Rule:{_rule}");

            return s_MatcherList[index];
        }

        public static bool IsMatch<T>(T _searchable, SearchParam _param) where T : ISegmentElement
        {
            IMatcher matcher = Get(_param.Input.Type, _param.Rule);
            Reflection<T> reflection = ReflectionMgr.Get(_searchable);
            IFieldHolder src = reflection.GetField(_searchable, _param.Member);
            return matcher.Match(src, _param.Input);
        }
    }


    public interface IMatcher
    {
        public Rule Rule { get; }
        public FieldType Type { get; }
        public int Index => (int)Rule.Max * (int)Type + (int)Rule;
        public bool Match(IFieldHolder _src, IFieldHolder _dst);
    }

    public class MatcherIntContain : IMatcher
    {
        public Rule Rule => Rule.Contain;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst)
            => _src.Value<int>().ToString().Contains(_dst.Value<int>().ToString());
    }

    public class MatcherIntExact : IMatcher
    {
        public Rule Rule => Rule.Exact;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) 
            => _src.Value<int>() == _dst.Value<int>();
    }

    public class MatcherIntStartWith : IMatcher
    {
        public Rule Rule => Rule.StartWith;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst)
            => _src.Value<int>().ToString().StartsWith(_dst.Value<int>().ToString());
    }

    public class MatcherIntEndWith : IMatcher
    {
        public Rule Rule => Rule.EndWith;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst)
            => _src.Value<int>().ToString().EndsWith(_dst.Value<int>().ToString());
    }

    public class MatcherIntRegex : IMatcher
    {
        public Rule Rule => Rule.Regex;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) 
            => throw new NotImplementedException();
    }

    public class MatcherStringContain : IMatcher
    {
        public Rule Rule => Rule.Contain;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) => _src.Value<string>().Contains(_dst.Value<string>());
    }

    public class MatcherStringExact : IMatcher
    {
        public Rule Rule => Rule.Exact;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst)
            => _src.Value<string>() == _dst.Value<string>();
    }

    public class MatcherStringStartWith : IMatcher
    {
        public Rule Rule => Rule.StartWith;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst)
            => _src.Value<string>().StartsWith(_dst.Value<string>());
    }

    public class MatcherStringEndWith : IMatcher
    {
        public Rule Rule => Rule.EndWith;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst)
            => _src.Value<string>().EndsWith(_dst.Value<string>());
    }

    public class MatcherStringRegex : IMatcher
    {
        public Rule Rule => Rule.Regex;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) => Regex.IsMatch(_src.Value<string>(), _dst.Value<string>());
    }
}

