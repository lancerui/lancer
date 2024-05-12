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
    public class LUCalendarDatePickerYearsView : Control
    {
        private readonly int MIN_YEAR = 1924;
        private readonly int MAX_YEAR = 2124;
        /// <summary>
        /// 可视区域日期改变事件
        /// </summary>
        public event PropertyChangedCallback OnVisibleDatePropertyChanged;
        /// <summary>
        /// 选择器类型
        /// </summary>
        public LUCalendarDatePickerDateType PickerType { get => (LUCalendarDatePickerDateType)GetValue(PickerTypeProperty); set => SetValue(PickerTypeProperty, value); }
        public static readonly DependencyProperty PickerTypeProperty = DependencyProperty.Register("PickerType", typeof(LUCalendarDatePickerDateType), typeof(LUCalendarDatePickerYearsView), new PropertyMetadata(LUCalendarDatePickerDateType.Day));
        /// <summary>
        /// 当前选中日期
        /// </summary>
        public System.DateTime SelectedDate { get => (System.DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(System.DateTime), typeof(LUCalendarDatePickerYearsView), new PropertyMetadata(System.DateTime.Now.Date));
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
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(System.DateTime), typeof(LUCalendarDatePickerYearsView), new PropertyMetadata(System.DateTime.Now.Date));

        public ObservableCollection<List<System.DateTime>> Years { get; set; } = new ObservableCollection<List<System.DateTime>>();
        //public ObservableCollection<List<System.DateTime>> Years
        //{
        //    get
        //    {
        //        return (ObservableCollection<List<System.DateTime>>)GetValue(YearsProperty);
        //    }
        //    set
        //    {
        //        SetValue(YearsProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty YearsProperty = DependencyProperty.Register("Years", typeof(ObservableCollection<List<System.DateTime>>), typeof(LUCalendarDatePickerYearsView), new PropertyMetadata(new ObservableCollection<List<System.DateTime>>()));

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
        public static readonly DependencyProperty VisibleDateProperty = DependencyProperty.Register("VisibleDate", typeof(System.DateTime), typeof(LUCalendarDatePickerYearsView), new PropertyMetadata(System.DateTime.Now.Date, new PropertyChangedCallback(OnVisibleDateChanged)));

        private static void OnVisibleDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUCalendarDatePickerYearsView;
            control.OnVisibleDatePropertyChanged?.Invoke(d, e);
        }

        private System.DateTime _lastDate, _firstDate;
        private ListView _listView;
        private ScrollViewer _scrollViewer;
        private bool _isNeedCheckVisibleMonth = false;
        private System.DateTime _firstVisibleDate = System.DateTime.Now.Date;
        private bool _isMaxYear = false, _isMinYear = false;
        static LUCalendarDatePickerYearsView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LUCalendarDatePickerYearsView), new FrameworkPropertyMetadata(typeof(LUCalendarDatePickerYearsView)));
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
                if (_isMaxYear) return;

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
                if (_isMinYear) return;

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

            //Debug.WriteLine("当前可视区域年份 -> " + _firstVisibleDate.Year + " 年");
        }

        public void Init()
        {
            //  加载选中日期
            _isMinYear = false;
            _isMaxYear = false;
            Years.Clear();
            VisibleDate = Date.Date;
            _firstVisibleDate = Date.Date;

            List<System.DateTime> years = new List<System.DateTime>();
            int preYears = 0;
            int diff = Date.Year - MIN_YEAR;
            int lines = diff / 4;
            if (lines != (double)diff / 4)
            {
                preYears = diff - lines * 4;
            }

            for (int i = 0; i < 4; i++)
            {
                var date = new System.DateTime(Date.Year, 1, 1).AddYears(-preYears + i);
                if (date.Year > MAX_YEAR)
                {
                    _isMaxYear = true;
                    break;
                }
                years.Add(date);
            }

            _firstDate = years[0];
            _lastDate = years[years.Count - 1];
            Years.Add(years);
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
            if (_isMaxYear) return;
            var dates = new List<System.DateTime>();
            for (int i = 0; i < 4; i++)
            {
                var date = _lastDate.AddYears(i + 1);
                if (date.Year > MAX_YEAR)
                {
                    _isMaxYear = true;
                    break;
                }
                dates.Add(date);
            }
            if (dates.Count == 0) return;
            Years.Add(dates);
            _lastDate = dates[dates.Count - 1];
        }
        private void Prepend()
        {
            if (_isMinYear) return;
            var dates = new List<System.DateTime>();
            for (int i = 3; i >= 0; i--)
            {
                var date = _firstDate.AddYears(-(i + 1));
                if (date.Year < MIN_YEAR)
                {
                    _isMinYear = true;
                    break;
                }
                dates.Add(date);
            }
            if (dates.Count == 0) return;
            Years.Insert(0, dates);
            _firstDate = dates[0];
        }

        /// <summary>
        /// 滚动到前一个月
        /// </summary>
        public void ScrollToPre()
        {
            //  这里偷懒不计算滚动直接重新渲染
            if (_isMinYear || _firstVisibleDate.Year <= MIN_YEAR) return;

            Date = _firstVisibleDate.AddYears(-4);
            Init();
        }
        /// <summary>
        /// 滚动到下一个月
        /// </summary>
        public void ScrollToNext()
        {
            if (_isMaxYear) return;

            Date = _firstVisibleDate.AddYears(4);
            Init();
        }
    }
}
