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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BallanceTasEditor.Views {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) {
            var dialog = new NewFileDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            var dialog = new PreferenceDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e) {
            var dialog = new AboutDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) {
            // TODO; Change Title and Icon
            var dialog = new SetupCountAndFpsDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

    }
}
