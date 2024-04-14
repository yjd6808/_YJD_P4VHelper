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
        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(Property);
        }

        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(Property, value);
        }

        public static readonly DependencyProperty Property =
            DependencyProperty.RegisterAttached("Image", typeof(ImageSource), typeof(ImageSourceProperty), new UIPropertyMetadata((ImageSource)null));
    }
}
