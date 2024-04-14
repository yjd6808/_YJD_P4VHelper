using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace P4VHelper.Customize.Control
{
    public class ImageToggleButton : Button
    {
        public static readonly DependencyProperty ToggledProperty =
            DependencyProperty.Register(nameof(Toggled), typeof(bool), typeof(ImageToggleButton), new PropertyMetadata(false));

        public bool Toggled
        {
            get { return (bool)GetValue(ToggledProperty); }
            set
            {
                SetValue(ToggledProperty, value);
                UpdateCurrentImageSource();
            }
        }

        public static readonly DependencyProperty NormalImageSourceProperty =
            DependencyProperty.RegisterAttached(nameof(NormalImageSource), typeof(ImageSource), typeof(ImageToggleButton), new UIPropertyMetadata((ImageSource)null));

        public ImageSource NormalImageSource
        {
            get { return (ImageSource)GetValue(NormalImageSourceProperty); }
            set { SetValue(NormalImageSourceProperty, value); }
        }

        public static readonly DependencyProperty ToggledImageSourceProperty =
            DependencyProperty.RegisterAttached(nameof(ToggledImageSource), typeof(ImageSource), typeof(ImageToggleButton), new UIPropertyMetadata((ImageSource)null));

        public ImageSource ToggledImageSource
        {
            get { return (ImageSource)GetValue(ToggledImageSourceProperty); }
            set { SetValue(ToggledImageSourceProperty, value); }
        }

        private static readonly DependencyPropertyKey CurrentImageSourcePropertyKey
            = DependencyProperty.RegisterReadOnly(
                nameof(CurrentImageSource),
                typeof(ImageSource), typeof(ImageToggleButton),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CurrentImageSourceProperty
            = CurrentImageSourcePropertyKey.DependencyProperty;

        public ImageSource CurrentImageSource
        {
            get
            {
                return (ImageSource)GetValue(CurrentImageSourceProperty);
            }
            protected set
            {
                SetValue(CurrentImageSourcePropertyKey, value);
            }
        }

        private void UpdateCurrentImageSource()
        {
            if (!Toggled)
                CurrentImageSource = NormalImageSource;
            else
                CurrentImageSource = ToggledImageSource;
        }

        protected override void OnInitialized(EventArgs e)
        {
            // CurrentImageSource를 처음에 업데이트 하기 위함
            UpdateCurrentImageSource();
            base.OnInitialized(e);
        }

        protected override void OnClick()
        {
            Toggled = !Toggled;
            UpdateCurrentImageSource();
            base.OnClick();
        }
    }
}
