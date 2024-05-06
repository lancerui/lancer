using LancerUI.Controls.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LancerUI.Controls.Menu
{
    public class LUMenuItem : MenuItem
    {
        public LUMenuItem()
        {
            DefaultStyleKey = typeof(LUMenuItem);
           this.Click += LUMenuItem_Click;
        }

        private void LUMenuItem_Click(object sender, RoutedEventArgs e)
        {
            LUMenuCommands.MenuItemClickCommand.Execute(sender, this);
        }
    }
}
