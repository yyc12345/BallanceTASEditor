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
            mIsHorizontalLayout = true;

            // init layout controller
            var headers = new List<TextBlock>();
            headers.Add(uiFlowHeader_Frame);
            headers.Add(uiFlowHeader_DeltaTime);
            headers.Add(uiFlowHeader_Up);
            headers.Add(uiFlowHeader_Down);
            headers.Add(uiFlowHeader_Left);
            headers.Add(uiFlowHeader_Right);
            headers.Add(uiFlowHeader_Shift);
            headers.Add(uiFlowHeader_Space);
            headers.Add(uiFlowHeader_Q);
            headers.Add(uiFlowHeader_Esc);
            headers.Add(uiFlowHeader_Enter);
            mFlow = new TASFlow(uiTASData, headers.ToArray());

            var components = new TASSliderComponents();
            components.container = uiTASSliderContainer;
            components.btnFastPrev = uiBtn_FastMovePrev;
            components.btnPrev = uiBtn_MovePrev;
            components.btnNext = uiBtn_MoveNext;
            components.btnFastNext = uiBtn_FastMoveNext;
            components.mSlider = uiTASSlider;
            mSlider = new TASSlider(components); ;

            // refresh ui and load cfg
            RefreshUI(false);
            ApplyConfigureManager();
        }

        TASFile mFile;
        TASViewer mViewer;
        TASFlow mFlow;
        TASSlider mSlider;
        bool mIsHorizontalLayout;

        #region ui func

        // shortcut

        private void funcCommand_Menu_File_Open(object sender, ExecutedRoutedEventArgs e) => funcMenu_File_Open(sender, e);
        private void funcCommand_Menu_File_Save(object sender, ExecutedRoutedEventArgs e) => funcMenu_File_Save(sender, e);
        private void funcCommand_Menu_Display_Undo(object sender, ExecutedRoutedEventArgs e) => funcMenu_Display_Undo(sender, e);
        private void funcCommand_Menu_Display_Redo(object sender, ExecutedRoutedEventArgs e) => funcMenu_Display_Redo(sender, e);
        private void funcCommand_DataMenu_Cut(object sender, ExecutedRoutedEventArgs e) => funcDataMenu_Cut(sender, e);
        private void funcCommand_DataMenu_Copy(object sender, ExecutedRoutedEventArgs e) => funcDataMenu_Copy(sender, e);
        private void funcCommand_DataMenu_PasteAfter(object sender, ExecutedRoutedEventArgs e) => funcDataMenu_PasteAfter(sender, e);
        private void funcCommand_DataMenu_DeleteAfter(object sender, ExecutedRoutedEventArgs e) => funcDataMenu_DeleteAfter(sender, e);
        private void funcCommand_DataMenu_DeleteBefore(object sender, ExecutedRoutedEventArgs e) => funcDataMenu_DeleteBefore(sender, e);
        private void funcCanExeCmd_Menu_File_Open(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_File_Open.IsEnabled;
        private void funcCanExeCmd_Menu_File_Save(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_File_Save.IsEnabled;
        private void funcCanExeCmd_Menu_Display_Undo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_Display_Undo.IsEnabled;
        private void funcCanExeCmd_Menu_Display_Redo(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = uiMenu_Display_Redo.IsEnabled;
        private void funcCanExeCmd_DataMenu_Cut(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = mViewer != null && uiDataMenu_Cut.IsEnabled;
        private void funcCanExeCmd_DataMenu_Copy(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = mViewer != null && uiDataMenu_Copy.IsEnabled;
        private void funcCanExeCmd_DataMenu_PasteAfter(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = mViewer != null && uiDataMenu_PasteAfter.IsEnabled;
        private void funcCanExeCmd_DataMenu_DeleteAfter(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = mViewer != null && uiDataMenu_DeleteAfter.IsEnabled;
        private void funcCanExeCmd_DataMenu_DeleteBefore(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = mViewer != null && uiDataMenu_DeleteBefore.IsEnabled;

        // =========================== menu
        #region window menu

        private void funcMenu_Help_ReportBugs(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("https://github.com/yyc12345/BallanceTASEditor/issues");
        }

        private void funcMenu_Help_About(object sender, RoutedEventArgs e) {
            MessageBox.Show(I18NProcessor.GetI18N("code_MainWindow_Menu_Help_About"),
                I18NProcessor.GetI18N("code_Shared_ProgramName"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void funcMenu_File_Open(object sender, RoutedEventArgs e) {
            var file = DialogUtil.OpenFileDialog();
            if (file == "") return;
            OpenFile(file);
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
            if (!DialogUtil.ConfirmDialog(I18NProcessor.GetI18N("code_MainWindow_Menu_File_Close"))) return;

            mViewer.UpdateDataUI -= RefreshDataUI;
            mViewer.UpdateSelection -= RefreshSelection;
            mViewer.UpdateToolMode -= RefreshToolMode;

            mViewer.Dispose();
            mFile = null;
            mViewer = null;
            RefreshUI(false);
        }

        private void funcWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (!(mFile is null)) {
                if (!DialogUtil.ConfirmDialog(I18NProcessor.GetI18N("code_MainWindow_Closing"))) {
                    e.Cancel = true;
                }
            }
        }

        private void funcMenu_Display_ItemCount(object sender, RoutedEventArgs e) {
            int newvalue = 0;
            if (DialogUtil.InputNumber(I18NProcessor.GetI18N("code_MainWindow_Menu_Display_ItemCount"), 5, 30, ref newvalue)) {
                ChangeItemCount(newvalue);
            }
        }

        private void funcMenu_Display_OverwrittenPaste(object sender, RoutedEventArgs e) {
            uiMenu_Display_OverwrittenPaste.IsChecked = !uiMenu_Display_OverwrittenPaste.IsChecked;
            uiStatusbar_OverwrittenPaste.Visibility = uiMenu_Display_OverwrittenPaste.IsChecked ? Visibility.Visible : Visibility.Hidden;

            GlobalVariable.configManager.Configuration[ConfigManager.CfgNode_IsOverwrittenPaste] = uiMenu_Display_OverwrittenPaste.IsChecked.ToString();
            if (mViewer != null)
                mViewer.ChangeOverwrittenMode(uiMenu_Display_OverwrittenPaste.IsChecked);
        }

        private void funcMenu_Display_HorizontalLayout(object sender, RoutedEventArgs e) {
            uiMenu_Display_HorizontalLayout.IsChecked = !uiMenu_Display_HorizontalLayout.IsChecked;
            GlobalVariable.configManager.Configuration[ConfigManager.CfgNode_IsHorizonLayout] = uiMenu_Display_HorizontalLayout.IsChecked.ToString();
            ChangeLayout(uiMenu_Display_HorizontalLayout.IsChecked);
        }

        private void funcMenu_Display_Redo(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Redo);
        }

        private void funcMenu_Display_Undo(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Undo);
        }

        #endregion

        #region datamenu operation

        private void funcDataMenu_Set(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Set);
        }

        private void funcDataMenu_Unset(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Unset);
        }

        private void funcDataMenu_Cut(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Cut);
        }

        private void funcDataMenu_Copy(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Copy);
        }

        private void funcDataMenu_PasteAfter(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.PasteAfter);
        }

        private void funcDataMenu_PasteBefore(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.PasteBefore);
        }

        private void funcDataMenu_Delete(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.Delete);
        }

        private void funcDataMenu_DeleteAfter(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.DeleteAfter);
        }

        private void funcDataMenu_DeleteBefore(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.DeleteBefore);
        }

        private void funcDataMenu_AddAfter(object sender, RoutedEventArgs e) {
            mViewer.ProcessOperation(OperationEnum.AddAfter);
        }

        private void funcDataMenu_AddBefore(object sender, RoutedEventArgs e) {
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

            switch (e.Key) {
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
                    ChangeItemCount(newvalue - 1);
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
                    ChangeItemCount(newvalue + 1);
                } else {
                    // normally move
                    mSlider.MoveSliderManually(false, false, mViewer.GetItemCountInPage());
                }
            }
        }

        // drop file to open
        private void funcDrop_Drop(object sender, DragEventArgs e) {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            OpenFile(fileName);
        }

        private void funcDrop_DragEnter(object sender, DragEventArgs e) {
            // only accept one file
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                var arr = (System.Array)e.Data.GetData(DataFormats.FileDrop);
                if (arr.Length != 1) e.Effects = DragDropEffects.None;
                else e.Effects = DragDropEffects.Link;
            } else e.Effects = DragDropEffects.None;
        }

        #endregion

        private void ApplyConfigureManager() {
            var isOverwrittenPaste = bool.Parse(GlobalVariable.configManager.Configuration[ConfigManager.CfgNode_IsOverwrittenPaste]);
            var isHorizontalLayout = bool.Parse(GlobalVariable.configManager.Configuration[ConfigManager.CfgNode_IsHorizonLayout]);
            var itemCount = int.Parse(GlobalVariable.configManager.Configuration[ConfigManager.CfgNode_ItemCount]);

            uiMenu_Display_OverwrittenPaste.IsChecked = isOverwrittenPaste;
            uiMenu_Display_HorizontalLayout.IsChecked = isHorizontalLayout;
            uiStatusbar_OverwrittenPaste.Visibility = isOverwrittenPaste ? Visibility.Visible : Visibility.Hidden;
            if (mViewer != null) {
                mViewer.ChangeOverwrittenMode(isOverwrittenPaste);
                mViewer.ChangeListLength(itemCount);
                ChangeLayout(isHorizontalLayout);
            }
        }

        private void OpenFile(string file) {
#if DEBUG
#else
            try {
#endif
                mFile = new TASFile(file);

#if DEBUG
#else
            } catch {
                MessageBox.Show(I18NProcessor.GetI18N("code_MainWindow_Menu_File_Open_Fail"),
                    I18NProcessor.GetI18N("code_Shared_ProgramName"),
                    MessageBoxButton.OK, MessageBoxImage.Error);
                mFile = null;
                return;
            }
#endif

            mViewer = new TASViewer(mFile, mSlider, mFlow);

            mViewer.UpdateDataUI += RefreshDataUI;
            mViewer.UpdateSelection += RefreshSelection;
            mViewer.UpdateToolMode += RefreshToolMode;

            RefreshUI(true);
            ApplyConfigureManager(); // apply item count and overwritten item
            mViewer.ChangeToolMode(ToolMode.Cursor);
        }

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
                uiMenu_Display_HorizontalLayout.IsEnabled = true;
                uiMenu_Display_Undo.IsEnabled = true;
                uiMenu_Display_Redo.IsEnabled = true;

                uiStatusbar_Runtime_Mode.Visibility = Visibility.Visible;
                uiStatusbar_Runtime_PasteMode.Visibility = Visibility.Visible;
                uiStatusbar_Runtime_Selected.Visibility = Visibility.Visible;
                uiStatusbar_Runtime_Separator1.Visibility = Visibility.Visible;
                uiStatusbar_Runtime_Separator2.Visibility = Visibility.Visible;
            } else {
                uiEditorPanel.Visibility = Visibility.Collapsed;
                uiEditorNote.Visibility = Visibility.Visible;

                uiMenu_File_Open.IsEnabled = true;
                uiMenu_File_Save.IsEnabled = false;
                uiMenu_File_SaveAs.IsEnabled = false;
                uiMenu_File_Close.IsEnabled = false;

                uiMenu_Display_ItemCount.IsEnabled = false;
                uiMenu_Display_OverwrittenPaste.IsEnabled = false;
                uiMenu_Display_HorizontalLayout.IsEnabled = false;
                uiMenu_Display_Undo.IsEnabled = false;
                uiMenu_Display_Redo.IsEnabled = false;

                uiStatusbar_Runtime_Mode.Visibility = Visibility.Collapsed;
                uiStatusbar_Runtime_PasteMode.Visibility = Visibility.Collapsed;
                uiStatusbar_Runtime_Selected.Visibility = Visibility.Collapsed;
                uiStatusbar_Runtime_Separator1.Visibility = Visibility.Collapsed;
                uiStatusbar_Runtime_Separator2.Visibility = Visibility.Collapsed;
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

        private void ChangeItemCount(int count) {
            GlobalVariable.configManager.Configuration[ConfigManager.CfgNode_ItemCount] = count.ToString();
            mViewer.ChangeListLength(count);
        }

        private void ChangeLayout(bool isHorizontal) {
            if (isHorizontal == mIsHorizontalLayout) return;
            mIsHorizontalLayout = isHorizontal;

            // swap window size
            var swap = this.Width;
            this.Width = this.Height;
            this.Height = swap;
            // change self layout first
            uiLayoutContainer.RowDefinitions.Clear();
            uiLayoutContainer.ColumnDefinitions.Clear();
            if (isHorizontal) {
                UI.Util.GridRowAdder(uiLayoutContainer, new GridLength(1, GridUnitType.Star));
                UI.Util.GridRowAdder(uiLayoutContainer, GridLength.Auto);
            } else {
                UI.Util.GridColumnAdder(uiLayoutContainer, new GridLength(1, GridUnitType.Star));
                UI.Util.GridColumnAdder(uiLayoutContainer, GridLength.Auto);
            }
            UI.Util.SwapGridItemRC(uiTASFlowContainer);
            UI.Util.SwapGridItemRC(uiTASSliderContainer);

            // change sub layout
            mFlow.ChangeLayout(isHorizontal);
            mSlider.ChangeLayout(isHorizontal);
        }

    }
}
