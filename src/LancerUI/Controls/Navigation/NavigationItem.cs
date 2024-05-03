using LancerUI.Controls.Buttons;
using LancerUI.Controls.Types;
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

namespace LancerUI.Controls.Navigation
{
    public class NavigationItem : System.Windows.Controls.Primitives.ButtonBase
    {
        #region 属性
        public string Title
        {
            get => (string)GetValue(TitleProperty);

            set => SetValue(TitleProperty, value);
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(NavigationItem), new PropertyMetadata());
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected",
                typeof(bool),
                typeof(NavigationItem), new PropertyMetadata(false));

        public IconSymbol Icon
        {
            get { return (IconSymbol)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon",
                typeof(IconSymbol),
                typeof(NavigationItem), new PropertyMetadata(IconSymbol.None));

        public IconSymbol SelectedIcon
        {
            get { return (IconSymbol)GetValue(SelectedIconIconProperty); }
            set { SetValue(SelectedIconIconProperty, value); }
        }
        public static readonly DependencyProperty SelectedIconIconProperty =
            DependencyProperty.Register("SelectedIcon",
                typeof(IconSymbol),
                typeof(NavigationItem), new PropertyMetadata(IconSymbol.None));
        #endregion
        public NavigationItem()
        {
            DefaultStyleKey = typeof(NavigationItem);
        }

        protected override void OnClick()
        {
            base.OnClick();
            NavigationCommands.ClickCommand.Execute(DataContext, this);
        }
    }
}
