using LancerUI.Language;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LancerUI.Controls.DateTime
{

    [ValueConversion(typeof(object), typeof(string))]
    public class LUCalendarDatePickerDateTagConverter : IMultiValueConverter
    {
        public object Convert(
        object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var date = (System.DateTime)values[0];
            var type = (LUCalendarDatePickerDateType)values[1];

            if (type == LUCalendarDatePickerDateType.Year)
            {
                return string.Empty;
            }
            else if (type == LUCalendarDatePickerDateType.Month)
            {
                return date.Year.ToString();
            }
            else
            {
                return LanguageHelper.GetMonthStr(date.Month);
            }
        }

        public object[] ConvertBack(
            object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    if (value == null)
        //    {
        //        return string.Empty;
        //    }
        //    var date = (System.DateTime)value;
        //    var type = (LUCalendarDatePickerDateType)parameter;
        //    if(type== LUCalendarDatePickerDateType.Day)
        //    {
        //        return date.Day.ToString();
        //    }
        //    else if(type== LUCalendarDatePickerDateType.Month)
        //    {
        //        return date.Month.ToString();
        //    }
        //    else
        //    {
        //        return date.Year.ToString();
        //    }
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
