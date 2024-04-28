// jdyun 24/04/13(토)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace P4VHelper.Customize.Property
{
    public class ImageSourceProperty 
    {
        public static ImageSource GetImage(DependencyObject _obj)
        {
            return (ImageSource)_obj.GetValue(Property);
        }

        public static void SetImage(DependencyObject _obj, ImageSource _value)
        {
            _obj.SetValue(Property, _value);
        }

        public static readonly DependencyProperty Property =
            DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(ImageSourceProperty), new UIPropertyMetadata((ImageSource)null));
    }
}
