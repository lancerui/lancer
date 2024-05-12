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
    public class LUCalendarDatePickerDaysView : Control
    {
        /// <summary>
        /// 当前选中日期
        /// </summary>
        public System.DateTime SelectedDate { get => (System.DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(System.DateTime), typeof(LUCalendarDatePickerDaysView), new PropertyMetadata(System.DateTime.Now.Date));

        /// <summary>
        /// 可视区域日期改变事件
        /// </summary>
        public event PropertyChangedCallback OnVisibleDatePropertyChanged;
        public DayOfWeek FirstDayOfWeek
        {
            get
            {
                return (DayOfWeek)GetValue(FirstDayOfWeekProperty);
            }
            set
            {
                SetValue(FirstDayOfWeekProperty, value);
            }
        }
        public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register("FirstDayOfWeek", typeof(DayOfWeek), typeof(LUCalendarDatePickerDaysView), new PropertyMetadata(DayOfWeek.Sunday));
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
        public static readonly DependencyProperty DateProperty = DependencyProperty.Register("Date", typeof(System.DateTime), typeof(LUCalendarDatePickerDaysView), new PropertyMetadata(System.DateTime.Now.Date));

        //public ObservableCollection<List<System.DateTime>> Days
        //{
        //    get
        //    {
        //        return (ObservableCollection<List<System.DateTime>>)GetValue(DaysProperty);
        //    }
        //    set
        //    {
        //        SetValue(DaysProperty, value);
        //    }
        //}
        //public static readonly DependencyProperty DaysProperty = DependencyProperty.Register("Days", typeof(ObservableCollection<List<System.DateTime>>), typeof(LUCalendarDatePickerDaysView), new PropertyMetadata(new ObservableCollection<List<System.DateTime>>()));
        public ObservableCollection<List<System.DateTime>> Days { get; set; } = new ObservableCollection<List<System.DateTime>>();
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
        public static readonly DependencyProperty VisibleDateProperty = DependencyProperty.Register("VisibleDate", typeof(System.DateTime), typeof(LUCalendarDatePickerDaysView), new PropertyMetadata(System.DateTime.Now.Date, new PropertyChangedCallback(OnVisibleDateChanged)));

        private static void OnVisibleDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUCalendarDatePickerDaysView;
            control.OnVisibleDatePropertyChanged?.Invoke(d, e);
        }

        private System.DateTime _lastDate, _firstDate;
        private ListView _listView;
        private ScrollViewer _scrollViewer;
        private bool _isNeedCheckVisibleMonth = false;
        private System.DateTime _firstVisibleDate;
        private Grid _weekGrid;
        //static LUCalendarDatePickerDaysView()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(LUCalendarDatePickerDaysView), new FrameworkPropertyMetadata(typeof(LUCalendarDatePickerDaysView)));
        //}
        public LUCalendarDatePickerDaysView()
        {
            DefaultStyleKey = typeof(LUCalendarDatePickerDaysView);
        }

        private string id = Guid.NewGuid().ToString();
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _listView = GetTemplateChild("PART_ListView") as ListView;
            _weekGrid = GetTemplateChild("PART_WeekGrid") as Grid;

            _listView.PreviewMouseWheel += _listView_PreviewMouseWheel;
            _listView.LayoutUpdated += _listView_LayoutUpdated;
            _listView.Loaded += _listView_Loaded;

            CreateWeek();
            //Init();
        }
        private void CreateWeek()
        {
            _weekGrid.Children.Clear();

            //  从周日开始
            string[] keys = new string[] { "Lang_WeekSu", "Lang_WeekMo", "Lang_WeekTu", "Lang_WeekWe", "Lang_WeekTh", "Lang_WeekFr", "Lang_WeekSa" };
            int firstDayOfWeekNum = (int)FirstDayOfWeek;
            int baseNum = 7 - firstDayOfWeekNum;

            for (int i = 0; i < 7; i++)
            {
                var textBlock = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 12,
                    FontWeight = FontWeights.Bold
                };

                textBlock.SetResourceReference(TextBlock.TextProperty, keys[i]);
                textBlock.SetResourceReference(TextBlock.ForegroundProperty, "LUCalendarDatePickerTextBrush");

                int colIndex = i;

                if (firstDayOfWeekNum != 0)
                {
                    if (i < firstDayOfWeekNum)
                    {
                        colIndex = baseNum + i;
                    }
                    else
                    {
                        colIndex = i - firstDayOfWeekNum;
                    }
                }

                Grid.SetColumn(textBlock, colIndex);
                _weekGrid.Children.Add(textBlock);
            }
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
                    AppendDays();
                }
                if (_scrollViewer.VerticalOffset + _scrollViewer.ScrollableHeight == 0)
                {
                    AppendDays(3);
                }
                //Debug.WriteLine("VerticalOffset->" + _scrollViewer.VerticalOffset);
                //Debug.WriteLine("ScrollableHeight->" + _scrollViewer.ScrollableHeight);
            }
            else
            {
                //  向上
                if (_scrollViewer.VerticalOffset == 0)
                {
                    PrependDays();
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
            //var lastDate = (firstVisibleItem.Content as List<System.DateTime>)[6];

            int firstDateDays = System.DateTime.DaysInMonth(_firstVisibleDate.Year, _firstVisibleDate.Month);
            if (_firstVisibleDate.Day <= firstDateDays / 2)
            {
                VisibleDate = _firstVisibleDate.Date;
            }
            else
            {
                VisibleDate = _firstVisibleDate.AddMonths(1).Date;
            }

            //Debug.WriteLine("当前可视区域月份 -> " + VisibleDate + " 月");
        }

        public void Init()
        {
            //  加载选中日期

            Days.Clear();
            VisibleDate = Date.Date;

            DayOfWeek firstDayWeek = new System.DateTime(Date.Year, Date.Month, 1).DayOfWeek;
            int preDays = (int)firstDayWeek - (int)FirstDayOfWeek;
            List<System.DateTime> days = new List<System.DateTime>();

            for (int i = 0; i < 7; i++)
            {
                var date = new System.DateTime(Date.Year, Date.Month, 1).AddDays(-preDays + i);
                days.Add(date);
            }
            _firstDate = days[0];
            _lastDate = days[6];
            Days.Add(days);
            AppendDays(5);
        }

        private void AppendDays(int row)
        {
            for (int i = 0; i < row; i++)
            {
                AppendDays();
            }
        }
        private void AppendDays()
        {
            var dates = new List<System.DateTime>();
            for (int i = 0; i < 7; i++)
            {
                var date = _lastDate.AddDays(i + 1);
                dates.Add(date);
            }
            Days.Add(dates);
            _lastDate = dates[6];
        }
        private void PrependDays()
        {
            var dates = new List<System.DateTime>();
            for (int i = 6; i >= 0; i--)
            {
                var date = _firstDate.AddDays(-(i + 1));
                dates.Add(date);
            }
            Days.Insert(0, dates);
            _firstDate = dates[0];
        }

        /// <summary>
        /// 滚动到前一个月
        /// </summary>
        public void ScrollToPreMonth()
        {
            //  这里偷懒不计算滚动直接重新渲染
            Date = VisibleDate.AddMonths(-1);
            Init();
        }
        /// <summary>
        /// 滚动到下一个月
        /// </summary>
        public void ScrollToNextMonth()
        {
            Date = VisibleDate.AddMonths(1);
            Init();
        }
    }
}
