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

        // =========================== menu
        private void funcMenu_Help_ReportBugs(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/yyc12345/BallanceTASEditor/issues");
        }

        private void funcMenu_Help_About(object sender, RoutedEventArgs e) {
            MessageBox.Show("Under MIT License\nVersion: 1.0 alpha\nyyc12345.", "Ballance TAS Editor");
        }

        private void funcMenu_File_Open(object sender, RoutedEventArgs e) {
            var file = DialogUtil.OpenFileDialog();
            if (file == "") return;
            mFile = new TASFile(file);
            mViewer = new TASViewer(mFile, uiTASSlider, uiTASData, uiStatusbar_Selected);
            RefreshUI(true);
            ChangeToolMode(ToolMode.Cursor);
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

        private void funcMenu_Display_ItemCount(object sender, RoutedEventArgs e) {
            int newvalue = 0;
            if (DialogUtil.InputNumber("Input new count (>=5 && <=30)", 5, 30, ref newvalue)) {
                mViewer.ChangeListLength(newvalue);
            }
        }

        // =========================== btn
        private void funcBtn_Cursor(object sender, RoutedEventArgs e) {
            ChangeToolMode(ToolMode.Cursor);
        }

        private void funcBtn_Fill(object sender, RoutedEventArgs e) {
            ChangeToolMode(ToolMode.Fill);
        }

        private void funcBtn_Overwrite(object sender, RoutedEventArgs e) {
            ChangeToolMode(ToolMode.Overwrite);
        }

        // move btn

        private void funcBtn_FastMovePrev(object sender, RoutedEventArgs e) {
            MoveSliderManually(true, true);
        }

        private void funcBtn_MovePrev(object sender, RoutedEventArgs e) {
            MoveSliderManually(true, false);
        }

        private void funcBtn_MoveNext(object sender, RoutedEventArgs e) {
            MoveSliderManually(false, false);
        }

        private void funcBtn_FastMoveNext(object sender, RoutedEventArgs e) {
            MoveSliderManually(false, true);
        }

        // move keyboard

        private void funcWindow_KeyUp(object sender, KeyEventArgs e) {
            if (mFile == null || mViewer == null) return;

            switch(e.Key) {
                case Key.A:
                    MoveSliderManually(true, true);
                    break;
                case Key.S:
                    MoveSliderManually(true, false);
                    break;
                case Key.D:
                    MoveSliderManually(false, false);
                    break;
                case Key.F:
                    MoveSliderManually(false, true);
                    break;
            }
        }

        #endregion


        private void ChangeToolMode(ToolMode mode) {
            switch (mode) {
                case ToolMode.Cursor:
                    uiBtn_Cursor.IsEnabled = false;
                    uiBtn_Fill.IsEnabled = true;
                    uiBtn_Overwrite.IsEnabled = true;

                    uiStatusbar_Mode_Cursor.Visibility = Visibility.Visible;
                    uiStatusbar_Mode_Fill.Visibility = Visibility.Collapsed;
                    uiStatusbar_Mode_Overwrite.Visibility = Visibility.Collapsed;
                    break;
                case ToolMode.Fill:
                    uiBtn_Cursor.IsEnabled = true;
                    uiBtn_Fill.IsEnabled = false;
                    uiBtn_Overwrite.IsEnabled = true;

                    uiStatusbar_Mode_Cursor.Visibility = Visibility.Collapsed;
                    uiStatusbar_Mode_Fill.Visibility = Visibility.Visible;
                    uiStatusbar_Mode_Overwrite.Visibility = Visibility.Collapsed;
                    break;
                case ToolMode.Overwrite:
                    uiBtn_Cursor.IsEnabled = true;
                    uiBtn_Fill.IsEnabled = true;
                    uiBtn_Overwrite.IsEnabled = false;

                    uiStatusbar_Mode_Cursor.Visibility = Visibility.Collapsed;
                    uiStatusbar_Mode_Fill.Visibility = Visibility.Collapsed;
                    uiStatusbar_Mode_Overwrite.Visibility = Visibility.Visible;
                    break;
            }

            mViewer.ChangeToolMode(mode);
        }

        private void RefreshUI(bool isFileOpened) {
            if (isFileOpened) {
                uiEditorPanel.Visibility = Visibility.Visible;
                uiEditorNote.Visibility = Visibility.Collapsed;

                uiMenu_File_Open.IsEnabled = false;
                uiMenu_File_Save.IsEnabled = true;
                uiMenu_File_SaveAs.IsEnabled = true;
                uiMenu_File_Close.IsEnabled = true;

                uiMenu_Display_ItemCount.IsEnabled = true;

                uiStatusbar.Visibility = Visibility.Visible;
            } else {
                uiEditorPanel.Visibility = Visibility.Collapsed;
                uiEditorNote.Visibility = Visibility.Visible;

                uiMenu_File_Open.IsEnabled = true;
                uiMenu_File_Save.IsEnabled = false;
                uiMenu_File_SaveAs.IsEnabled = false;
                uiMenu_File_Close.IsEnabled = false;

                uiMenu_Display_ItemCount.IsEnabled = false;

                uiStatusbar.Visibility = Visibility.Collapsed;
            }
        }

        private void MoveSliderManually(bool isPrev, bool isFast) {
            var step = isFast ? mViewer.GetItemCountInPage() : 1;
            uiTASSlider.Value = Util.Clamp(uiTASSlider.Value.ToInt32() + (isPrev ? -1 : 1) * step, uiTASSlider.Minimum.ToInt32(), uiTASSlider.Maximum.ToInt32());
        }

    }
}
