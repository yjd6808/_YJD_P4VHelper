// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P4VHelper.Base.Extension;

namespace P4VHelper.Engine.Search
{
    public enum FieldType
    {
        Int,
        DateTime,
        String,
        Max,
    }

    public static class FieldTypeOf
    {
        public static readonly Type[] Types;
        public static readonly Dictionary<Type, FieldType> Fields;

        static FieldTypeOf()
        {
            int size = (int)FieldType.Max;
            Types = new Type[size];
            Types[(int)FieldType.Int] = typeof(int);
            Types[(int)FieldType.DateTime] = typeof(DateTime);
            Types[(int)FieldType.String] = typeof(string);

            Fields = new Dictionary<Type, FieldType>(32);
            Fields.Add(typeof(int), FieldType.Int);
            Fields.Add(typeof(DateTime), FieldType.DateTime);
            Fields.Add(typeof(string), FieldType.String);
        }

        public static bool CanConvert<T>(FieldType type)
        {
            if (Types[(int)type] != typeof(T))
                throw new Exception("타입이 일치하지 않습니다.");
            return true;
        }

        public static FieldType GetType<T>()
        {
            if (Fields.TryGetValue(typeof(T), out var type))
                return type;

            throw new Exception("올바른 필드 타입이 아닙니다.");
        }
    }
}
