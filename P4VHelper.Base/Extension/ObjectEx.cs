// jdyun 24/04/13(토)
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class ObjectEx
    {
        // https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number
        public static bool IsNumericType(this object _o)
        {
            switch (Type.GetTypeCode(_o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        // https://stackoverflow.com/questions/4223589/does-properties-increase-memory-size-of-instances
        // 인스턴스 메모리 크기 계산
        public static double SizeOf<T>(this object _o, int _testCount = 10000) where T : new()
        {
            object[] array = new object[_testCount];
            long initialMemory = GC.GetTotalMemory(true);

            for (int i = 0; i < _testCount; i++)
                array[i] = new T();

            long finalMemory = GC.GetTotalMemory(true);
            GC.KeepAlive(array);
            long total = finalMemory - initialMemory;
            return (double)total / _testCount;
        }
    }
}
