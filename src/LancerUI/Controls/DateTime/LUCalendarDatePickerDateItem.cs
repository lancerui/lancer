using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LancerUI.Controls.DateTime
{
    public class LUCalendarDatePickerDateItem : Control
    {
        /// <summary>
        /// 选中日期
        /// </summary>
        public System.DateTime SelectedDate { get => (System.DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(System.DateTime), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(System.DateTime.Now.Date, new PropertyChangedCallback(OnPropertyChanged)));
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(false));

        /// <summary>
        /// 当前可视区域日期
        /// </summary>
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
        public static readonly DependencyProperty VisibleDateProperty = DependencyProperty.Register("VisibleDate", typeof(System.DateTime), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(System.DateTime.Now.Date, new PropertyChangedCallback(OnPropertyChanged)));
        /// <summary>
        /// 显示类型
        /// </summary>
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
        public static readonly DependencyProperty DisplayTypeProperty = DependencyProperty.Register("DisplayType", typeof(LUCalendarDatePickerDateType), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(LUCalendarDatePickerDateType.Day, new PropertyChangedCallback(OnPropertyChanged)));

        public System.DateTime Date
        {
            get
            {
                return (System.DateTime)GetValue(DayProperty);
            }
            set
            {
                SetValue(DayProperty, value);
            }
        }
        public static readonly DependencyProperty DayProperty = DependencyProperty.Register("Date", typeof(System.DateTime), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(System.DateTime.Now, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUCalendarDatePickerDateItem;
            if (e.OldValue != e.NewValue)
            {
                control.UpdateState();
            }
        }

        public bool IsFirstDay
        {
            get
            {
                return (bool)GetValue(IsFirstDayProperty);
            }
            set
            {
                SetValue(IsFirstDayProperty, value);
            }
        }
        public static readonly DependencyProperty IsFirstDayProperty = DependencyProperty.Register("IsFirstDay", typeof(bool), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(false));

        public bool IsVisibleDate
        {
            get
            {
                return (bool)GetValue(IsVisibleDateProperty);
            }
            set
            {
                SetValue(IsVisibleDateProperty, value);
            }
        }
        public static readonly DependencyProperty IsVisibleDateProperty = DependencyProperty.Register("IsVisibleDate", typeof(bool), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(true));
        public bool IsToday { get { return (bool)GetValue(IsTodayProperty); } set { SetValue(IsTodayProperty, value); } }
        public static readonly DependencyProperty IsTodayProperty = DependencyProperty.Register("IsToday", typeof(bool), typeof(LUCalendarDatePickerDateItem), new PropertyMetadata(false));
        static LUCalendarDatePickerDateItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LUCalendarDatePickerDateItem), new FrameworkPropertyMetadata(typeof(LUCalendarDatePickerDateItem)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateState();
        }

        private void UpdateState()
        {
            if (DisplayType == LUCalendarDatePickerDateType.Day)
            {
                IsFirstDay = Date.Day == 1;
                IsVisibleDate = Date.Year == VisibleDate.Year && Date.Month == VisibleDate.Month;
                IsToday = Date.Date == System.DateTime.Now.Date;
                IsSelected = Date.Date == SelectedDate.Date;
            }
            else if (DisplayType == LUCalendarDatePickerDateType.Month)
            {
                IsFirstDay = Date.Month == 1;
                IsToday = Date.Year == System.DateTime.Now.Year && Date.Month == System.DateTime.Now.Month;
            }
            else
            {
                IsVisibleDate = true;
                IsToday = Date.Year == System.DateTime.Now.Year;
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (DisplayType == LUCalendarDatePickerDateType.Day)
            {
                LUCalendarDatePickerCommands.SelectDayCommand.Execute(Date, this);
            }
            else if(DisplayType== LUCalendarDatePickerDateType.Month)
            {
                LUCalendarDatePickerCommands.SelectMonthCommand.Execute(Date, this);
            }
            else
            {
                LUCalendarDatePickerCommands.SelectYearsCommand.Execute(Date, this);
            }
        }
    }
}
