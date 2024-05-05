using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LancerUI.Controls.SelectorBar
{
    public class LUSelectorBar : ItemsControl
    {
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(LUSelectorBar), new PropertyMetadata(0, OnSelectedIndexChanged));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var control = d as LUSelectorBar;
                control.ScrollToSelectedPoint();
            }
        }
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(LUSelectorBar), new PropertyMetadata(null));
        /// <summary>
        /// 是否启用动画效果
        /// </summary>
        public bool IsAnimation { get => (bool)GetValue(IsAnimationProperty); set => SetValue(IsAnimationProperty, value); }
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(LUSelectorBar), new PropertyMetadata(true));
        private bool IsBinding { get => ItemsSource != null; }
        private Rectangle _mark;

        //  动画
        private Storyboard _storyboard;
        private DoubleAnimation _scrollAnimation;
        private DoubleAnimation _stretchAnimation;
        public LUSelectorBar()
        {
            DefaultStyleKey = typeof(LUSelectorBar);
        }




        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _mark = GetTemplateChild("Mark") as Rectangle;
            Loaded += LUSelectorBar_Loaded;

            CreateAnimations();
        }
        private void LUSelectorBar_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= LUSelectorBar_Loaded;
            Init();
        }

        private void CreateAnimations()
        {
            _storyboard = new Storyboard();
            _scrollAnimation = new DoubleAnimation();
            _stretchAnimation = new DoubleAnimation();

            //  滚动动画
            _scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut };
            _scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.12));

            Storyboard.SetTarget(_scrollAnimation, _mark);
            Storyboard.SetTargetProperty(_scrollAnimation, new PropertyPath("RenderTransform.Children[0].X"));

            //  伸缩动画
            _stretchAnimation.AutoReverse = true;
            //_stretchAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            _stretchAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.12));
            _stretchAnimation.To = 1.3;

            Storyboard.SetTarget(_stretchAnimation, _mark);
            Storyboard.SetTargetProperty(_stretchAnimation, new PropertyPath("RenderTransform.Children[1].ScaleX"));

            _storyboard.Children.Add(_scrollAnimation);
            //_storyboard.Children.Add(_stretchAnimation);
        }

        private double GetSelectedPosition()
        {
            if (Items.Count == 0 || _mark == null) return -1;

            if (IsBinding)
            {
                //  绑定模式
                ContentPresenter cp = null;
                if (SelectedItem == null)
                {
                    cp = ItemContainerGenerator.ContainerFromItem(Items[0]) as ContentPresenter;
                }
                else
                {
                    cp = ItemContainerGenerator.ContainerFromItem(SelectedItem) as ContentPresenter;
                }

                if (cp == null) return -1;
                Point p = cp.TransformToAncestor(this).Transform(new Point(0, 0));
                double x = p.X + cp.ActualWidth / 2 - _mark.ActualWidth / 2;
                return x;
            }
            else
            {
                //  非绑定模式
                int index = 0;
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                {
                    index = SelectedIndex;
                }
                var selectedItem = Items[index] as LUSelectorBarItem;
                Point p = selectedItem.TransformToAncestor(this).Transform(new Point(0, 0));
                double margin = selectedItem.Margin.Right > 0 ? selectedItem.Margin.Right / 2 : 0;
                double x = p.X + selectedItem.ActualWidth / 2 - _mark.ActualWidth / 2 - margin;
                return x;
            }
        }

        private void Init()
        {
            double defaultItemX = GetSelectedPosition();
            if (defaultItemX == -1) return;

            var tfg = new TransformGroup();
            tfg.Children.Add(new TranslateTransform() { X = defaultItemX, Y = 0 });
            tfg.Children.Add(new ScaleTransform() { ScaleX = 1, ScaleY = 1 });
            _mark.RenderTransform = tfg;
        }

        private void ScrollToSelectedPoint()
        {
            if (_mark == null) return;

            var tfg = _mark.RenderTransform as TransformGroup;
            var ttf = tfg.Children[0] as TranslateTransform;
            double toX = GetSelectedPosition();
            if (toX == -1) return;

            if (IsAnimation)
            {

                _storyboard.Stop();
                _scrollAnimation.To = toX;
                _mark.RenderTransformOrigin = new Point(ttf.X > toX ? 0 : 1, 0);
                tfg.Children[1] = new ScaleTransform() { ScaleX = 1 };
                _storyboard.Begin();
            }
            else
            {

                ttf.X = toX;
            }
        }

        internal void NotifySelectorBarItemMouseUp(LUSelectorBarItem item)
        {
            if (!IsEnabled) return;

            if (IsBinding)
            {
                if (SelectedItem == item.DataContext) return;
                SelectedItem = item.DataContext;
                SelectedIndex = Items.IndexOf(item.DataContext);
            }
            else
            {
                if (SelectedIndex == Items.IndexOf(item)) return;
                SelectedIndex = Items.IndexOf(item);
                SelectedItem = item;
            }

            ScrollToSelectedPoint();
        }
    }
}
