// jdyun 24/04/13(토)
using P4VHelper.Base.Extension;
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
    public class DoubleToMarginConverter : IMultiValueConverter
    {
        public static readonly DoubleToMarginConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 1 && values.Length != 4 && values.Length != 5)
                throw new Exception("파라미터가 1개 또는 4개 또는 5개 필요합니다.");

            if (values.Length == 1)
            {
                if (!values.All(x => x.IsNumericType()))
                    throw new Exception("파라미터가 숫자 타입이어야 합니다.");

                double f = System.Convert.ToDouble(values[0]);
                return new Thickness(f, f, f, f);
            }
            if (values.Length == 4)
            {
                if (!values.All(x => x.IsNumericType()))
                    throw new Exception("파라미터가 숫자 타입이어야 합니다.");

                double f1 = System.Convert.ToDouble(values[0]);
                double f2 = System.Convert.ToDouble(values[1]);
                double f3 = System.Convert.ToDouble(values[2]);
                double f4 = System.Convert.ToDouble(values[3]);
                return new Thickness(f1, f2, f3, f4);
            }
            if (values.Length == 5)
            {
                if (!values[0].IsNumericType() ||
                    !values[1].IsNumericType() ||
                    !values[2].IsNumericType() ||
                    !values[3].IsNumericType())
                    throw new Exception("1, 2, 3, 4 파라미터가 숫자 타입이어야 합니다.");

                if (values[4] is not Thickness)
                    throw new Exception("5 파라미터가 Thickness 타입이어야 합니다.");

                double f1 = System.Convert.ToDouble(values[0]);
                double f2 = System.Convert.ToDouble(values[1]);
                double f3 = System.Convert.ToDouble(values[2]);
                double f4 = System.Convert.ToDouble(values[3]);
                Thickness r = new Thickness(f1, f2, f3, f4);
                Thickness o = (Thickness)values[4];

                r.Bottom += o.Bottom;
                r.Top += o.Top;
                r.Left += o.Left;
                r.Right += o.Right;
                return r;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
