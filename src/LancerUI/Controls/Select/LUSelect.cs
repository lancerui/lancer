using LancerUI.Controls.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LancerUI.Controls.Select
{
    public class LUSelect : ItemsControl
    {
        /// <summary>
        /// 当前选择的索引
        /// </summary>
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetValue(SelectedIndexProperty, value);
        }
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(LUSelect), new PropertyMetadata(0, new PropertyChangedCallback(OnSelectedIndexChanged)));
        /// <summary>
        /// 选中项
        /// </summary>
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(LUSelect), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUSelect;
            if (e.NewValue != e.OldValue)
            {
                control.HandleSelectedItemChanged();
            }
        }

        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            set => SetValue(IsPressedProperty, value);
        }
        public static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.Register("IsPressed", typeof(bool), typeof(LUSelect), new PropertyMetadata(false));

        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUSelect;
            if (e.NewValue != e.OldValue)
            {
                control.HandleSelectedIndexChanged();
            }
        }

        /// <summary>
        /// 是否绑定数据源
        /// </summary>
        private bool IsBinding
        {
            get => ItemsSource != null;
        }

        private Popup _popup;
        private Border _button;
        private TextBlock _displayText;

        private LUSelectItem _lastSelectedItem;
        public LUSelect()
        {
            DefaultStyleKey = typeof(LUSelect);
            CommandBindings.Add(new CommandBinding(LUSelectCommands.SelectItemCommand, OnSelectItemCommand));

            Loaded += LUSelect_Loaded;
        }

        private void LUSelect_Loaded(object sender, RoutedEventArgs e)
        {
            SetDefaultDisplayText();
        }

        private void SetDefaultDisplayText()
        {
            if (Items.Count == 0) return;
            if (IsBinding)
            {
                if (SelectedItem == null) return;
                SelectedIndex = Items.IndexOf(SelectedItem);
            }
            else
            {
                //  非绑定模式
                if (SelectedIndex > Items.Count - 1) return;
                SelectedItem = Items[SelectedIndex];
            }

            UpdateDisplayText();
        }
        private void OnSelectItemCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.OriginalSource as LUSelectItem;
            HandleSelectedItem(item);
            _popup.IsOpen = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _popup = GetTemplateChild("Popup") as Popup;
            _button = GetTemplateChild("Button") as Border;
            _displayText = GetTemplateChild("DisplayText") as TextBlock;

            HandleButtonEvent();
            SetDisplayBinding();
            BindingEvent();
        }

        private void BindingEvent()
        {
            if (_popup == null) return;
            _popup.Opened += _popup_Opened;
        }

        private void _popup_Opened(object sender, EventArgs e)
        {
            SetDefaultCheckedItem();
        }

        private void SetDefaultCheckedItem()
        {
            if (IsBinding)
            {
                //  绑定模式
                if (SelectedItem == null) return;
                var selectedItemContent = ItemContainerGenerator.ContainerFromItem(SelectedItem) as ContentPresenter;
                var item = VisualTreeHelper.GetChild(selectedItemContent, 0) as LUSelectItem;
                item.IsChecked = true;
                _lastSelectedItem = item;
            }
            else
            {
                //  非绑定模式
                if (SelectedIndex > Items.Count - 1) return;
                var item = Items[SelectedIndex] as LUSelectItem;
                item.IsChecked = true;
                _lastSelectedItem = item;
            }
        }

        private void SetDisplayBinding()
        {
            if (!IsBinding || _displayText == null) return;

            DisplayMemberPath = "Text";

            var binding = new Binding
            {
                Source = SelectedItem,
                Path = new PropertyPath(DisplayMemberPath),
                Mode = BindingMode.TwoWay
            };

            _displayText.SetBinding(TextBlock.TextProperty, binding);
        }

        private void HandleButtonEvent()
        {
            if (_button == null || _popup == null) return;
            _button.MouseLeftButtonDown += (s, e) =>
            {
                if (!IsEnabled) return;

                IsPressed = true;
            };
            _button.MouseLeftButtonUp += (s, e) =>
            {
                IsPressed = false;
                _popup.IsOpen = true;
            };
        }

        private void HandleSelectedItem(LUSelectItem selectedItem_)
        {
            if (selectedItem_.IsChecked) return;

            if (_lastSelectedItem != null)
            {
                _lastSelectedItem.IsChecked = false;
            }
            selectedItem_.IsChecked = true;
            _lastSelectedItem = selectedItem_;

            if (IsBinding)
            {
                SelectedItem = selectedItem_.DataContext;
            }
            else
            {
                SelectedItem = selectedItem_;
            }

            SelectedIndex = Items.IndexOf(SelectedItem);

            UpdateDisplayText();
            Debug.WriteLine("当前选中：" + SelectedIndex + "," + SelectedItem);
        }

        private void UpdateDisplayText()
        {
            if (IsBinding)
            {
                SetDisplayBinding();
            }
            else
            {
                if (SelectedItem != null && _displayText != null)
                {
                    _displayText.Text = (SelectedItem as LUSelectItem).Text;
                }
            }
        }

        private void HandleSelectedIndexChanged()
        {
            if (Items.Count == 0) return;

            if (IsBinding)
            {
                var selectedItemContent = ItemContainerGenerator.ContainerFromItem(Items[SelectedIndex]) as ContentPresenter;
                if (selectedItemContent == null) return;
                var item = VisualTreeHelper.GetChild(selectedItemContent, 0) as LUSelectItem;
                item.IsChecked = true;
            }
            else
            {
                //  非绑定模式
                var item = Items[SelectedIndex] as LUSelectItem;
                item.IsChecked = true;
            }

            SelectedItem = Items[SelectedIndex];
            UpdateDisplayText();
        }

        private void HandleSelectedItemChanged()
        {
            if (!IsBinding || Items.Count == 0) return;

            SelectedIndex = Items.IndexOf(SelectedItem);
            UpdateDisplayText();
        }
    }
}
