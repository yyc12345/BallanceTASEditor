using BallanceTASEditor.Core;
using BallanceTASEditor.Core.TASStruct;
using BallanceTASEditor.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BallanceTASEditor {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            RefreshUI(false);
        }

        TASFile mFile;
        TASViewer mViewer;

        #region ui func

        // menu
        private void funcMenu_Help_ReportBugs(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/yyc12345/BallanceTASEditor/issues");
        }

        private void funcMenu_Help_About(object sender, RoutedEventArgs e) {
            MessageBox.Show("Under MIT License\nVersion 0\nyyc12345.", "Ballance TAS Editor");
        }

        private void funcMenu_File_Open(object sender, RoutedEventArgs e) {
            var file = DialogUtil.OpenFileDialog();
            if (file == "") return;
            mFile = new TASFile(file);
            mViewer = new TASViewer(mFile, uiTASSlider, uiTASData);
            RefreshUI(true);
        }

        private void funcMenu_File_Save(object sender, RoutedEventArgs e) {
            mFile.Save();
        }

        private void funcMenu_File_SaveAs(object sender, RoutedEventArgs e) {
            var file = DialogUtil.SaveFileDialog();
            if (file == "") return;
            mFile.SaveAs(file);
        }

        private void funcMenu_File_Close(object sender, RoutedEventArgs e) {
            if (!DialogUtil.ConfirmDialog("Do you want to close this TAS file?")) return;
            mViewer.Dispose();
            mFile = null;
            mViewer = null;
            RefreshUI(false);
        }


        #endregion

        private void RefreshUI(bool isFileOpened) {
            if (isFileOpened) {
                uiEditorPanel.Visibility = Visibility.Visible;
                uiEditorNote.Visibility = Visibility.Collapsed;

                uiMenu_File_Open.IsEnabled = false;
                uiMenu_File_Save.IsEnabled = true;
                uiMenu_File_SaveAs.IsEnabled = true;
                uiMenu_File_Close.IsEnabled = true;
            } else {
                uiEditorPanel.Visibility = Visibility.Collapsed;
                uiEditorNote.Visibility = Visibility.Visible;

                uiMenu_File_Open.IsEnabled = true;
                uiMenu_File_Save.IsEnabled = false;
                uiMenu_File_SaveAs.IsEnabled = false;
                uiMenu_File_Close.IsEnabled = false;
            }
        }

    }
}
