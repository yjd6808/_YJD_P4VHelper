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
        public static readonly StringConverter s_Instance = new();

        public object Convert(object[] _values, Type _targetType, object _parameter, CultureInfo _culture)
        {
            if (_values.Length != 1)
                throw new Exception("value가 1개가 아닙니다.");

            return _values[0].ToString();
        }

        public object[] ConvertBack(object _value, Type[] _targetTypes, object _parameter, CultureInfo _culture)
        {
            throw new NotImplementedException();
        }
    }
}
