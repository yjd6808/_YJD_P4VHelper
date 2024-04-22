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
        public static readonly IMatcher[] MatcherList;

        static Matchers()
        {
            FieldType maxField = FieldType.Max;
            Rule maxRule = Rule.Max;

            MatcherList = new IMatcher[(int)maxField * (int)maxRule];

            {
                IMatcher matcher = new Matcher_IntExact();
                MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new Matcher_IntRegex();
                MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new Matcher_IntContain();
                MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new Matcher_StringExact();
                MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new Matcher_StringRegex();
                MatcherList[matcher.Index] = matcher;
            }

            {
                IMatcher matcher = new Matcher_StringContain();
                MatcherList[matcher.Index] = matcher;
            }
        }

        public static IMatcher Get(FieldType type, Rule rule)
        {
            int index = (int)Rule.Max * (int)type + (int)rule;

            if (MatcherList[index] == null)
                throw new Exception($"해당 하는 매쳐가 존재하지 않습니다 Field:{type} Rule:{rule}");

            return MatcherList[index];
        }

        public static bool IsMatch<T>(T searchable, SearchParam param) where T : ISearchable
        {
            IMatcher matcher = Get(param.Input.Type, param.Rule);
            Reflection<T> reflection = ReflectionMgr.Get(searchable);
            IFieldHolder src = reflection.GetField(searchable, param.Member);
            return matcher.Match(src, param.Input);
        }
    }


    public interface IMatcher
    {
        public Rule Rule { get; }
        public FieldType Type { get; }
        public int Index => (int)Rule.Max * (int)Type + (int)Rule;
        public bool Match(IFieldHolder src, IFieldHolder dst);
    }

    public class Matcher_IntExact : IMatcher
    {
        public Rule Rule => Rule.Exact;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder src, IFieldHolder dst) => src.Value<int>() == dst.Value<int>();
    }

    public class Matcher_IntRegex : IMatcher
    {
        public Rule Rule => Rule.Regex;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder src, IFieldHolder dst) => throw new NotImplementedException();
    }

    public class Matcher_IntContain : IMatcher
    {
        public Rule Rule => Rule.Contain;
        public FieldType Type => FieldType.Int;
        public bool Match(IFieldHolder src, IFieldHolder dst)
        {
            string srcStr = src.ToString();
            string dstStr = dst.Value<int>().ToString();
            return srcStr.Contains(dstStr);
        }
    }

    public class Matcher_StringExact : IMatcher
    {
        public Rule Rule => Rule.Exact;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder src, IFieldHolder dst) => src.Value<string>() == dst.Value<string>();
    }

    public class Matcher_StringRegex : IMatcher
    {
        public Rule Rule => Rule.Regex;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder src, IFieldHolder dst) => Regex.IsMatch(src.Value<string>(), dst.Value<string>());
    }

    public class Matcher_StringContain : IMatcher
    {
        public Rule Rule => Rule.Contain;
        public FieldType Type => FieldType.String;
        public bool Match(IFieldHolder src, IFieldHolder dst) => src.Value<string>().Contains(dst.Value<string>());
    }

}
