using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LancerUI.Controls.DateTime
{
    public class LUCalendarDatePickerDateGroup : Control
    {
        public System.DateTime SelectedDate { get=> (System.DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(System.DateTime), typeof(LUCalendarDatePickerDateGroup), new PropertyMetadata(System.DateTime.Now.Date));
        public System.DateTime VisibleDate
        {
            get
            {
                return (System.DateTime)GetValue(VisibleDateProperty);
            }
            set
            {
                SetValue(VisibleDateProperty, value);
            }
        }
        public static readonly DependencyProperty VisibleDateProperty = DependencyProperty.Register("VisibleDate", typeof(System.DateTime), typeof(LUCalendarDatePickerDateGroup), new PropertyMetadata(System.DateTime.Now.Date));

        public LUCalendarDatePickerDateType DisplayType
        {
            get
            {
                return (LUCalendarDatePickerDateType)GetValue(DisplayTypeProperty);
            }
            set
            {
                SetValue(DisplayTypeProperty, value);
            }
        }
        public static readonly DependencyProperty DisplayTypeProperty = DependencyProperty.Register("DisplayType", typeof(LUCalendarDatePickerDateType), typeof(LUCalendarDatePickerDateGroup), new PropertyMetadata(LUCalendarDatePickerDateType.Day));
        public List<System.DateTime> Dates
        {
            get
            {
                return (List<System.DateTime>)GetValue(DatesProperty);
            }
            set
            {
                SetValue(DatesProperty, value);
            }
        }
        public static readonly DependencyProperty DatesProperty = DependencyProperty.Register("Dates", typeof(List<System.DateTime>), typeof(LUCalendarDatePickerDateGroup), new PropertyMetadata(new List<System.DateTime>()));
        static LUCalendarDatePickerDateGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LUCalendarDatePickerDateGroup), new FrameworkPropertyMetadata(typeof(LUCalendarDatePickerDateGroup)));
        }
    }
}
