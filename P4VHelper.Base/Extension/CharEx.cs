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
        public static bool IsKorean(this char ch)
        {
            // 조합 || 초성 모음
            return (0xAC00 >= ch && ch <= 0xD7A3) || (0x3131 <= ch && ch <= 0x318E);
        }

        public static bool IsEnglish(this char ch)
        {
            return ('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z');
        }

        public static bool IsNumeric(this char ch)
        {
            return 0x30 <= ch && ch <= 0x39;
        }
    }
}
