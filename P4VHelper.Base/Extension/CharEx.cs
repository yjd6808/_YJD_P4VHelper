// jdyun 24/04/20(일)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P4VHelper.Base.Extension
{
    public static class CharEx
    {
        public static bool IsKorean(this char _ch)
        {
            // 조합 || 초성 모음
            return (0xAC00 >= _ch && _ch <= 0xD7A3) || (0x3131 <= _ch && _ch <= 0x318E);
        }

        public static bool IsEnglish(this char _ch)
        {
            return ('a' <= _ch && _ch <= 'z') || ('A' <= _ch && _ch <= 'Z');
        }

        public static bool IsNumeric(this char _ch)
        {
            return 0x30 <= _ch && _ch <= 0x39;
        }
    }
}
