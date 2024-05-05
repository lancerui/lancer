using LancerUI.Controls.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LancerUI.Controls.SelectorBar
{
    public class LUSelectorBarItem : Control
    {
        internal LUSelectorBar ParentSelectorBar
        {
            get
            {
                LUSelectorBar selectorBar = ItemsControl.ItemsControlFromItemContainer(this) as LUSelectorBar;
                if (selectorBar == null)
                {
                    selectorBar = ItemsControl.ItemsControlFromItemContainer(TemplatedParent) as LUSelectorBar;
                }
                return selectorBar;
            }
        }

        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }
        public static readonly System.Windows.DependencyProperty IsPressedProperty =
            System.Windows.DependencyProperty.Register("IsPressed", typeof(bool), typeof(LUSelectorBarItem), new System.Windows.PropertyMetadata(false));
        public IconSymbol Icon
        {
            get { return (IconSymbol)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly System.Windows.DependencyProperty IconProperty =
            System.Windows.DependencyProperty.Register("Icon", typeof(IconSymbol), typeof(LUSelectorBarItem), new System.Windows.PropertyMetadata(IconSymbol.None));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly System.Windows.DependencyProperty TextProperty =
            System.Windows.DependencyProperty.Register("Text", typeof(string), typeof(LUSelectorBarItem), new System.Windows.PropertyMetadata(string.Empty));
        public LUSelectorBarItem()
        {
            DefaultStyleKey = typeof(LUSelectorBarItem);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (!IsEnabled) return;

            IsPressed = true;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsPressed = false;
            ParentSelectorBar.NotifySelectorBarItemMouseUp(this);
        }
    }
}
