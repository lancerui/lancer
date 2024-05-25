using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LancerUI.Controls.Base
{
    /// <summary>
    /// 条件显示视图，通过设定 Condition 属性判断值是否满足，满足时显示内容，否则不渲染
    /// </summary>
    public class LUView : ContentControl
    {
        /// <summary>
        /// 值1
        /// </summary>
        public object Value1 { get => (object)GetValue(Value1Property); set => SetValue(Value1Property, value); }
        public static DependencyProperty Value1Property = DependencyProperty.Register(nameof(Value1), typeof(object), typeof(LUView), new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUView;
            if (e.OldValue != e.NewValue)
            {
                control.Check();
            }
        }

        /// <summary>
        /// 值2
        /// </summary>
        public object Value2 { get => (object)GetValue(Value2Property); set => SetValue(Value2Property, value); }
        public static DependencyProperty Value2Property = DependencyProperty.Register(nameof(Value2), typeof(object), typeof(LUView), new PropertyMetadata(null, new PropertyChangedCallback(OnPropertyChanged)));
        /// <summary>
        /// 判定条件
        /// </summary>
        public LUViewCondition Condition { get => (LUViewCondition)GetValue(ConditionProperty); set => SetValue(ConditionProperty, value); }
        public static DependencyProperty ConditionProperty = DependencyProperty.Register(nameof(Condition), typeof(LUViewCondition), typeof(LUView), new PropertyMetadata(LUViewCondition.True, new PropertyChangedCallback(OnPropertyChanged)));
        public LUView()
        {
            DefaultStyleKey = typeof(LUView);
        }

        private void Check()
        {
            Visibility = IsDisplay() ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool IsDisplay()
        {
            switch (Condition)
            {
                case LUViewCondition.True:
                    //  没有设定值时默认显示
                    if (Value1 == null && Value2 == null) return true;
                    if (Value1 != null && Value2 != null)
                    {
                        //  设定了两个值
                        return (bool)Value1 && (bool)Value2;
                    }
                    else if (Value1 != null)
                    {
                        return (bool)Value1;
                    }
                    else if (Value2 != null)
                    {
                        return (bool)Value2;
                    }
                    return true;
                case LUViewCondition.False:
                    //  没有设定值时默认显示
                    if (Value1 == null && Value2 == null) return true;
                    if (Value1 != null && Value2 != null)
                    {
                        //  设定了两个值
                        return !(bool)Value1 && !(bool)Value2;
                    }
                    else if (Value1 != null)
                    {
                        return !(bool)Value1;
                    }
                    else if (Value2 != null)
                    {
                        return !(bool)Value2;
                    }
                    return true;
                case LUViewCondition.Equal:
                    //  没有设定值时默认显示
                    if (Value1 == null && Value2 == null) return true;
                    if (Value1 is bool || Value1 is string || Value1 is double || Value1 is int)
                    {
                        if (Value2 == null) return false;
                        if (Value1 is bool)
                        {
                            return Value1.ToString().ToLower() == Value2.ToString().ToLower();
                        }
                        return Value1.ToString() == Value2.ToString();
                    }
                    return Value1 == Value2;
                case LUViewCondition.NotEmptyList:
                    if (Value1 == null && Value2 == null) return false;
                    if (Value1 is System.Collections.IList)
                    {
                        return (Value1 as System.Collections.IList).Count > 0;
                    }
                    if (Value1 is string[])
                    {
                        return (Value1 as string[]).Length > 0;
                    }
                    if (Value1 is double[])
                    {
                        return (Value1 as double[]).Length > 0;
                    }
                    break;
                case LUViewCondition.EmptyList:
                    if (Value1 == null && Value2 == null) return true;
                    if (Value1 is System.Collections.IList)
                    {
                        return (Value1 as System.Collections.IList).Count == 0;
                    }
                    if (Value1 is string[])
                    {
                        return (Value1 as string[]).Length == 0;
                    }
                    if (Value1 is double[])
                    {
                        return (Value1 as double[]).Length == 0;
                    }
                    break;
                case LUViewCondition.ListMinCount:
                    if (Value1 == null && Value2 == null) return false;
                    if (Value1 is System.Collections.IList)
                    {
                        return (Value1 as System.Collections.IList).Count >= Convert.ToInt32(Value2);
                    }
                    return false;
                case LUViewCondition.ListMaxCount:
                    if (Value1 == null && Value2 == null) return true;
                    if (Value1 is System.Collections.IList)
                    {
                        return (Value1 as System.Collections.IList).Count <= Convert.ToInt32(Value2);
                    }
                    return false;
            }

            return false;
        }
    }
}
