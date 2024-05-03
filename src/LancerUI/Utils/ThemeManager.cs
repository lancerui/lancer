using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LancerUI.Utils
{
    /// <summary>
    /// 主题管理
    /// </summary>
    public class ThemeManager
    {
        /// <summary>
        /// 设置当前主题色为Windows系统主题色
        /// </summary>
        /// <returns></returns>
        public static bool SetWindowsThemeColor()
        {
            //var color = new Windows.UI.Color();
            //if (Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.UI.ViewManagement.UISettings", "GetColorValue"))
            //{
            //    var uiSettings = new Windows.UI.ViewManagement.UISettings();
            //    color = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);

            //    Application.Current.Resources["ThemeColor"] = Color.FromArgb(color.A, color.R, color.G, color.B);
            //    Application.Current.Resources["ThemeBrush"] = new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            //    return true;
            //}
            return false;
        }
    }
}
