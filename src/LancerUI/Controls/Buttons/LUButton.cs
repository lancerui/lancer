using LancerUI.Controls.Types;
using LancerUI.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LancerUI.Controls.Buttons
{
    public class LUButton : ButtonBase
    {
        /// <summary>
        /// 外观
        /// </summary>
        public ButtonAppearanceType Appearance
        {
            get => (ButtonAppearanceType)GetValue(AppearanceProperty);
            set => SetValue(AppearanceProperty, value);
        }
        public static readonly System.Windows.DependencyProperty AppearanceProperty = System.Windows.DependencyProperty.Register("Appearance", typeof(ButtonAppearanceType), typeof(LUButton), new System.Windows.PropertyMetadata(ButtonAppearanceType.Default));
        /// <summary>
        /// 鼠标经过背景笔刷
        /// </summary>
        public SolidColorBrush HoverBackground
        {
            get => (SolidColorBrush)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }
        public static readonly System.Windows.DependencyProperty HoverBackgroundProperty = System.Windows.DependencyProperty.Register("HoverBackground", typeof(SolidColorBrush), typeof(LUButton), new System.Windows.PropertyMetadata(new SolidColorBrush(Color.FromArgb(0, 0, 0, 0))));
        /// <summary>
        /// 图标
        /// </summary>
        public IconSymbol Icon
        {
            get => (IconSymbol)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly System.Windows.DependencyProperty IconProperty = System.Windows.DependencyProperty.Register("Icon", typeof(IconSymbol), typeof(LUButton), new System.Windows.PropertyMetadata(IconSymbol.None));
        /// <summary>
        /// 图标大小
        /// </summary>
        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        public static readonly System.Windows.DependencyProperty IconSizeProperty = System.Windows.DependencyProperty.Register("IconSize", typeof(double), typeof(LUButton), new System.Windows.PropertyMetadata(20.0));
        /// <summary>
        /// 图标位置
        /// </summary>
        public Position IconPosition
        {
            get => (Position)GetValue(IconPositionProperty);
            set => SetValue(IconPositionProperty, value);
        }
        public static readonly System.Windows.DependencyProperty IconPositionProperty = System.Windows.DependencyProperty.Register("IconPosition", typeof(Position), typeof(LUButton), new System.Windows.PropertyMetadata(Position.Left));
        /// <summary>
        /// 是否显示加载中
        /// </summary>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }
        public static readonly System.Windows.DependencyProperty IsLoadingProperty = System.Windows.DependencyProperty.Register("IsLoading", typeof(bool), typeof(LUButton), new System.Windows.PropertyMetadata(false));
        /// <summary>
        /// 加载中提示内容
        /// </summary>
        public string LoadingText
        {
            get => (string)GetValue(LoadingTextProperty);
            set => SetValue(LoadingTextProperty, value);
        }
        public static readonly System.Windows.DependencyProperty LoadingTextProperty = System.Windows.DependencyProperty.Register("LoadingText", typeof(string), typeof(LUButton), new System.Windows.PropertyMetadata(""));
        /// <summary>
        /// 打开链接
        /// </summary>
        public string OpenUrl
        {
            get => (string)GetValue(OpenUrlProperty);
            set => SetValue(OpenUrlProperty, value);
        }
        public static readonly System.Windows.DependencyProperty OpenUrlProperty = System.Windows.DependencyProperty.Register("OpenUrl", typeof(string), typeof(LUButton), new System.Windows.PropertyMetadata(""));

        public LUButton()
        {
            DefaultStyleKey = typeof(LUButton);
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (!string.IsNullOrEmpty(OpenUrl))
            {
                Process.Start(new ProcessStartInfo(OpenUrl) { UseShellExecute = true });
            }
        }
    }
}
