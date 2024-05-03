using LancerUI.Controls.Types;
using LancerUI.Controls.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace LancerUI.Controls.Input
{
    public class LUInput : LUInputBase
    {
        public LUInput()
        {
            DefaultStyleKey = typeof(LUInput);
            CommandBindings.Add(new CommandBinding(LUInputCommands.ClearableCommand, OnClearable));
        }

        private void OnClearable(object sender, ExecutedRoutedEventArgs e)
        {
            Text = string.Empty;
        }
    }
}
