using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace LancerUI.Controls.DateTime
{
    public class LUCalendarDatePickerMonthsView : Control
    {
        private readonly int MIN_YEAR = 1924;
        private readonly int MAX_YEAR = 2124;
        /// <summary>
        /// 选择器类型
        /// </summary>
        public LUCalendarDatePickerDateType PickerType { get=> (LUCalendarDatePickerDateType)GetValue(PickerTypeProperty); set => SetValue(PickerTypeProperty, value); }
        public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register("PickerType", typeof(LUCalendarDatePickerDateType), typeof(LUCalendarDatePickerMonthsView), new PropertyMetadata(LUCalendarDatePickerDateType.Day));
        /// <summary>
        /// 当前选中日期
        /// </summary>
        public System.DateTime SelectedDate { get => (System.DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(System.DateTime), typeof(LUCalendarDatePickerMonthsView), new PropertyMetadata(System.DateTime.Now.Date));

        /// <summary>
        /// 可视区域日期改变事件
        /// </summary>
        public event PropertyChangedCallback OnVisibleDatePropertyChanged;

        public System.DateTime Date
        {
            get
            {
                return (System.DateTime)GetValue(DateProperty);
            }
            set
            {
                SetValue(DateProperty, value);
            }
        }
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(System.DateTime), typeof(LUCalendarDatePickerMonthsView), new PropertyMetadata(System.DateTime.Now.Date));

        //public ObservableCollection<List<System.DateTime>> Months
        //{
        //    get
        //    {
        //        return (ObservableCollection<List<System.DateTime>>)GetValue(MonthsProperty);
        //    }
        //    set
        //    {
        //        SetValue(MonthsProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty MonthsProperty = DependencyProperty.Register("Months", typeof(ObservableCollection<List<System.DateTime>>), typeof(LUCalendarDatePickerMonthsView), new PropertyMetadata(new ObservableCollection<List<System.DateTime>>()));

        public ObservableCollection<List<System.DateTime>> Months { get; set; } = new ObservableCollection<List<System.DateTime>>();
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
        public static readonly DependencyProperty VisibleDateProperty = DependencyProperty.Register("VisibleDate", typeof(System.DateTime), typeof(LUCalendarDatePickerMonthsView), new PropertyMetadata(System.DateTime.Now.Date, new PropertyChangedCallback(OnVisibleDateChanged)));

        private static void OnVisibleDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUCalendarDatePickerMonthsView;
            control.OnVisibleDatePropertyChanged?.Invoke(d, e);
        }

        private System.DateTime _lastDate, _firstDate;
        private ListView _listView;
        private ScrollViewer _scrollViewer;
        private bool _isNeedCheckVisibleMonth = false;
        private System.DateTime _firstVisibleDate;
        static LUCalendarDatePickerMonthsView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LUCalendarDatePickerMonthsView), new FrameworkPropertyMetadata(typeof(LUCalendarDatePickerMonthsView)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listView = GetTemplateChild("PART_ListView") as ListView;
            _listView.PreviewMouseWheel += _listView_PreviewMouseWheel;
            _listView.LayoutUpdated += _listView_LayoutUpdated;
            _listView.Loaded += _listView_Loaded;
            //Init();
        }

        private void _listView_Loaded(object sender, RoutedEventArgs e)
        {
            _scrollViewer = VisualTreeHelper.GetChild(_listView, 0) as ScrollViewer;
            _scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        }


        private void _listView_LayoutUpdated(object sender, EventArgs e)
        {
            CheckVisibleMonth();
        }


        private void _listView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_scrollViewer == null) return;

            if (e.Delta < 0)
            {
                //  向下
                if (_scrollViewer.VerticalOffset >= _scrollViewer.ScrollableHeight - 2)
                {
                    Append();
                }
                if (_scrollViewer.VerticalOffset + _scrollViewer.ScrollableHeight == 0)
                {
                    Append(3);
                }
            }
            else
            {
                //  向上
                if (_scrollViewer.VerticalOffset == 0)
                {
                    Prepend();
                }
            }
            _isNeedCheckVisibleMonth = true;
        }

        //  检测当前可视区域月份
        private void CheckVisibleMonth()
        {
            if (!_isNeedCheckVisibleMonth) return;
            _isNeedCheckVisibleMonth = false;

            var firstVisibleItem = _listView.ItemContainerGenerator.ContainerFromIndex((int)_scrollViewer.VerticalOffset) as ListViewItem;
            if (firstVisibleItem == null) return;

            _firstVisibleDate = (firstVisibleItem.Content as List<System.DateTime>)[0];
            //int firstDateDays = System.DateTime.DaysInMonth(_firstVisibleDate.Year, _firstVisibleDate.Month);
            if (_firstVisibleDate.Month < 9)
            {
                VisibleDate = _firstVisibleDate.Date;
            }
            else
            {
                VisibleDate = _firstVisibleDate.AddYears(1).Date;
            }

            //Debug.WriteLine("当前可视区域年份 -> " + VisibleDate.Year + " 年");
        }

        public void Init()
        {
            //  加载选中日期

            Months.Clear();
            VisibleDate = Date.Date;

            List<System.DateTime> months = new List<System.DateTime>();

            for (int i = 0; i < 4; i++)
            {
                var date = new System.DateTime(Date.Year, 1, 1).AddMonths(i);
                months.Add(date);
            }
            _firstDate = months[0];
            _lastDate = months[3];
            Months.Add(months);
            Append(3);
        }

        private void Append(int row)
        {
            for (int i = 0; i < row; i++)
            {
                Append();
            }
        }
        private void Append()
        {
            var dates = new List<System.DateTime>();
            for (int i = 0; i < 4; i++)
            {
                var date = _lastDate.AddMonths(i + 1);
                dates.Add(date);
            }
            Months.Add(dates);
            _lastDate = dates[3];
        }
        private void Prepend()
        {
            var dates = new List<System.DateTime>();
            for (int i = 3; i >= 0; i--)
            {
                var date = _firstDate.AddMonths(-(i + 1));
                dates.Add(date);
            }
            Months.Insert(0, dates);
            _firstDate = dates[0];
        }

        /// <summary>
        /// 滚动到前一个月
        /// </summary>
        public void ScrollToPre()
        {
            //  这里偷懒不计算滚动直接重新渲染
            Date = VisibleDate.AddYears(-1);
            Init();
        }
        /// <summary>
        /// 滚动到下一个月
        /// </summary>
        public void ScrollToNext()
        {
            Date = VisibleDate.AddYears(1);
            Init();
        }
    }
}
