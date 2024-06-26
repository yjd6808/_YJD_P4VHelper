﻿// jdyun 24/04/07(일)
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class DictionaryEx
    {
        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> _dictionary, TKey _key)
        {
            if (_dictionary.TryGetValue(_key, out TValue value))
            {
                return value;
            }

            throw new Exception("딕셔너리에서 Key를 찾지 못했습니다.");
        }
    }
}
