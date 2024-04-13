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

namespace P4VHelper.Customize.Converter
{
    public class FloatToMarginConverter : IValueConverter
    {
        public static readonly FloatToMarginConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(0, (float)parameter, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("구현이 안되었어요");
        }
    }
}
