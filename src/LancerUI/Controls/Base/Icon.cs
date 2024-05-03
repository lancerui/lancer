using LancerUI.Controls.Buttons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using LancerUI.Utils;
using LancerUI.Controls.Types;
using LancerUI.Extensions;

namespace LancerUI.Controls.Base
{
    public class Icon : Control
    {
        public IconSymbol Symbol
        {
            get { return (IconSymbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }
        public static readonly DependencyProperty SymbolProperty =
            DependencyProperty.Register("Symbol",
                typeof(IconSymbol),
                typeof(Icon), new PropertyMetadata(IconSymbol.EmojiSmileSlight, new PropertyChangedCallback(OnSymbolChanged)));

        private static void OnSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Icon;
            if (e.NewValue != e.OldValue)
            {
                control.Unicode = control.Symbol.String();
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
                typeof(Icon));
        public Icon()
        {
            DefaultStyleKey = typeof(Icon);
        }
    }
}
