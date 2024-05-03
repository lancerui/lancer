using LancerUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LancerUI.Controls.Navigation
{
    public class LUSelectCommands
    {
        public static RoutedUICommand SelectItemCommand { get; } = new RoutedUICommand("", "SelectItemCommand", typeof(LUSelectCommands));
    }
}
