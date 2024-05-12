using LancerUI.Controls.Buttons;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LancerUI.Controls.DateTime
{
    public class LUCalendarDatePickerButton : Control
    {
        /// <summary>
        /// 当前选择日期
        /// </summary>
        public System.DateTime SelectedDate { get => (System.DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(System.DateTime), typeof(LUCalendarDatePickerButton), new PropertyMetadata(System.DateTime.Now.Date));
        /// <summary>
        /// 日期格式
        /// </summary>
        public string Format { get => (string)GetValue(FormatProperty); set => SetValue(FormatProperty, value); }
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(string), typeof(LUCalendarDatePickerButton), new PropertyMetadata("yyyy-MM-dd"));
        /// <summary>
        /// 选择器类型
        /// </summary>
        public LUCalendarDatePickerDateType PickerType { get => (LUCalendarDatePickerDateType)GetValue(PickerTypeProperty); set => SetValue(PickerTypeProperty, value); }
        public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register("PickerType", typeof(LUCalendarDatePickerDateType), typeof(LUCalendarDatePickerButton), new PropertyMetadata(LUCalendarDatePickerDateType.Day));

        private LUButton _button;
        private Popup _popup;
        private LUCalendarDatePicker _calendar;
        static LUCalendarDatePickerButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LUCalendarDatePickerButton), new FrameworkPropertyMetadata(typeof(LUCalendarDatePickerButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _button = GetTemplateChild("PART_Button") as LUButton;
            _popup = GetTemplateChild("PART_Popup") as Popup;
            _calendar = GetTemplateChild("PART_CalendarDatePicker") as LUCalendarDatePicker;
            _calendar.SelectedDate = SelectedDate;
            _calendar.OnSelectedDateChanged += _calendar_OnSelectedDateChanged; ;
            _button.Click += _button_Click;
            UpdateSelectedDateText();
        }

        private void _calendar_OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SelectedDate = (System.DateTime)e.NewValue;
            _popup.IsOpen = false;
            UpdateSelectedDateText();
        }

        private void _button_Click(object sender, RoutedEventArgs e)
        {
            _popup.IsOpen = true;
        }

        private void UpdateSelectedDateText()
        {
            _button.Content = SelectedDate.ToString(Format);
        }
    }
}
