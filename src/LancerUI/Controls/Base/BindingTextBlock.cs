using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LancerUI.Controls.Base
{
    public class BindingTextBlock : Control
    {
        public object Text
        {
            get { return GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(object), typeof(BindingTextBlock), new PropertyMetadata(null));
        public BindingTextBlock()
        {
            DefaultStyleKey = typeof(BindingTextBlock);

        }
    }
}
