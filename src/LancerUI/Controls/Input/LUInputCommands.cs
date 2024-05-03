using LancerUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LancerUI.Controls.Input
{
    public class LUInputCommands
    {
        public static RoutedUICommand ClearableCommand { get; } = new RoutedUICommand("", "ClearableCommand", typeof(LUInputCommands));
    }
}
