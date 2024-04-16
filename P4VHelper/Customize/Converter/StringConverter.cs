// jdyun 24/04/16(화)
using P4VHelper.Base.Extension;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace P4VHelper.Customize.Converter
{
    public class StringConverter : IMultiValueConverter
    {
        public static readonly StringConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 1)
                throw new Exception("value가 1개가 아닙니다.");

            return values[0].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
