// jdyun 24/04/08(월)
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace P4VHelper.Customize.Converter
{
    public class PackUrlImageSourceConverter : IValueConverter
    {
        public static readonly PackUrlImageSourceConverter Instance = new();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new BitmapImage(new Uri((string)parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("구현이 안되었어요");
        }
    }
}
