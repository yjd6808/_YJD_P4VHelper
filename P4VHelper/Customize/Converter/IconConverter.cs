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
        public static readonly IconConverter s_Instance = new();
        private readonly Dictionary<string, ImageSource> cached_ = new ();

        public object Convert(object _value, Type _targetType, object _parameter, CultureInfo _culture)
        {
            if (_parameter is not string)
                throw new Exception("문자열을 인자로 전달해주세요.");

            var iconFileName = (string)_parameter;
            ImageSource result = null;
            if (cached_.TryGetValue(iconFileName, out result))
                return result;

            result = new BitmapImage(new Uri(R.ICON_PATH + iconFileName));
            cached_.Add(iconFileName, result);
            return result;
        }

        public object ConvertBack(object _value, Type _targetType, object _parameter, CultureInfo _culture)
        {
            throw new NotImplementedException("구현이 안되었어요");
        }
    }
}
