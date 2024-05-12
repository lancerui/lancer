using LancerUI.Controls.Buttons;
using LancerUI.Language;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace LancerUI.Controls.DateTime
{
    public class LUCalendarDatePicker : Control
    {
        public event PropertyChangedCallback OnSelectedDateChanged;
        /// <summary>
        /// 当前选择日期
        /// </summary>
        public System.DateTime SelectedDate { get => (System.DateTime)GetValue(SelectedDateProperty); set => SetValue(SelectedDateProperty, value); }
        public static readonly DependencyProperty SelectedDateProperty = DependencyProperty.Register("SelectedDate", typeof(System.DateTime), typeof(LUCalendarDatePicker), new PropertyMetadata(System.DateTime.Now.Date));
        /// <summary>
        /// 一周的第一天
        /// </summary>
        public DayOfWeek FirstDayOfWeek { get => (DayOfWeek)GetValue(FirstDayOfWeekProperty); set => SetValue(FirstDayOfWeekProperty, value); }
        public static readonly DependencyProperty FirstDayOfWeekProperty = DependencyProperty.Register("FirstDayOfWeek", typeof(DayOfWeek), typeof(LUCalendarDatePicker), new PropertyMetadata(DayOfWeek.Monday));

        private LUCalendarDatePickerDaysView _pickerDaysView;
        private LUButton _viewButton, _preBtn, _nextBtn;
        private LUCalendarDatePickerMonthsView _pickerMonthsView;
        private LUCalendarDatePickerYearsView _pickerYearsView;

        private System.DateTime _visibleDate = System.DateTime.Now, _visibleYearDate = System.DateTime.Now;
        private LUCalendarDatePickerDateType _currentDisplayType = LUCalendarDatePickerDateType.Day;
        private bool _isAnimating = false;
        static LUCalendarDatePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LUCalendarDatePicker), new FrameworkPropertyMetadata(typeof(LUCalendarDatePicker)));

        }
        public LUCalendarDatePicker()
        {
            CommandBindings.Add(new CommandBinding(LUCalendarDatePickerCommands.SelectDayCommand, OnSelectDay));
            CommandBindings.Add(new CommandBinding(LUCalendarDatePickerCommands.SelectMonthCommand, OnSelectMonth));
            CommandBindings.Add(new CommandBinding(LUCalendarDatePickerCommands.SelectYearsCommand, OnSelectYear));
        }

        private void OnSelectYear(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = (System.DateTime)e.Parameter;
            //ShowMonthsView(selected);
            //_pickerYearsView.Visibility = Visibility.Collapsed;
            _pickerMonthsView.Visibility = Visibility.Visible;
            _pickerMonthsView.Date = selected;
            _pickerMonthsView.Init();
            _currentDisplayType = LUCalendarDatePickerDateType.Month;
            YearsToMonthsViewAnimation();
            UpdateButtonDateText(selected);
        }

        private void OnSelectMonth(object sender, ExecutedRoutedEventArgs e)
        {
            var selected = (System.DateTime)e.Parameter;
            _pickerDaysView.Visibility = Visibility.Visible;
            //_pickerMonthsView.Visibility = Visibility.Collapsed;
            _pickerDaysView.Date = selected;
            _pickerDaysView.Init();
            _currentDisplayType = LUCalendarDatePickerDateType.Day;
            //ShowDaysView(selected);
            MonthsToDaysViewAnimation();
            UpdateButtonDateText(selected);
        }

        private void OnSelectDay(object sender, ExecutedRoutedEventArgs e)
        {
            var oldValue = SelectedDate;
            SelectedDate = (System.DateTime)e.Parameter;
            _pickerDaysView.SelectedDate = SelectedDate;
            OnSelectedDateChanged?.Invoke(this, new DependencyPropertyChangedEventArgs(SelectedDateProperty, oldValue, SelectedDate));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _pickerDaysView = GetTemplateChild("PART_PickerDaysView") as LUCalendarDatePickerDaysView;
            _viewButton = GetTemplateChild("PART_ViewButton") as LUButton;
            _preBtn = GetTemplateChild("PART_PreButton") as LUButton;
            _nextBtn = GetTemplateChild("PART_NextButton") as LUButton;
            _pickerMonthsView = GetTemplateChild("PART_PickerMonthsView") as LUCalendarDatePickerMonthsView;
            _pickerYearsView = GetTemplateChild("PART_PickerYearsView") as LUCalendarDatePickerYearsView;

            _pickerDaysView.OnVisibleDatePropertyChanged += _pickerDaysView_OnVisibleDatePropertyChanged;
            _pickerMonthsView.OnVisibleDatePropertyChanged += _pickerMonthsView_OnVisibleDatePropertyChanged;

            _preBtn.Click += _preBtn_Click;
            _nextBtn.Click += _nextBtn_Click;
            _viewButton.Click += _viewButton_Click;

            Init();
            UpdateButtonDateText(SelectedDate);
        }

        private void _pickerMonthsView_OnVisibleDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _visibleYearDate = _pickerMonthsView.VisibleDate;

            UpdateButtonDateText(_visibleYearDate);
        }

        private void _viewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDisplayType == LUCalendarDatePickerDateType.Day)
            {
                ShowMonthsView(_visibleDate);
                DaysToMonthsViewAnimation();
                UpdateButtonDateText(_visibleDate);
            }
            else if (_currentDisplayType == LUCalendarDatePickerDateType.Month)
            {
                ShowYearsView(_visibleYearDate);
                MonthsToYearsViewAnimation();
                UpdateButtonDateText(_visibleYearDate);
            }
        }

        private void _nextBtn_Click(object sender, RoutedEventArgs e)
        {
            ToNext();
        }

        private void _preBtn_Click(object sender, RoutedEventArgs e)
        {
            ToPre();
        }

        private void _pickerDaysView_OnVisibleDatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _visibleDate = _pickerDaysView.VisibleDate;

            UpdateButtonDateText(_visibleDate);
        }

        private void Init()
        {
            ShowDaysView(SelectedDate);
        }

        private void ShowDaysView(System.DateTime date_)
        {
            _pickerDaysView.Visibility = Visibility.Visible;
            _pickerMonthsView.Visibility = Visibility.Collapsed;
            _pickerDaysView.Date = date_;
            _pickerDaysView.Init();
            _currentDisplayType = LUCalendarDatePickerDateType.Day;
        }

        private void ShowMonthsView(System.DateTime date_)
        {
            //_pickerDaysView.Visibility = Visibility.Collapsed;
            _pickerYearsView.Visibility = Visibility.Collapsed;
            _pickerMonthsView.Visibility = Visibility.Visible;
            _pickerMonthsView.Date = date_;
            _pickerMonthsView.Init();
            _currentDisplayType = LUCalendarDatePickerDateType.Month;
        }

        private void ShowYearsView(System.DateTime date_)
        {
            _pickerYearsView.Visibility = Visibility.Visible;
            //_pickerMonthsView.Visibility = Visibility.Collapsed;
            _pickerYearsView.Date = date_;
            _pickerYearsView.Init();
            _currentDisplayType = LUCalendarDatePickerDateType.Year;
        }

        private void ToPre()
        {
            if (_currentDisplayType == LUCalendarDatePickerDateType.Day)
            {
                _pickerDaysView.ScrollToPreMonth();
            }
            else if (_currentDisplayType == LUCalendarDatePickerDateType.Month)
            {
                _pickerMonthsView.ScrollToPre();
            }
            else
            {
                _pickerYearsView.ScrollToPre();
            }
        }

        private void ToNext()
        {
            if (_currentDisplayType == LUCalendarDatePickerDateType.Day)
            {
                _pickerDaysView.ScrollToNextMonth();
            }
            else if (_currentDisplayType == LUCalendarDatePickerDateType.Month)
            {
                _pickerMonthsView.ScrollToNext();
            }
            else
            {
                _pickerYearsView.ScrollToNext();
            }
        }


        /// <summary>
        /// 天视图到月视图的动画
        /// </summary>
        private void DaysToMonthsViewAnimation()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var stroyboard = new Storyboard();

            //  隐藏天视图
            var animationDaysViewScaleX = new DoubleAnimation();
            animationDaysViewScaleX.From = 1;
            animationDaysViewScaleX.To = 0.7;
            animationDaysViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationDaysViewScaleY = new DoubleAnimation();
            animationDaysViewScaleY.From = 1;
            animationDaysViewScaleY.To = 0.7;
            animationDaysViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationDaysViewScaleX, _pickerDaysView);
            Storyboard.SetTarget(animationDaysViewScaleY, _pickerDaysView);
            Storyboard.SetTargetProperty(animationDaysViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationDaysViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationDaysViewOpacity = new DoubleAnimation();
            animationDaysViewOpacity.From = 1;
            animationDaysViewOpacity.To = 0;
            animationDaysViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationDaysViewOpacity, _pickerDaysView);
            Storyboard.SetTargetProperty(animationDaysViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationDaysViewScaleX);
            stroyboard.Children.Add(animationDaysViewScaleY);
            stroyboard.Children.Add(animationDaysViewOpacity);


            //  显示月视图
            var animationMonthsViewScaleX = new DoubleAnimation();
            animationMonthsViewScaleX.From = 1.5;
            animationMonthsViewScaleX.To = 1;
            animationMonthsViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationMonthsViewScaleY = new DoubleAnimation();
            animationMonthsViewScaleY.From = 1.5;
            animationMonthsViewScaleY.To = 1;
            animationMonthsViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewScaleX, _pickerMonthsView);
            Storyboard.SetTarget(animationMonthsViewScaleY, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationMonthsViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationMonthsViewOpacity = new DoubleAnimation();
            animationMonthsViewOpacity.From = 0;
            animationMonthsViewOpacity.To = 1;
            animationMonthsViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewOpacity, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationMonthsViewScaleX);
            stroyboard.Children.Add(animationMonthsViewScaleY);
            stroyboard.Children.Add(animationMonthsViewOpacity);
            stroyboard.Completed += (s, e) =>
            {
                _pickerDaysView.Visibility = Visibility.Collapsed;
                _isAnimating = false;
            };
            stroyboard.Begin();
        }

        /// <summary>
        /// 月视图到天视图的动画
        /// </summary>
        private void MonthsToDaysViewAnimation()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var stroyboard = new Storyboard();

            //  显示天视图
            var animationDaysViewScaleX = new DoubleAnimation();
            animationDaysViewScaleX.From = 0.7;
            animationDaysViewScaleX.To = 1;
            animationDaysViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationDaysViewScaleY = new DoubleAnimation();
            animationDaysViewScaleY.From = 0.7;
            animationDaysViewScaleY.To = 1;
            animationDaysViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationDaysViewScaleX, _pickerDaysView);
            Storyboard.SetTarget(animationDaysViewScaleY, _pickerDaysView);
            Storyboard.SetTargetProperty(animationDaysViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationDaysViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationDaysViewOpacity = new DoubleAnimation();
            animationDaysViewOpacity.From = 0;
            animationDaysViewOpacity.To = 1;
            animationDaysViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationDaysViewOpacity, _pickerDaysView);
            Storyboard.SetTargetProperty(animationDaysViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationDaysViewScaleX);
            stroyboard.Children.Add(animationDaysViewScaleY);
            stroyboard.Children.Add(animationDaysViewOpacity);


            //  隐藏月视图
            var animationMonthsViewScaleX = new DoubleAnimation();
            animationMonthsViewScaleX.From = 1;
            animationMonthsViewScaleX.To = 1.5;
            animationMonthsViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationMonthsViewScaleY = new DoubleAnimation();
            animationMonthsViewScaleY.From = 1;
            animationMonthsViewScaleY.To = 1.5;
            animationMonthsViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewScaleX, _pickerMonthsView);
            Storyboard.SetTarget(animationMonthsViewScaleY, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationMonthsViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationMonthsViewOpacity = new DoubleAnimation();
            animationMonthsViewOpacity.From = 1;
            animationMonthsViewOpacity.To = 0;
            animationMonthsViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewOpacity, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationMonthsViewScaleX);
            stroyboard.Children.Add(animationMonthsViewScaleY);
            stroyboard.Children.Add(animationMonthsViewOpacity);
            stroyboard.Completed += (s, e) =>
            {
                _pickerMonthsView.Visibility = Visibility.Collapsed;
                _isAnimating = false;
            };
            stroyboard.Begin();
        }

        /// <summary>
        /// 月视图到年视图的动画
        /// </summary>
        private void MonthsToYearsViewAnimation()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var stroyboard = new Storyboard();

            //  隐藏月视图
            var animationMonthsViewScaleX = new DoubleAnimation();
            animationMonthsViewScaleX.From = 1;
            animationMonthsViewScaleX.To = 0.7;
            animationMonthsViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationMonthsViewScaleY = new DoubleAnimation();
            animationMonthsViewScaleY.From = 1;
            animationMonthsViewScaleY.To = 0.7;
            animationMonthsViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewScaleX, _pickerMonthsView);
            Storyboard.SetTarget(animationMonthsViewScaleY, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationMonthsViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationMonthsViewOpacity = new DoubleAnimation();
            animationMonthsViewOpacity.From = 1;
            animationMonthsViewOpacity.To = 0;
            animationMonthsViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewOpacity, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationMonthsViewScaleX);
            stroyboard.Children.Add(animationMonthsViewScaleY);
            stroyboard.Children.Add(animationMonthsViewOpacity);


            //  显示年视图
            var animationYearsViewScaleX = new DoubleAnimation();
            animationYearsViewScaleX.From = 1.5;
            animationYearsViewScaleX.To = 1;
            animationYearsViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationYearsViewScaleY = new DoubleAnimation();
            animationYearsViewScaleY.From = 1.5;
            animationYearsViewScaleY.To = 1;
            animationYearsViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationYearsViewScaleX, _pickerYearsView);
            Storyboard.SetTarget(animationYearsViewScaleY, _pickerYearsView);
            Storyboard.SetTargetProperty(animationYearsViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationYearsViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationYearsViewOpacity = new DoubleAnimation();
            animationYearsViewOpacity.From = 0;
            animationYearsViewOpacity.To = 1;
            animationYearsViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationYearsViewOpacity, _pickerYearsView);
            Storyboard.SetTargetProperty(animationYearsViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationYearsViewScaleX);
            stroyboard.Children.Add(animationYearsViewScaleY);
            stroyboard.Children.Add(animationYearsViewOpacity);
            stroyboard.Completed += (s, e) =>
            {
                _pickerMonthsView.Visibility = Visibility.Collapsed;
                _isAnimating = false;
            };
            stroyboard.Begin();
        }

        /// <summary>
        /// 年视图到月视图的动画
        /// </summary>
        private void YearsToMonthsViewAnimation()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            var stroyboard = new Storyboard();

            //  显示月视图
            var animationMonthsViewScaleX = new DoubleAnimation();
            animationMonthsViewScaleX.From = 0.7;
            animationMonthsViewScaleX.To = 1;
            animationMonthsViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationMonthsViewScaleY = new DoubleAnimation();
            animationMonthsViewScaleY.From = 0.7;
            animationMonthsViewScaleY.To = 1;
            animationMonthsViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewScaleX, _pickerMonthsView);
            Storyboard.SetTarget(animationMonthsViewScaleY, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationMonthsViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationMonthsViewOpacity = new DoubleAnimation();
            animationMonthsViewOpacity.From = 0;
            animationMonthsViewOpacity.To = 1;
            animationMonthsViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationMonthsViewOpacity, _pickerMonthsView);
            Storyboard.SetTargetProperty(animationMonthsViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationMonthsViewScaleX);
            stroyboard.Children.Add(animationMonthsViewScaleY);
            stroyboard.Children.Add(animationMonthsViewOpacity);


            //  隐藏年视图
            var animationYearsViewScaleX = new DoubleAnimation();
            animationYearsViewScaleX.From = 1;
            animationYearsViewScaleX.To = 1.5;
            animationYearsViewScaleX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            var animationYearsViewScaleY = new DoubleAnimation();
            animationYearsViewScaleY.From = 1;
            animationYearsViewScaleY.To = 1.5;
            animationYearsViewScaleY.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationYearsViewScaleX, _pickerYearsView);
            Storyboard.SetTarget(animationYearsViewScaleY, _pickerYearsView);
            Storyboard.SetTargetProperty(animationYearsViewScaleX, new PropertyPath("RenderTransform.ScaleX"));
            Storyboard.SetTargetProperty(animationYearsViewScaleY, new PropertyPath("RenderTransform.ScaleY"));

            var animationYearsViewOpacity = new DoubleAnimation();
            animationYearsViewOpacity.From = 1;
            animationYearsViewOpacity.To = 0;
            animationYearsViewOpacity.Duration = new Duration(TimeSpan.FromMilliseconds(200));

            Storyboard.SetTarget(animationYearsViewOpacity, _pickerYearsView);
            Storyboard.SetTargetProperty(animationYearsViewOpacity, new PropertyPath("Opacity"));

            stroyboard.Children.Add(animationYearsViewScaleX);
            stroyboard.Children.Add(animationYearsViewScaleY);
            stroyboard.Children.Add(animationYearsViewOpacity);
            stroyboard.Completed += (s, e) =>
            {
                _pickerYearsView.Visibility = Visibility.Collapsed;
                _isAnimating = false;
            };
            stroyboard.Begin();
        }

        private void UpdateButtonDateText(System.DateTime date_)
        {
            var lang = LanguageHelper.CurrentLanguage;
            var monthStr = LanguageHelper.GetMonthStr(date_.Month);
            if (_currentDisplayType == LUCalendarDatePickerDateType.Day)
            {
                _viewButton.Content = $"{date_.Year}年{monthStr}";
                if (lang == Lang.ENUS)
                {
                    _viewButton.Content = $"{monthStr} {date_.Year}";
                }
            }
            else
            {
                _viewButton.Content = date_.Year.ToString();
                if (lang == Lang.ZHCN)
                {
                    _viewButton.Content = date_.Year.ToString() + LanguageHelper.GetStr("Lang_Year");
                }
            }
        }
    }
}
