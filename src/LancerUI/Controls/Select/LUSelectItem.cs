using LancerUI.Controls.Navigation;
using LancerUI.Controls.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace LancerUI.Controls.Select
{
    public class LUSelectItem : Control
    {
        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            set => SetValue(IsPressedProperty, value);
        }
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(LUSelectItem), new PropertyMetadata(false));
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(LUSelectItem), new PropertyMetadata(false));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LUSelectItem), new PropertyMetadata(string.Empty));
        public IconSymbol Icon
        {
            get => (IconSymbol)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(IconSymbol), typeof(LUSelectItem), new PropertyMetadata(IconSymbol.None));
        public LUSelectItem()
        {
            DefaultStyleKey = typeof(LUSelectItem);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            IsPressed = true;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsPressed = false;
            LUSelectCommands.SelectItemCommand.Execute(DataContext, this);
        }
    }
}
