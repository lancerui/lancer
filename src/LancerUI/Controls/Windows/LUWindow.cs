using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Drawing;
using System.Diagnostics;

namespace LancerUI.Controls.Windows
{
    public class LUWindow : Window
    {
        #region 最小化按钮显示状态

        public static readonly DependencyProperty MinimizeVisibilityProperty = DependencyProperty.Register("MinimizeVisibility", typeof(Visibility), typeof(LUWindow));

        /// <summary>
        /// 最小化按钮显示状态
        /// </summary>
        public Visibility MinimizeVisibility
        {
            get { return (Visibility)GetValue(MinimizeVisibilityProperty); }
            set { SetValue(MinimizeVisibilityProperty, value); }
        }
        #endregion   

        #region 最大化按钮显示状态

        public static readonly DependencyProperty MaximizeVisibilityProperty = DependencyProperty.Register("MaximizeVisibility", typeof(Visibility), typeof(LUWindow));

        /// <summary>
        /// 最大化按钮显示状态
        /// </summary>
        public Visibility MaximizeVisibility
        {
            get { return (Visibility)GetValue(MaximizeVisibilityProperty); }
            set { SetValue(MaximizeVisibilityProperty, value); }
        }
        #endregion

        #region 关闭按钮显示状态

        public static readonly DependencyProperty CloseVisibilityProperty = DependencyProperty.Register("CloseVisibility", typeof(Visibility), typeof(LUWindow));

        /// <summary>
        /// 关闭按钮显示状态
        /// </summary>
        public Visibility CloseVisibility
        {
            get { return (Visibility)GetValue(CloseVisibilityProperty); }
            set { SetValue(CloseVisibilityProperty, value); }
        }
        #endregion

        #region 标题栏副区域内容
        public static readonly DependencyProperty TitleBarSubContentProperty = DependencyProperty.Register("TitleBarSubContent", typeof(object), typeof(LUWindow));
        /// <summary>
        /// 拓展内容
        /// </summary>
        public object TitleBarSubContent
        {
            get { return GetValue(TitleBarSubContentProperty); }
            set { SetValue(TitleBarSubContentProperty, value); }
        }
        #endregion

        #region 标题栏主区域内容
        public static readonly DependencyProperty TitleBarMainContentProperty = DependencyProperty.Register("TitleBarMainContent", typeof(object), typeof(LUWindow));
        /// <summary>
        /// 拓展内容
        /// </summary>
        public object TitleBarMainContent
        {
            get { return GetValue(TitleBarMainContentProperty); }
            set { SetValue(TitleBarMainContentProperty, value); }
        }
        #endregion

        #region 初始化
        public LUWindow()
        {
            this.DefaultStyleKey = typeof(LUWindow);
            //添加当前窗体到窗体集合

            //命令绑定
            this.CommandBindings.Add(new CommandBinding(LUWindowCommands.MinimizeWindowCommand, OnMinimizeWindowCommand));
            this.CommandBindings.Add(new CommandBinding(LUWindowCommands.MaximizeWindowCommand, OnMaximizeWindowCommand));
            this.CommandBindings.Add(new CommandBinding(LUWindowCommands.RestoreWindowCommand, OnRestoreWindowCommand));
            this.CommandBindings.Add(new CommandBinding(LUWindowCommands.CloseWindowCommand, OnCloseWindowCommand));

            Loaded += window_Loaded;
            Unloaded += LUWindow_Unloaded;
        }

        private void LUWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= LUWindow_Unloaded;
            Loaded -= window_Loaded;
        }


        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            OnSystemButtonsVisibility();
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }


        /// <summary>
        /// 根据窗口ResizeMode设置系统按钮的可视状态
        /// </summary>
        private void OnSystemButtonsVisibility()
        {
            switch (ResizeMode)
            {
                case ResizeMode.NoResize:
                    //仅显示关闭按钮
                    MinimizeVisibility = Visibility.Collapsed;
                    MaximizeVisibility = Visibility.Collapsed;
                    CloseVisibility = Visibility.Visible;
                    break;
                case ResizeMode.CanResize:
                    MinimizeVisibility = Visibility.Visible;
                    MaximizeVisibility = Visibility.Visible;
                    CloseVisibility = Visibility.Visible;
                    break;
                case ResizeMode.CanMinimize:
                    MinimizeVisibility = Visibility.Visible;
                    MaximizeVisibility = Visibility.Collapsed;
                    CloseVisibility = Visibility.Visible;
                    break;
            }
        }
        #endregion

        #region 响应命令

        private void OnCloseWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void OnRestoreWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void OnMaximizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }

        private void OnMinimizeWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region win32
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = (-20);

        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);

        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);
        #endregion
    }
}
