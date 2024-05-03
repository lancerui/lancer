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
    public class LanguageManager
    {
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
        }
    }
}
