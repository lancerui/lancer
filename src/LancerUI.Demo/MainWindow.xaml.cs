using LancerUI.Controls.Buttons;
using LancerUI.Controls.Types;
using LancerUI.Controls.Windows;
using LancerUI.Extensions;
using LancerUI.Language;
using LancerUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LancerUI.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : LUWindow
    {
        public class Command : ICommand
        {
            private Action<object> _action;

            public Command(Action<object> action)
            {
                _action = action;
            }


            #region ICommand Members  
            public bool CanExecute(object parameter)
            {
                return true;
            }
            public event EventHandler CanExecuteChanged;
            public void Execute(object parameter)
            {
                _action(parameter);
                OnExecuted();
            }
            public delegate void ExecutedHandler(object parameter);
            public event ExecutedHandler Executed;
            public void OnExecuted()
            {
                Executed?.Invoke(null);
            }
            #endregion
        }
        public class MenuItem
        {
            public string Title { get; set; }
            public IconSymbol Icon { get; set; }
            public IconSymbol SelectedIcon { get; set; }
        }
        public ObservableCollection<MenuItem> MenuItems { get; set; }
        public Command HomeCommand { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            HomeCommand = new Command((parameter) =>
            {
                MessageBox.Show("命令绑定测试弹窗");
            });

            MenuItems = new ObservableCollection<MenuItem>()
            {
                new MenuItem()
                {
                    Title="主页",
                    Icon= IconSymbol.Home,
                    SelectedIcon= IconSymbol.HomeFilled
                },
                  new MenuItem()
                {
                    Title="游戏",
                    Icon= IconSymbol.Games,
                    SelectedIcon= IconSymbol.GamesFilled
                },
               
            };

            DataContext = this;

            //  设置主题色
            //ThemeManager.SetWindowsThemeColor();
            //  设置语言
            LanguageManager.SetLanguage();

            var test = IconSymbol.ZoomFit.String();
        }

        private void LUButton_Click(object sender, RoutedEventArgs e)
        {
            
            MessageBox.Show("删除成功（无作用弹窗）");
        }
    }
}
