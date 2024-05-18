using LancerUI.Language;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LancerUI.Controls.Base
{

    [ValueConversion(typeof(object), typeof(string))]
    public class BindingTextBlockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is string) return value;
            if (value is System.DateTime)
            {
                var dateTime = (System.DateTime)value;
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
