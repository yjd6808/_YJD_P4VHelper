// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            {
                IMatcher matcher = new MatcherIntExact();
                s_MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new MatcherIntRegex();
                s_MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new MatcherIntContain();
                s_MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new MatcherStringExact();
                s_MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new MatcherStringRegex();
                s_MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new MatcherStringContain();
                s_MatcherList[matcher.Index] = matcher;
            }
        }

        public static IMatcher Get(FieldType _type, Rule _rule)
        {
            int index = (int)Rule.Max * (int)_type + (int)_rule;

            if (s_MatcherList[index] == null)
                throw new Exception($"해당 하는 매쳐가 존재하지 않습니다 Field:{_type} Rule:{_rule}");

            return s_MatcherList[index];
        }

        public static bool IsMatch<T>(T _searchable, SearchParam _param) where T : ISearchable
        {
            IMatcher matcher = Get(_param.input_.Type, _param.rule_);
            Reflection<T> reflection = ReflectionMgr.Get(_searchable);
            IFieldHolder src = reflection.GetField(_searchable, _param.member_);
            return matcher.Match(src, _param.input_);
        }
    }


    public interface IMatcher
    {
        public Rule Rule { get; }
        public FieldType Type { get; }
        public int Index => (int)Rule.Max * (int)Type + (int)Rule;
        public bool Match(IFieldHolder _src, IFieldHolder _dst);
    }

    public class MatcherIntExact : IMatcher
    {
        public Rule Rule => Rule.Exact;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) => _src.Value<int>() == _dst.Value<int>();
    }

    public class MatcherIntRegex : IMatcher
    {
        public Rule Rule => Rule.Regex;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) => throw new NotImplementedException();
    }

    public class MatcherIntContain : IMatcher
    {
        public Rule Rule => Rule.Contain;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder _src, IFieldHolder _dst)
        {
            string srcStr = _src.ToString();
            string dstStr = _dst.Value<int>().ToString();
            return srcStr.Contains(dstStr);
        }
    }

    public class MatcherStringExact : IMatcher
    {
        public Rule Rule => Rule.Exact;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) => _src.Value<string>() == _dst.Value<string>();
    }

    public class MatcherStringRegex : IMatcher
    {
        public Rule Rule => Rule.Regex;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) => Regex.IsMatch(_src.Value<string>(), _dst.Value<string>());
    }

    public class MatcherStringContain : IMatcher
    {
        public Rule Rule => Rule.Contain;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder _src, IFieldHolder _dst) => _src.Value<string>().Contains(_dst.Value<string>());
    }

}
