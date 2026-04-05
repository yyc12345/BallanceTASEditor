using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace BallanceTasEditor.Frontend.Views {
    /// <summary>
    /// Interaction logic for AboutDialog.xaml
    /// </summary>
    public partial class AboutDialog : Window {
        public AboutDialog() {
            InitializeComponent();

            var vm = new ViewModels.AboutDialog();
            vm.RequestCloseDialog += ViewModel_RequestCloseDialog;
            this.DataContext = vm;
        }

        private void ViewModel_RequestCloseDialog(Shared.RequestCloseDialogEventArgs e) {
            this.DialogResult = e.Result;
            this.Close();
        }
    }
}
