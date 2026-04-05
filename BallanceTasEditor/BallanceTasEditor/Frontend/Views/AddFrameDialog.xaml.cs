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
    /// Interaction logic for AddFrameDialog.xaml
    /// </summary>
    public partial class AddFrameDialog : Window {
        public AddFrameDialog() {
            InitializeComponent();

            var vm = new ViewModels.AddFrameDialog();
            vm.RequestCloseDialog += ViewModel_RequestCloseDialog;
            this.DataContext = vm;
        }
        private void ViewModel_RequestCloseDialog(Shared.RequestCloseDialogEventArgs e) {
            this.DialogResult = e.Result;
            this.Close();
        }
    }
}
