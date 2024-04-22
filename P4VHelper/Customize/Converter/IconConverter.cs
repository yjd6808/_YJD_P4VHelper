// jdyun 24/04/08(월)
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using P4VHelper.Resource;

namespace P4VHelper.Customize.Converter
{
    public class IconConverter : IValueConverter
    {
        public static readonly IconConverter Instance = new();
        private Dictionary<string, ImageSource> _cached = new ();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string)
                throw new Exception("문자열을 인자로 전달해주세요.");

            var iconFileName = (string)parameter;
            ImageSource result = null;
            if (_cached.TryGetValue(iconFileName, out result))
                return result;

            result = new BitmapImage(new Uri(R.ICON_PATH + iconFileName));
            _cached.Add(iconFileName, result);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("구현이 안되었어요");
        }
    }
}
