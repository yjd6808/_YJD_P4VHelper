// jdyun 24/04/16(화)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace P4VHelper.Customize.Converter
{
    public class StringFormatConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length <= 1)
                throw new Exception("또잉? 2개이상 인자를 전달해주세요.");

            if (values[0] is not string)
                throw new Exception("또잉? 1번째 인자가 문자열이 아닌데용?");

            if (values.Length == 2)
                return String.Format((string)values[0], values[1]);
            if (values.Length == 3)
                return String.Format((string)values[0], values[1], values[2]);
            if (values.Length == 4)
                return String.Format((string)values[0], values[1], values[2], values[3]);

            throw new Exception("또잉.. 너무 많은 인자를 전달했어요.. 추가로 if문을 작성해주세요.");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
