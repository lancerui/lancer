using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace LancerUI.Controls.Input
{
    public class LUPasswordInput : LUInputBase
    {
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get => (string)GetValue(PasswordProperty); set => SetValue(PasswordProperty, value); }
        public static readonly DependencyProperty PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(LUPasswordInput), new PropertyMetadata(string.Empty, new PropertyChangedCallback(OnPasswordPropertyChanged)));

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LUPasswordInput;
            if (e.OldValue != e.NewValue)
            {
                control._password = e.NewValue as string;
                if (control.Text.Length != control._password.Length)
                {
                    control.Text = control.GetPasswordChar(control._password);
                }
                Debug.WriteLine("外部更改：" + control.Password);
            }
        }

        /// <summary>
        /// 密码显示字符（默认为"●"，长度限制1）
        /// </summary>
        public string PasswordChar { get => (string)GetValue(PasswordCharProperty); set => SetValue(PasswordCharProperty, value); }
        public static readonly DependencyProperty PasswordCharProperty = DependencyProperty.Register("PasswordChar", typeof(string), typeof(LUPasswordInput));

        /// <summary>
        /// 明文密码
        /// </summary>
        private string _password = string.Empty;
        private char _passwordChar = '●';

        public LUPasswordInput()
        {
            DefaultStyleKey = typeof(LUPasswordInput);
            CommandBindings.Add(new CommandBinding(LUInputCommands.ClearableCommand, OnClearable));
            Init();
        }

        private void Init()
        {
            //  禁用输入法
            InputMethod.SetIsInputMethodEnabled(this, false);

            //  禁用右键菜单
            ContextMenu = null;

            //  禁止粘贴
            DataObject.AddPastingHandler(this, OnPaste);

        }

        private void OnClearable(object sender, ExecutedRoutedEventArgs e)
        {
            _password = string.Empty;
            Text = string.Empty;
            Password = string.Empty;
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            //  将粘贴数据清空
            if (e.DataObject.GetDataPresent(DataFormats.UnicodeText))
            {
                e.DataObject = new DataObject(DataFormats.UnicodeText, string.Empty);
            }
        }

        protected override void OnSelectionChanged(RoutedEventArgs e)
        {
            base.OnSelectionChanged(e);
            //  不允许选择文本
            if (SelectionLength > 0)
            {
                SelectionLength = 0;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            //  禁用Ctrl组合键，防止复制、粘贴、剪切、全选
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.C || e.Key == Key.V || e.Key == Key.X || e.Key == Key.A)
                {
                    e.Handled = true;
                }
            }

        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (e.Text.Length > 0)
            {
                //  处理密码输入和暗文显示
                e.Handled = true;
                SetPassword(e.Text);
            }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            //  处理删除
            if (Text.Length < _password.Length)
            {
                _password = _password.Remove(SelectionStart, 1);
                Password = _password;
            }
        }


        /// <summary>
        /// 从明文转为暗文
        /// </summary>
        /// <param name="text_"></param>
        /// <returns></returns>
        private string GetPasswordChar(string text_)
        {
            if (string.IsNullOrEmpty(text_)) { return string.Empty; }
            if (string.IsNullOrEmpty(PasswordChar))
            {
                PasswordChar = _passwordChar.ToString();
            }
            var _char = PasswordChar.ToCharArray()[0];

            return new string(_char, text_.Length);
        }

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="text_">密码明文</param>
        /// <param name="isPaste_">是否来自于粘贴</param>
        private void SetPassword(string text_, bool isPaste_ = false)
        {
            if (text_.Length > 0)
            {
                var point = SelectionStart;
                _password = _password.Insert(point, text_);
                if (!isPaste_)
                {
                    Text = Text.Insert(point, GetPasswordChar(text_));
                    SelectionStart = point + 1;
                }
                Password = _password;
                Debug.WriteLine("当前密码：" + _password);
            }

        }
    }
}
