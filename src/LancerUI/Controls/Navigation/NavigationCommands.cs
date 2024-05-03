using LancerUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LancerUI.Controls.Navigation
{
    public class NavigationCommands
    {
        public static RoutedUICommand ClickCommand { get; } = new RoutedUICommand("", "MinimizeWindowCommand", typeof(NavigationCommands));
    }
}
