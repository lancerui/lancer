using LancerUI.Controls.Types;
using LancerUI.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace LancerUI.Controls.Buttons
{
    public class LUConfirmButton : LUButton
    {
        /// <summary>
        /// 二次确认使用的图标
        /// </summary>
        public IconSymbol ConfirmIcon
        {
            get => (IconSymbol)GetValue(ConfirmIconProperty);
            set => SetValue(ConfirmIconProperty, value);
        }
        public static readonly System.Windows.DependencyProperty ConfirmIconProperty = System.Windows.DependencyProperty.Register("ConfirmIcon", typeof(IconSymbol), typeof(LUConfirmButton), new System.Windows.PropertyMetadata(IconSymbol.None));
        public Position ConfirmIconPosition
        {
            get => (Position)GetValue(ConfirmIconPositionProperty);
            set => SetValue(ConfirmIconPositionProperty, value);
        }
        public static readonly System.Windows.DependencyProperty ConfirmIconPositionProperty = System.Windows.DependencyProperty.Register("ConfirmIconPosition", typeof(Position), typeof(LUConfirmButton), new System.Windows.PropertyMetadata(Position.Left));
        /// <summary>
        /// 二次确认使用的样式（默认危险）
        /// </summary>
        public ButtonAppearanceType ConfirmAppearance
        {
            get => (ButtonAppearanceType)GetValue(ConfirmAppearanceProperty);
            set => SetValue(ConfirmAppearanceProperty, value);
        }
        public static readonly System.Windows.DependencyProperty ConfirmAppearanceProperty = System.Windows.DependencyProperty.Register("ConfirmAppearance", typeof(ButtonAppearanceType), typeof(LUConfirmButton), new System.Windows.PropertyMetadata(ButtonAppearanceType.Danger));
        /// <summary>
        /// 二次确认内容
        /// </summary>
        public object ConfirmContent
        {
            get => GetValue(ConfirmContentProperty);
            set => SetValue(ConfirmContentProperty, value);
        }
        public static readonly System.Windows.DependencyProperty ConfirmContentProperty = System.Windows.DependencyProperty.Register("ConfirmContent", typeof(object), typeof(LUConfirmButton), new System.Windows.PropertyMetadata(null));
        public bool IsDisplayConfirm
        {
            get => (bool)GetValue(IsDisplayConfirmProperty);
            set => SetValue(IsDisplayConfirmProperty, value);
        }
        public static readonly System.Windows.DependencyProperty IsDisplayConfirmProperty = System.Windows.DependencyProperty.Register("IsDisplayConfirm", typeof(bool), typeof(LUConfirmButton), new System.Windows.PropertyMetadata(false, new PropertyChangedCallback(OnIsDisplayConfirmChanged)));

        private static void OnIsDisplayConfirmChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUConfirmButton;
            if (control.IsDisplayConfirm)
            {
                control.ShowConfirm();
            }
            else
            {
                control.HideConfirm();
            }
        }
        /// <summary>
        /// 是否启用动画效果
        /// </summary>
        public bool IsAnimation
        {
            get => (bool)GetValue(IsAnimationProperty);
            set => SetValue(IsAnimationProperty, value);
        }
        public static readonly System.Windows.DependencyProperty IsAnimationProperty = System.Windows.DependencyProperty.Register("IsAnimation", typeof(bool), typeof(LUConfirmButton), new System.Windows.PropertyMetadata(true));
        /// <summary>
        /// 确认超时时间（毫秒）
        /// </summary>
        public double ConfirmTimeout
        {
            get => (double)GetValue(ConfirmTimeoutProperty);
            set => SetValue(ConfirmTimeoutProperty, value);
        }
        public static readonly System.Windows.DependencyProperty ConfirmTimeoutProperty = System.Windows.DependencyProperty.Register("ConfirmTimeout", typeof(double), typeof(LUConfirmButton), new System.Windows.PropertyMetadata(3000.0));

        private LUConfirmButtonModel _model;
        private LUTimer _timer;
        private LUButton _baseButton;
        private LUButton _confirmBtn;

        private Storyboard _storyboardDisplay;
        private Storyboard _storyboardHide;

        public LUConfirmButton()
        {
            DefaultStyleKey = typeof(LUConfirmButton);
            Loaded += LUConfirmButton_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _baseButton = GetTemplateChild("BaseButton") as LUButton;
            _confirmBtn = GetTemplateChild("ConfirmBtn") as LUButton;
            _baseButton.Click += BaseButton_Click;
        }

        private void Init()
        {
            _model = new LUConfirmButtonModel();
            var binding = new Binding()
            {
                Source = _model,
                Path = new System.Windows.PropertyPath(nameof(_model.IsDisplayConfirm)),
                Mode = BindingMode.TwoWay
            };
            SetBinding(IsDisplayConfirmProperty, binding);
            _timer = new LUTimer(ConfirmTimeout);
            _timer.OnTimeOut += _timer_OnTimeOut;

            Click += LUConfirmButton_Click;
        }

        private void CreateAnimations()
        {
            _baseButton.RenderTransform = new TranslateTransform()
            {
                Y = 0
            };
            _confirmBtn.RenderTransform = new TranslateTransform()
            {
                Y = -_confirmBtn.ActualHeight
            };

            _storyboardDisplay = new Storyboard();
            _storyboardHide = new Storyboard();

            var easeIn = new BackEase() { EasingMode = EasingMode.EaseIn };
            var easeOut = new BackEase() { EasingMode = EasingMode.EaseOut };

            //  显示确认按钮动画
            var _displayAnimation1 = new DoubleAnimation()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = easeIn
            };
            var _displayAnimation2 = new DoubleAnimation()
            {
                To = _baseButton.ActualHeight,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = easeIn
            };

            Storyboard.SetTarget(_displayAnimation1, _confirmBtn);
            Storyboard.SetTargetProperty(_displayAnimation1, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));
            Storyboard.SetTarget(_displayAnimation2, _baseButton);
            Storyboard.SetTargetProperty(_displayAnimation2, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

            _storyboardDisplay.Children.Add(_displayAnimation1);
            _storyboardDisplay.Children.Add(_displayAnimation2);

            //  隐藏确认按钮动画
            var _hideAnimation1 = new DoubleAnimation()
            {
                To = -_confirmBtn.ActualHeight,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = easeOut
            };
            var _hideAnimation2 = new DoubleAnimation()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(300)),
                EasingFunction = easeOut
            };

            Storyboard.SetTarget(_hideAnimation1, _confirmBtn);
            Storyboard.SetTargetProperty(_hideAnimation1, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));
            Storyboard.SetTarget(_hideAnimation2, _baseButton);
            Storyboard.SetTargetProperty(_hideAnimation2, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

            _storyboardHide.Children.Add(_hideAnimation1);
            _storyboardHide.Children.Add(_hideAnimation2);
        }

        private void ShowConfirm()
        {
            if (IsAnimation)
            {
                _storyboardDisplay.Begin();
            }
            else
            {
                var ttfBaseBtn = _baseButton.RenderTransform as TranslateTransform;
                ttfBaseBtn.Y = _baseButton.ActualHeight;
                var ttfConfirmBtn = _confirmBtn.RenderTransform as TranslateTransform;
                ttfConfirmBtn.Y = 0;
            }
        }
        private void HideConfirm()
        {
            if (IsAnimation)
            {
                _storyboardHide.Begin();
            }
            else
            {
                var ttfBaseBtn = _baseButton.RenderTransform as TranslateTransform;
                ttfBaseBtn.Y = 0;
                var ttfConfirmBtn = _confirmBtn.RenderTransform as TranslateTransform;
                ttfConfirmBtn.Y = -_confirmBtn.ActualHeight;
            }
        }

        private void BaseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            IsDisplayConfirm = true;
            _timer.Start();
        }

        private void LUConfirmButton_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Init();
            CreateAnimations();
        }


        private void LUConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            IsDisplayConfirm = false;
            Command?.Execute(CommandParameter);
        }

        private void _timer_OnTimeOut(object? sender, EventArgs e)
        {
            _model.IsDisplayConfirm = false;
        }
    }
}
