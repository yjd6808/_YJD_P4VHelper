// jdyun 24/04/13(토)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using P4VHelper.Base.Extension;

namespace P4VHelper.Customize.Converter
{
    public class FloatAddConverter : IMultiValueConverter
    {
        public static readonly FloatAddConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                throw new Exception("파라미터가 2개 필요합니다.");

            if (!values.All(x => x.IsNumericType()))
                throw new Exception("파라미터가 숫자 타입이어야 합니다.");

            float add = System.Convert.ToSingle(values[0]) + System.Convert.ToSingle(values[1]);
            return add;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
