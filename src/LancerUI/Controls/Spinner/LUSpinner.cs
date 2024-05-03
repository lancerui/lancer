using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LancerUI.Controls.Spinner
{
    public class LUSpinner : Control
    {
        public double SpinnerSize
        {
            get => (double)GetValue(SpinnerSizeProperty);
            set => SetValue(SpinnerSizeProperty, value);
        }
        public static readonly System.Windows.DependencyProperty SpinnerSizeProperty = System.Windows.DependencyProperty.Register("SpinnerSize", typeof(double), typeof(LUSpinner), new System.Windows.PropertyMetadata(18.0));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly System.Windows.DependencyProperty TextProperty = System.Windows.DependencyProperty.Register("Text", typeof(string), typeof(LUSpinner), new System.Windows.PropertyMetadata(string.Empty));
        public LUSpinner()
        {
            DefaultStyleKey = typeof(LUSpinner);
        }
    }
}
