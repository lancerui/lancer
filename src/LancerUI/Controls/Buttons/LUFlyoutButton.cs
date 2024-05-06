using LancerUI.Controls.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LancerUI.Controls.Buttons
{
    public class LUFlyoutButton : LUButton
    {
        public object Flyout { get => GetValue(FlyoutProperty); set => SetValue(FlyoutProperty, value); }
        public static readonly System.Windows.DependencyProperty FlyoutProperty = System.Windows.DependencyProperty.Register("Flyout", typeof(object), typeof(LUFlyoutButton), new System.Windows.PropertyMetadata(null));
        
        public bool IsOpen { get => (bool)GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public static readonly System.Windows.DependencyProperty IsOpenProperty = System.Windows.DependencyProperty.Register("IsOpen", typeof(bool), typeof(LUFlyoutButton), new System.Windows.PropertyMetadata(false));

        private LUButton _button;
        private Popup _popup;
        public LUFlyoutButton()
        {
            DefaultStyleKey = typeof(LUFlyoutButton);
            CommandBindings.Add(new CommandBinding(LUMenuCommands.MenuItemClickCommand, OnMenuItemClickCommand));
        }

        private void OnMenuItemClickCommand(object sender, ExecutedRoutedEventArgs e)
        {
            HideFlyout();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _button = GetTemplateChild("Button") as LUButton;
            _popup = GetTemplateChild("FlyoutPopup") as Popup;
            BindingEvent();
        }

        private void BindingEvent()
        {
            if (_button == null || _popup==null) return;
            _button.Click += (s, e) =>
            {
                IsOpen = !IsOpen;
            };
            _popup.Closed += (s, e) =>
            {
                IsOpen = false;
            };
        }

        public void HideFlyout()
        {
            IsOpen = false;
        }
    }
}
