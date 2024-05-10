using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LancerUI.Controls.Chart.Model
{
    public class ChartLineItem
    {
        public double[] Values { get; set; }
        public string Label { get; set; }
        public SolidColorBrush ColorBrush { get; set; }
    }
}
