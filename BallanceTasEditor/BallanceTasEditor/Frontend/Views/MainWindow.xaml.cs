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

namespace BallanceTasEditor.Frontend.Views {
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

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) {
            var dialog =new GotoDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e) {
            var dialog = new EditFpsDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuItem_Click_6(object sender, RoutedEventArgs e) {
            var dialog = new EditFpsDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e) {
            var dialog = new AddFrameDialog();
            dialog.Owner = this;
            dialog.ShowDialog();
        }

    }
}
