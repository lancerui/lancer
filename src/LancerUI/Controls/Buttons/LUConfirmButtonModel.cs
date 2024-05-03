using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LancerUI.Controls.Buttons
{
    public class LUConfirmButtonModel : UINotifyPropertyChanged
    {
        private bool _isDisplayConfirm;
        public bool IsDisplayConfirm
        {
            get => _isDisplayConfirm;
            set
            {
                _isDisplayConfirm = value;
                OnPropertyChanged();
            }
        }
    }
}
