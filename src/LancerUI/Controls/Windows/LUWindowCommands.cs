using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LancerUI.Controls.Windows
{
    public class LUWindowCommands
    {
        public static RoutedUICommand MinimizeWindowCommand { get; } = new RoutedUICommand("", "MinimizeWindowCommand", typeof(LUWindowCommands));
        public static RoutedUICommand RestoreWindowCommand { get; } = new RoutedUICommand("", "RestoreWindowCommand", typeof(LUWindowCommands));
        public static RoutedUICommand MaximizeWindowCommand { get; } = new RoutedUICommand("", "MaximizeWindowCommand", typeof(LUWindowCommands));
        public static RoutedUICommand CloseWindowCommand { get; } = new RoutedUICommand("", "CloseWindowCommand", typeof(LUWindowCommands));

    }
}
