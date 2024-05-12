using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Uri = System.Uri;

namespace LancerUI.Language
{
    /// <summary>
    /// 语言管理
    /// </summary>
    public class LanguageHelper
    {
        private static Lang _currentLanguage = Lang.ZHCN;
        public static Lang CurrentLanguage { get => _currentLanguage; }
        /// <summary>
        /// 设置组件库语言
        /// </summary>
        /// <param name="val_"></param>
        public static void SetLanguage(Lang val_ = Lang.ZHCN)
        {
            var MergedDictionaries = Application.Current.Resources.MergedDictionaries;
            var oldRes = MergedDictionaries.Where(m => m.Source.OriginalString.Contains("Language")).ToList();
            if (oldRes != null)
            {
                foreach (var res in oldRes)
                {
                    MergedDictionaries.Remove(res);
                }
            }

            MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"pack://application:,,,/LancerUI;component/Resources/Language/{val_}.xaml", UriKind.RelativeOrAbsolute) });
            _currentLanguage = val_;
        }

        public static string GetStr(string key)
        {
            try
            {
                return Application.Current.Resources[key] as string;
            }
            catch (Exception e)
            {
                return key;
            }
        }

        public static string GetMonthStr(int month)
        {
            string[] monthKeys = new string[] { "Lang_Jan", "Lang_Feb", "Lang_Mar", "Lang_Apr", "Lang_May", "Lang_Jun", "Lang_Jul", "Lang_Aug", "Lang_Sep", "Lang_Oct", "Lang_Nov", "Lang_Dec" };

            return GetStr(monthKeys[month - 1]);
        }
    }
}
