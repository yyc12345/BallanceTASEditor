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
    /// Interaction logic for NewFileDialog.xaml
    /// </summary>
    public partial class NewFileDialog : Window {
        public NewFileDialog() {
            InitializeComponent();

            ViewModel = new ViewModels.NewFileDialog();
            ViewModel.RequestCloseDialog += ViewModel_RequestCloseDialog;
            this.DataContext = ViewModel;
        }

        public ViewModels.NewFileDialog ViewModel { get; private set; }

        private void ViewModel_RequestCloseDialog(Shared.RequestCloseDialogEventArgs e) {
            this.DialogResult = e.Result;
            this.Close();
        }
    }
}
