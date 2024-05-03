using LancerUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LancerUI.Controls.Navigation
{
    public class Navigation : ItemsControl
    {
        public struct NavigationInfo
        {
            public double Y;
            public double Height;
            public NavigationItem Control;
        }

        private Border _activeBlock;
        private Rectangle _activeFillBlock;
        private Storyboard _storyboard;
        private bool _isAnimating = false;

        private DoubleAnimation _scrollAnimation;
        private DoubleAnimation _stretchAnimation;
        private NavigationItem _lastSelectedItem;
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(Navigation), new PropertyMetadata(0, new PropertyChangedCallback(OnSelectedIndexChanged)));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Navigation;
            control.ScrollActiveBlockToCurrentIndex();
        }

        public Navigation()
        {
            DefaultStyleKey = typeof(Navigation);

            CommandBindings.Add(new CommandBinding(NavigationCommands.ClickCommand, OnNavigationItemClick));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _activeBlock = GetTemplateChild("ActiveBlock") as Border;
            _activeFillBlock = GetTemplateChild("ActiveFillBlock") as Rectangle;
            var tfg = new TransformGroup();
            tfg.Children.Add(new TranslateTransform() { X = 0, Y = 0 });
            tfg.Children.Add(new ScaleTransform() { ScaleX = 1, ScaleY = 1 });
            _activeBlock.RenderTransform = tfg;
            _activeFillBlock.RenderTransform = new TranslateTransform() { X = 0, Y = 0 };

            CreateAnimations();

            Loaded += Navigation_Loaded;
        }

        private void Navigation_Loaded(object sender, RoutedEventArgs e)
        {
            SetActiveBlcokDefaultIndex();
        }

        #region 获取SelectedIndex目标信息
        /// <summary>
        /// 获取SelectedIndex目标信息
        /// </summary>
        /// <returns></returns>
        private NavigationInfo GetSelectedIndexItemInfo()
        {
            SelectedIndex = SelectedIndex > Items.Count - 1 ? Items.Count - 1 : SelectedIndex;

            double itemActualHeight = 0;
            double itemY = 0;
            NavigationItem control = null;
            if (Items.Count > 0)
            {
                if (ItemsSource == null)
                {
                    var item = (Items[SelectedIndex] as NavigationItem);
                    Point relativePoint = item.TransformToAncestor(this).Transform(new Point(0, 0));
                    itemY = relativePoint.Y;
                    itemActualHeight = item.ActualHeight;
                    control = item;
                }
                else
                {
                    var item = ItemContainerGenerator.ContainerFromItem(Items[SelectedIndex]) as ContentPresenter;
                    Point relativePoint = item.TransformToAncestor(this).Transform(new Point(0, 0));
                    itemY = relativePoint.Y;
                    itemActualHeight = item.ActualHeight;
                    control = VisualTreeHelper.GetChild(item, 0) as NavigationItem;
                }
            }

            return new NavigationInfo
            {
                Y = itemY,
                Height = itemActualHeight,
                Control = control
            };
        }
        #endregion

        #region 设定标记块的初始位置
        /// <summary>
        /// 设定标记块的初始位置
        /// </summary>
        private void SetActiveBlcokDefaultIndex()
        {
            var itemInfo = GetSelectedIndexItemInfo();
            //  设定初始坐标
            double defaultY = itemInfo.Y + (itemInfo.Height / 2) - _activeBlock.ActualHeight / 2;

            if (_activeBlock != null)
            {
                var tfg = new TransformGroup();
                tfg.Children.Add(new TranslateTransform() { X = 0, Y = defaultY });
                tfg.Children.Add(new ScaleTransform() { ScaleX = 1, ScaleY = 1 });
                _activeBlock.RenderTransform = tfg;
            }
        }
        #endregion

        #region 创建动画
        private void CreateAnimations()
        {

            _storyboard = new Storyboard();
            _scrollAnimation = new DoubleAnimation();
            _stretchAnimation = new DoubleAnimation();

            //  滚动动画
            _scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            _scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.25));

            Storyboard.SetTarget(_scrollAnimation, _activeBlock);
            Storyboard.SetTargetProperty(_scrollAnimation, new PropertyPath("RenderTransform.Children[0].Y"));

            //  伸缩动画
            _stretchAnimation.AutoReverse = true;
            _stretchAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            _stretchAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.25));


            Storyboard.SetTarget(_stretchAnimation, _activeFillBlock);
            Storyboard.SetTargetProperty(_stretchAnimation, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));


            _storyboard.Children.Add(_scrollAnimation);
            _storyboard.Children.Add(_stretchAnimation);

            _storyboard.Completed += (e, c) =>
            {
                _isAnimating = false;
            };
        }
        #endregion

        private void OnNavigationItemClick(object sender, ExecutedRoutedEventArgs e)
        {
            //  如果动画未结束不允许点击
            if (_isAnimating) return;
            //  获取点击项的索引
            var navigationItem = e.OriginalSource as NavigationItem;

            if (ItemsSource == null)
            {
                //  从UI中获取索引
                SelectedIndex = Items.IndexOf(navigationItem);
            }
            else
            {
                //  从绑定中获取索引
                SelectedIndex = Items.IndexOf(e.Parameter);
            }
        }

        #region 滚动标记块到当前选中位置
        private void ScrollActiveBlockToCurrentIndex()
        {
            if (!IsLoaded) return;

            var itemInfo = GetSelectedIndexItemInfo();

            //  更新子项选中状态
            itemInfo.Control.IsSelected = true;
            if (_lastSelectedItem != null)
            {
                _lastSelectedItem.IsSelected = false;
            }
            _lastSelectedItem = itemInfo.Control;


            var activeBlockTTF = (_activeBlock.RenderTransform as TransformGroup).Children[0] as TranslateTransform;
            _scrollAnimation.To = itemInfo.Y + (itemInfo.Height / 2) - _activeBlock.ActualHeight / 2;


            if (_scrollAnimation.To == activeBlockTTF.Y)
            {
                return;
            }

            //  计算拉伸长度
            double weight = (double)Math.Abs((decimal)activeBlockTTF.Y - (decimal)_scrollAnimation.To) / itemInfo.Height / 10;

            double stretchHeight = 10 + _activeFillBlock.ActualHeight * weight;
            stretchHeight = stretchHeight > _activeBlock.ActualHeight ? _activeBlock.ActualHeight : stretchHeight;
            
            _stretchAnimation.To = stretchHeight;
            if (itemInfo.Y > activeBlockTTF.Y)
            {
                _stretchAnimation.To = -stretchHeight;
                //  向下移动   
            }

            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new TranslateTransform(0, activeBlockTTF.Y));
            transformGroup.Children.Add(new ScaleTransform(1, 1, 0.5, 0.5));
            _activeBlock.RenderTransform = transformGroup;
            _storyboard.Begin();
            _isAnimating = true;
        }
        #endregion
    }
}
