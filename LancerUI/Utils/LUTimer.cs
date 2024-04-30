using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LancerUI.Utils
{
    public class LUTimer
    {
        private System.Timers.Timer _timer;
        public event EventHandler OnTimeOut;
        public LUTimer(double timeout_)
        {
            _timer = new System.Timers.Timer();
            _timer.Interval = timeout_;
            _timer.Elapsed += _timer_Elapsed;
        }

        private void _timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            OnTimeOut?.Invoke(sender, e);
            _timer.Stop();
        }

        public void Start()
        {
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
        }
    }
}
