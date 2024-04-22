// jdyun 24/04/21(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Engine.Search
{
    public interface IFieldHolder
    {
        public FieldType Type { get; }

        void Set<T>(T value)
        {
            ((FieldHolder<T>)this).Value = value;
        }

        public FieldHolder<T> To<T>()
        {
            if (FieldTypeOf.CanConvert<T>(Type) == false)
                throw new Exception("변환할 수 없는 타입입니다.");

            return ((FieldHolder<T>)this);
        }

        public T Value<T>()
        {
            if (FieldTypeOf.CanConvert<T>(Type) == false)
                throw new Exception("변환할 수 없는 타입입니다.");

            return ((FieldHolder<T>)this).Value;
        }
    }

    public class FieldHolder<T> : IFieldHolder
    {
        public FieldType Type => FieldTypeOf.GetType<T>();
        public T Value { get; set; }
        public FieldHolder(T value)
        {
            Value = value;
        }
    }
}
