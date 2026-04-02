using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace BallanceTasEditor.Frontend.Behaviors {
    public class ConfirmCloseBehavior : Behavior<Window> {

        public ICommand ConfirmCommand {
            get { return (ICommand)GetValue(ConfirmCommandProperty); }
            set { SetValue(ConfirmCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ConfirmCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfirmCommandProperty =
            DependencyProperty.Register("ConfirmCommand", typeof(ICommand), typeof(ConfirmCloseBehavior));


        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Closing += OnClosing;
        }

        protected override void OnDetaching() {
            AssociatedObject.Closing -= OnClosing;
            base.OnDetaching();
        }

        private void OnClosing(object? sender, CancelEventArgs e) {
            if (ConfirmCommand?.CanExecute(null) == true) {
                // 假设Command返回 bool 或通过回调/事件通知结果
                bool allowClose = (Func<object?, bool>)ConfirmCommand.Execute(null);
                e.Cancel = !allowClose;
            }
        }
    }
}
