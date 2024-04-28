using LancerUI.Controls.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using LancerUI.Utils;

namespace LancerUI.Controls.Base
{
    public class Icon : Control
    {
        public IconTypes IconType
        {
            get { return (IconTypes)GetValue(IconTypeProperty); }
            set { SetValue(IconTypeProperty, value); }
        }
        public static readonly DependencyProperty IconTypeProperty =
            DependencyProperty.Register("IconType",
                typeof(IconTypes),
                typeof(Icon), new PropertyMetadata(IconTypes.Back, new PropertyChangedCallback(OnIconTypeChanged)));

        private static void OnIconTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Icon;
            if (e.NewValue != e.OldValue)
            {
                control.Unicode = IconFontConverter.ToUnicode((IconTypes)e.NewValue);
            }
        }

        public string Unicode
        {
            get { return (string)GetValue(UnicodeProperty); }
            set { SetValue(UnicodeProperty, value); }
        }
        public static readonly DependencyProperty UnicodeProperty =
        DependencyProperty.Register("Unicode",
                typeof(string),
                typeof(Icon),
                new PropertyMetadata(IconFontConverter.ToUnicode(IconTypes.Back)));
        public Icon()
        {
            DefaultStyleKey = typeof(Icon);
        }
    }
}
