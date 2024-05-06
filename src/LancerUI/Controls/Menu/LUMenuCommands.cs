using LancerUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LancerUI.Controls.Menu
{
    public class LUMenuCommands
    {
        public static RoutedUICommand MenuItemClickCommand { get; } = new RoutedUICommand("", "MenuItemClickCommand", typeof(LUMenuCommands));
    }
}
