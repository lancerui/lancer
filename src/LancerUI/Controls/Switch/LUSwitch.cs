using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using LancerUI.Controls.Types;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LancerUI.Controls.Switch
{
    public class LUSwitch : Control
    {
        public event EventHandler Changed;
        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(LUSwitch), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnIsCheckedChanged)));

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUSwitch;
            if (e.NewValue != e.OldValue)
            {
                control.SetThumbPoint();
                control.UpdateSwitchText();
            }
        }
        public Position TextPosition
        {
            get { return (Position)GetValue(TextPositionProperty); }
            set { SetValue(TextPositionProperty, value); }
        }
        public static readonly DependencyProperty TextPositionProperty =
            DependencyProperty.Register("TextPosition", typeof(Position), typeof(LUSwitch), new PropertyMetadata(Position.Right));
        public string OnText
        {
            get { return (string)GetValue(OnTextProperty); }
            set { SetValue(OnTextProperty, value); }
        }
        public static readonly DependencyProperty OnTextProperty =
            DependencyProperty.Register("OnText", typeof(string), typeof(LUSwitch), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnStateTextChanged)));
        public string OffText
        {
            get { return (string)GetValue(OffTextProperty); }
            set { SetValue(OffTextProperty, value); }
        }
        public static readonly DependencyProperty OffTextProperty =
            DependencyProperty.Register("OffText", typeof(string), typeof(LUSwitch), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnStateTextChanged)));



        public double SwitchWitdh { get => (double)GetValue(SwitchWitdhProperty); set => SetValue(SwitchWitdhProperty, value); }
        public static readonly DependencyProperty SwitchWitdhProperty =
            DependencyProperty.Register("SwitchWitdh", typeof(double), typeof(LUSwitch), new PropertyMetadata(40.0));
        public double SwitchHeight { get => (double)GetValue(SwitchHeightProperty); set => SetValue(SwitchHeightProperty, value); }
        public static readonly DependencyProperty SwitchHeightProperty =
            DependencyProperty.Register("SwitchHeight", typeof(double), typeof(LUSwitch), new PropertyMetadata(20.0));
        public double ThumbSize { get => (double)GetValue(ThumbSizeProperty); set => SetValue(ThumbSizeProperty, value); }
        public static readonly DependencyProperty ThumbSizeProperty =
            DependencyProperty.Register("ThumbSize", typeof(double), typeof(LUSwitch), new PropertyMetadata(12.0));
        public CornerRadius ThumbRadius { get => (CornerRadius)GetValue(ThumbRadiusProperty); set => SetValue(ThumbRadiusProperty, value); }
        public static readonly DependencyProperty ThumbRadiusProperty =
            DependencyProperty.Register("ThumbRadius", typeof(CornerRadius), typeof(LUSwitch), new PropertyMetadata(new CornerRadius(12)));
        public CornerRadius SwitchRadius { get => (CornerRadius)GetValue(SwitchRadiusProperty); set => SetValue(SwitchRadiusProperty, value); }
        public static readonly DependencyProperty SwitchRadiusProperty =
            DependencyProperty.Register("SwitchRadius", typeof(CornerRadius), typeof(LUSwitch), new PropertyMetadata(new CornerRadius(10)));
        /// <summary>
        /// 是否启用动画效果
        /// </summary>
        public bool IsAnimation { get => (bool)GetValue(IsAnimationProperty); set => SetValue(IsAnimationProperty, value); }
        public static readonly DependencyProperty IsAnimationProperty =
            DependencyProperty.Register("IsAnimation", typeof(bool), typeof(LUSwitch), new PropertyMetadata(true));
        //  控件
        private Border _switchThumb;
        private TextBlock _switchText;

        //  动画
        private Storyboard _storyboard;
        private DoubleAnimation _scrollAnimation;
        private DoubleAnimation _stretchAnimation;

        //  Thumb的内边距（左右）
        private readonly double _thumbMargin = 5;
        private double _offThumbX = 0;
        private double _onThumbX = 0;

        public LUSwitch()
        {
            DefaultStyleKey = typeof(LUSwitch);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _switchText = GetTemplateChild("SwitchText") as TextBlock;
            _switchThumb = GetTemplateChild("SwitchThumb") as Border;

            //  计算Thumb左右的位置
            CalculateThumbPosition();
            InitSwitch();
            UpdateSwitchText();
            CreateAnimations();
            UpdateSwitchTextVisibility();
        }

        private void InitSwitch()
        {
            _switchThumb.HorizontalAlignment = HorizontalAlignment.Center;
            _switchThumb.VerticalAlignment = VerticalAlignment.Center;
            var tfg = new TransformGroup();
            tfg.Children.Add(new TranslateTransform() { X = IsChecked ? _onThumbX : _offThumbX });
            tfg.Children.Add(new ScaleTransform() { ScaleX = 1 });
            _switchThumb.RenderTransform = tfg;
        }
        private void UpdateSwitchText()
        {
            if (_switchText == null) return;

            _switchText.Text = IsChecked ? OnText : OffText;
        }

        /// <summary>
        /// 计算Thumb的开关状态位置，计算位置调用优先级必须比 void InitSwitch() 高
        /// </summary>
        private void CalculateThumbPosition()
        {
            //  计算Thumb的位置，正值向右，负值向左
            _onThumbX = SwitchWitdh / 2 - ThumbSize / 2 - _thumbMargin;
            _offThumbX = -_onThumbX;
        }
        private void CreateAnimations()
        {
            _storyboard = new Storyboard();
            _scrollAnimation = new DoubleAnimation();
            _stretchAnimation = new DoubleAnimation();

            //  滚动动画
            _scrollAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            _scrollAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));

            Storyboard.SetTarget(_scrollAnimation, _switchThumb);
            Storyboard.SetTargetProperty(_scrollAnimation, new PropertyPath("RenderTransform.Children[0].X"));

            //  伸缩动画
            _stretchAnimation.AutoReverse = true;
            _stretchAnimation.EasingFunction = new SineEase() { EasingMode = EasingMode.EaseIn };
            _stretchAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.15));
            _stretchAnimation.To = 1.3;

            Storyboard.SetTarget(_stretchAnimation, _switchThumb);
            Storyboard.SetTargetProperty(_stretchAnimation, new PropertyPath("RenderTransform.Children[1].ScaleX"));

            _storyboard.Children.Add(_scrollAnimation);
            _storyboard.Children.Add(_stretchAnimation);
        }

        private void SetThumbPoint()
        {
            if (_switchThumb == null) return;

            var tfg = _switchThumb.RenderTransform as TransformGroup;

            if (IsAnimation)
            {
                _storyboard.Stop();
                _scrollAnimation.To = IsChecked ? _onThumbX : _offThumbX;
                _switchThumb.RenderTransformOrigin = new Point(IsChecked ? 1 : 0, 0);
                tfg.Children[1] = new ScaleTransform() { ScaleX = 1 };
                _storyboard.Begin();
            }
            else
            {
                var ttf = tfg.Children[0] as TranslateTransform;
                ttf.X = IsChecked ? _onThumbX : _offThumbX;
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (!IsEnabled) return;
            IsChecked = !IsChecked;
            UpdateSwitchText();
            SetThumbPoint();
            Changed?.Invoke(this, EventArgs.Empty);
        }
        private static void OnStateTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUSwitch;
            if (e.NewValue != e.OldValue)
            {
                control.UpdateSwitchTextVisibility();
                control.UpdateSwitchText();
            }
        }

        private void UpdateSwitchTextVisibility()
        {
            if (_switchText == null) return;

            if (!string.IsNullOrEmpty(OnText) || !string.IsNullOrEmpty(OffText))
            {
                _switchText.Visibility = Visibility.Visible;
            }
            else
            {
                _switchText.Visibility = Visibility.Collapsed;
            }
        }
    }
}
