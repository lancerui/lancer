using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LancerUI.Controls.Separator
{
    public class LUSeparator : System.Windows.Controls.Separator
    {
        public LUSeparator()
        {
            DefaultStyleKey = typeof(LUSeparator);
            IsEnabled = false;
        }
    }
}
