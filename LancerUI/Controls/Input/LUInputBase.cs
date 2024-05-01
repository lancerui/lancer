using LancerUI.Controls.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LancerUI.Controls.Input
{
    public class LUInputBase : TextBox
    {
        /// <summary>
        /// 输入框高度
        /// </summary>
        public double InputHeight { get => (double)GetValue(InputHeightProperty); set => SetValue(InputHeightProperty, value); }
        public static readonly DependencyProperty InputHeightProperty = DependencyProperty.Register("InputHeight", typeof(double), typeof(LUInputBase), new PropertyMetadata(33.0));
        /// <summary>
        /// 圆角半径
        /// </summary>
        public CornerRadius CornerRadius { get => (CornerRadius)GetValue(CornerRadiusProperty); set => SetValue(CornerRadiusProperty, value); }
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(LUInputBase));
        /// <summary>
        /// 输入框占位文本
        /// </summary>
        public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
        public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(LUInputBase));
        /// <summary>
        /// 图标
        /// </summary>
        public IconSymbol Icon { get => (IconSymbol)GetValue(IconProperty); set => SetValue(IconProperty, value); }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(IconSymbol), typeof(LUInputBase), new PropertyMetadata(IconSymbol.None));
        /// <summary>
        /// 图标尺寸
        /// </summary>
        public double IconSize { get => (double)GetValue(IconSizeProperty); set => SetValue(IconSizeProperty, value); }
        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(LUInputBase), new PropertyMetadata(20.0));
        /// <summary>
        /// 图标位置
        /// </summary>
        public Position IconPosition { get => (Position)GetValue(IconPositionProperty); set => SetValue(IconProperty, value); }
        public static readonly DependencyProperty IconPositionProperty = DependencyProperty.Register("IconPosition", typeof(Position), typeof(LUInputBase), new PropertyMetadata(Position.Left));
        /// <summary>
        /// 是否显示清除按钮
        /// </summary>
        public bool IsClearable { get => (bool)GetValue(IsClearableProperty); set => SetValue(IsClearableProperty, value); }
        public static readonly DependencyProperty IsClearableProperty = DependencyProperty.Register("IsClearable", typeof(bool), typeof(LUInputBase), new PropertyMetadata(true));
        /// <summary>
        /// 输入框前置内容
        /// </summary>
        public object ContentBefore { get => (object)GetValue(ContentBeforeProperty); set => SetValue(ContentBeforeProperty, value); }
        public static readonly DependencyProperty ContentBeforeProperty = DependencyProperty.Register("ContentBefore", typeof(object), typeof(LUInputBase));
        /// <summary>
        /// 输入框后置内容
        /// </summary>
        public object ContentAfter { get => (object)GetValue(ContentAfterProperty); set => SetValue(ContentAfterProperty, value); }
        public static readonly DependencyProperty ContentAfterProperty = DependencyProperty.Register("ContentAfter", typeof(object), typeof(LUInputBase));
        /// <summary>
        /// 是否显示弹出层
        /// </summary>
        public bool IsOpenPopup { get => (bool)GetValue(IsOpenPopupProperty); set => SetValue(IsOpenPopupProperty, value); }
        public static readonly DependencyProperty IsOpenPopupProperty = DependencyProperty.Register("IsOpenPopup", typeof(bool), typeof(LUInputBase), new PropertyMetadata(false));
        /// <summary>
        /// 是否在获得焦点时显示弹出层
        /// </summary>
        public bool IsFocusedPopup { get => (bool)GetValue(IsFocusedPopupProperty); set => SetValue(IsFocusedPopupProperty, value); }
        public static readonly DependencyProperty IsFocusedPopupProperty = DependencyProperty.Register("IsFocusedPopup", typeof(bool), typeof(LUInputBase), new PropertyMetadata(false));
        /// <summary>
        /// 弹出层内容
        /// </summary>
        public object PopupContent { get => (object)GetValue(PopupContentProperty); set => SetValue(PopupContentProperty, value); }
        public static readonly DependencyProperty PopupContentProperty = DependencyProperty.Register("PopupContent", typeof(object), typeof(LUInputBase));
        public bool IsInvalid { get => (bool)GetValue(IsInvalidProperty); set => SetValue(IsInvalidProperty, value); }
        public static readonly DependencyProperty IsInvalidProperty = DependencyProperty.Register("IsInvalid", typeof(bool), typeof(LUInputBase));
        public string InvalidMessage { get => (string)GetValue(InvalidMessageProperty); set => SetValue(InvalidMessageProperty, value); }
        public static readonly DependencyProperty InvalidMessageProperty = DependencyProperty.Register("InvalidMessage", typeof(string), typeof(LUInputBase));

        public LUInputBase()
        {
            DefaultStyleKey = typeof(LUInputBase);
        }
    }
}
