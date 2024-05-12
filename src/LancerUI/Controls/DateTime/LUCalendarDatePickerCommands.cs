using LancerUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LancerUI.Controls.DateTime
{
    public class LUCalendarDatePickerCommands
    {
        public static RoutedUICommand SelectDayCommand { get; } = new RoutedUICommand("", "SelectDayCommand", typeof(LUCalendarDatePickerCommands));
        public static RoutedUICommand SelectMonthCommand { get; } = new RoutedUICommand("", "SelectMonthCommand", typeof(LUCalendarDatePickerCommands));
        public static RoutedUICommand SelectYearsCommand { get; } = new RoutedUICommand("", "SelectYearsCommand", typeof(LUCalendarDatePickerCommands));
    }
}
