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
            mFlow = new TASFlow(uiTASData);
            mSlider = new TASSlider(uiTASSlider);
        }

        TASFile mFile;
        TASViewer mViewer;
        TASFlow mFlow;
        TASSlider mSlider;

        #region ui func

        // shortcut

        private void funcCommand_Menu_File_Open(object sender, ExecutedRoutedEventArgs e) => funcMenu_File_Open(sender, e);
        private void funcCommand_Menu_File_Save(object sender, ExecutedRoutedEventArgs e) => funcMenu_File_Save(sender, e);
        private void funcCommand_Menu_Display_Undo(object sender, ExecutedRoutedEventArgs e) => funcMenu_Display_Undo(sender, e);
        private void funcCommand_Menu_Display_Redo(object sender, ExecutedRoutedEventArgs e) => funcMenu_Display_Redo(sender, e);
        private void funcCanExeCmd_Menu_File_Open(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_File_Open.IsEnabled;
        private void funcCanExeCmd_Menu_File_Save(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_File_Save.IsEnabled;
        private void funcCanExeCmd_Menu_Display_Undo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_Display_Undo.IsEnabled;
        private void funcCanExeCmd_Menu_Display_Redo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_Display_Redo.IsEnabled;

        // =========================== menu
        #region window menu

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
            mViewer = new TASViewer(mFile, mSlider, mFlow);

            mViewer.UpdateDataUI += RefreshDataUI;
            mViewer.UpdateSelection += RefreshSelection;
            mViewer.UpdateToolMode += RefreshToolMode;

            RefreshUI(true);

            mViewer.ChangeToolMode(ToolMode.Cursor);
            mViewer.ChangeOverwrittenMode(uiMenu_Display_OverwrittenPaste.IsChecked);
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

            mViewer.UpdateDataUI -= RefreshDataUI;
            mViewer.UpdateSelection -= RefreshSelection;
            mViewer.UpdateToolMode -= RefreshToolMode;

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

        private void funcMenu_Display_OverwrittenPaste(object sender, RoutedEventArgs e) {
            //uiMenu_Display_OverwrittenPaste.IsChecked = !uiMenu_Display_OverwrittenPaste.IsChecked;
            if (mViewer != null)
                mViewer.ChangeOverwrittenMode(uiMenu_Display_OverwrittenPaste.IsChecked);
        }

        private void funcMenu_Display_Redo(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Redo);
        }

        private void funcMenu_Display_Undo(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Undo);
        }

        #endregion

        #region menu operation

        private void uiDataMenu_Set_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Set);
        }

        private void uiDataMenu_Unset_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Unset);
        }

        private void uiDataMenu_Cut_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Cut);
        }

        private void uiDataMenu_Copy_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Copy);
        }

        private void uiDataMenu_PasteAfter_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.PasteAfter);
        }

        private void uiDataMenu_PasteBefore_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.PasteBefore);
        }

        private void uiDataMenu_Delete_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Delete);
        }

        private void uiDataMenu_DeleteAfter_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.DeleteAfter);
        }

        private void uiDataMenu_DeleteBefore_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.DeleteBefore);
        }

        private void uiDataMenu_AddAfter_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.AddAfter);
        }

        private void uiDataMenu_AddBefore_Click(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.AddBefore);
        }

        #endregion

        // =========================== btn
        private void funcBtn_Cursor(object sender, RoutedEventArgs e) {
            mViewer.ChangeToolMode(ToolMode.Cursor);
        }

        private void funcBtn_Fill(object sender, RoutedEventArgs e) {
            mViewer.ChangeToolMode(ToolMode.Fill);
        }

        private void funcBtn_Overwrite(object sender, RoutedEventArgs e) {
            mViewer.ChangeToolMode(ToolMode.Overwrite);
        }

        // move btn

        private void funcBtn_FastMovePrev(object sender, RoutedEventArgs e) {
            mSlider.MoveSliderManually(true, true, mViewer.GetItemCountInPage());
        }

        private void funcBtn_MovePrev(object sender, RoutedEventArgs e) {
            mSlider.MoveSliderManually(true, false, mViewer.GetItemCountInPage());
        }

        private void funcBtn_MoveNext(object sender, RoutedEventArgs e) {
            mSlider.MoveSliderManually(false, false, mViewer.GetItemCountInPage());
        }

        private void funcBtn_FastMoveNext(object sender, RoutedEventArgs e) {
            mSlider.MoveSliderManually(false, true, mViewer.GetItemCountInPage());
        }

        // move keyboard

        private void funcWindow_KeyUp(object sender, KeyEventArgs e) {
            if (mFile == null || mViewer == null) return;

            switch(e.Key) {
                case Key.A:
                    mSlider.MoveSliderManually(true, true, mViewer.GetItemCountInPage());
                    break;
                case Key.S:
                    mSlider.MoveSliderManually(true, false, mViewer.GetItemCountInPage());
                    break;
                case Key.D:
                    mSlider.MoveSliderManually(false, false, mViewer.GetItemCountInPage());
                    break;
                case Key.F:
                    mSlider.MoveSliderManually(false, true, mViewer.GetItemCountInPage());
                    break;
            }
        }

        // move mouse

        private void funcWindow_MouseWheel(object sender, MouseWheelEventArgs e) {
            if (e.Delta > 0) {
                // wheel up
                if (KeyboardState.IsKeyPressed(KeyboardState.VirtualKeyStates.VK_SHIFT)) {
                    // move quickly
                    mSlider.MoveSliderManually(true, true, mViewer.GetItemCountInPage());
                } else if (KeyboardState.IsKeyPressed(KeyboardState.VirtualKeyStates.VK_CONTROL)) {
                    // decrease item count
                    var newvalue = mViewer.GetItemCountInPage();
                    mViewer.ChangeListLength(newvalue - 1);
                } else {
                    // normally move
                    mSlider.MoveSliderManually(true, false, mViewer.GetItemCountInPage());
                }
                
            } else if (e.Delta < 0) {
                // wheel down
                if (KeyboardState.IsKeyPressed(KeyboardState.VirtualKeyStates.VK_SHIFT)) {
                    // move quickly
                    mSlider.MoveSliderManually(false, true, mViewer.GetItemCountInPage());
                } else if (KeyboardState.IsKeyPressed(KeyboardState.VirtualKeyStates.VK_CONTROL)) {
                    // increase item count
                    var newvalue = mViewer.GetItemCountInPage();
                    mViewer.ChangeListLength(newvalue + 1);
                } else {
                    // normally move
                    mSlider.MoveSliderManually(false, false, mViewer.GetItemCountInPage());
                }
            }
        }

        #endregion

        private void RefreshToolMode(ToolMode mode) {
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
                uiMenu_Display_OverwrittenPaste.IsEnabled = true;
                uiMenu_Display_Undo.IsEnabled = true;
                uiMenu_Display_Redo.IsEnabled = true;

                uiStatusbar.Visibility = Visibility.Visible;
            } else {
                uiEditorPanel.Visibility = Visibility.Collapsed;
                uiEditorNote.Visibility = Visibility.Visible;

                uiMenu_File_Open.IsEnabled = true;
                uiMenu_File_Save.IsEnabled = false;
                uiMenu_File_SaveAs.IsEnabled = false;
                uiMenu_File_Close.IsEnabled = false;

                uiMenu_Display_ItemCount.IsEnabled = false;
                uiMenu_Display_OverwrittenPaste.IsEnabled = false;
                uiMenu_Display_Undo.IsEnabled = false;
                uiMenu_Display_Redo.IsEnabled = false;

                uiStatusbar.Visibility = Visibility.Collapsed;
            }
        }

        private void RefreshDataUI(bool showCursorPasteAddDeleteOne, bool showFill, bool showCursorCopyDelete) {
            uiDataMenu_Set.IsEnabled = showFill;
            uiDataMenu_Unset.IsEnabled = showFill;
            uiDataMenu_Cut.IsEnabled = showCursorCopyDelete;
            uiDataMenu_Copy.IsEnabled = showCursorCopyDelete;
            uiDataMenu_Delete.IsEnabled = showCursorCopyDelete;
            uiDataMenu_DeleteAfter.IsEnabled = showCursorPasteAddDeleteOne;
            uiDataMenu_DeleteBefore.IsEnabled = showCursorPasteAddDeleteOne;
            uiDataMenu_PasteAfter.IsEnabled = showCursorPasteAddDeleteOne;
            uiDataMenu_PasteBefore.IsEnabled = showCursorPasteAddDeleteOne;
            uiDataMenu_AddAfter.IsEnabled = showCursorPasteAddDeleteOne;
            uiDataMenu_AddBefore.IsEnabled = showCursorPasteAddDeleteOne;
        }

        private void RefreshSelection(SelectionHelp mSelectionHelp) {
            var mode = mSelectionHelp.GetToolMode();

            switch (mode) {
                case ToolMode.Cursor:
                    if (mSelectionHelp.IsDataReady()) {
                        var data = mSelectionHelp.GetRange();
                        uiStatusbar_Selected.Text = $"{data.start} - {data.end}";
                    } else if (mSelectionHelp.IsDataPartialReady()) {
                        var data2 = mSelectionHelp.GetPoint();
                        uiStatusbar_Selected.Text = data2.ToString();
                    } else uiStatusbar_Selected.Text = "-";
                    break;
                case ToolMode.Fill:
                    if (mSelectionHelp.IsDataReady()) {
                        var data3 = mSelectionHelp.GetRange();
                        uiStatusbar_Selected.Text = $"{data3.start} - {data3.end}";
                    } else uiStatusbar_Selected.Text = "-";
                    break;
                case ToolMode.Overwrite:
                    if (mSelectionHelp.IsDataReady()) {
                        var data4 = mSelectionHelp.GetPoint();
                        uiStatusbar_Selected.Text = data4.ToString();
                    } else uiStatusbar_Selected.Text = "-";
                    break;
            }
        }

    }
}
