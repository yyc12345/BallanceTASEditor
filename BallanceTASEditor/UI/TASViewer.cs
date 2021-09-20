using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BallanceTASEditor.Core;
using BallanceTASEditor.Core.TASStruct;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;

namespace BallanceTASEditor.UI {
    public class TASViewer : IDisposable {
        public TASViewer(TASFile file, TASSlider slider, TASFlow datagrid) {
            mFile = file;
            mSlider = slider;
            mDataGrid = datagrid;

            // restore slider
            mSlider.UpdateRange(mFile);

            // init selection
            mSelectionHelp = new SelectionHelp();
            mSelectionHelp.SelectionChanged += funcSelectionHelp_SelectionChanged;

            // init data
            INVALID_FRAME_DATA = new FrameData(-1f, 0);
            mDataSource = new List<FrameDataDisplay>();
            mListLength = 0;
            mOverwrittenPaste = false;

            // bind event and source
            mDataGrid.DataSources = mDataSource;
            mDataGrid.SelectionHelp = mSelectionHelp;

            mDataGrid.Click += funcDataMenu_Click;
            mSlider.ValueChanged += funcSlider_ValueChanged;
        }

        public void Dispose() {
            mDataGrid.DataSources = null;

            mDataGrid.Click -= funcDataMenu_Click;
            mSlider.ValueChanged -= funcSlider_ValueChanged;
        }

        //const int DATA_LIST_LENGTH = 15;
        FrameData INVALID_FRAME_DATA;
        TASFile mFile;
        TASSlider mSlider;
        TASFlow mDataGrid;
        SelectionHelp mSelectionHelp;
        int mListLength;
        List<FrameDataDisplay> mDataSource;
        bool mOverwrittenPaste;

        #region self event
        public event Action<bool, bool, bool> UpdateDataUI;
        public event Action<ToolMode> UpdateToolMode;
        public event Action<SelectionHelp> UpdateSelection;

        private void OnUpdateDataUI() {
            ToolMode mode = mSelectionHelp.GetToolMode();
            bool showCursorPasteAddDeleteOne = mode == ToolMode.Cursor && mSelectionHelp.IsDataPartialReady();
            bool showFill = mode == ToolMode.Fill && mSelectionHelp.IsDataReady();
            bool showCursorCopyDelete = mode == ToolMode.Cursor && mSelectionHelp.IsDataReady();

            UpdateDataUI?.Invoke(showCursorPasteAddDeleteOne, showFill, showCursorCopyDelete);
        }

        private void OnUpdateToolMode() {
            UpdateToolMode?.Invoke(mSelectionHelp.GetToolMode());
        }

        private void OnUpdateSelection() {
            UpdateSelection?.Invoke(mSelectionHelp);
        }

        #endregion

        #region process event

        private void funcSlider_ValueChanged(long pos) {
            mFile.Shift(pos);
            RefreshDisplay();
        }

        private void funcSelectionHelp_SelectionChanged() {
            mDataGrid.RefreshSelectionHighlight();
            OnUpdateDataUI();
            OnUpdateSelection();
        }

        private void funcDataMenu_Click() {
            var data = mSelectionHelp.GetPoint();
            var field = (int)mSelectionHelp.GetPointField();
            mFile.Set(new SelectionRange(field, field), new SelectionRange(data, data), null);
            RefreshDisplay();
        }

        #endregion

        public void ChangeOverwrittenMode(bool isOverwritten) {
            mOverwrittenPaste = isOverwritten;
        }

        public void ChangeListLength(int newLen) {
            if (newLen < 5 || newLen > 200) return;
            int offset = newLen - mListLength;
            int abs = Math.Abs(offset);
            if (offset == 0) return;

            // change mDataSource first

            if (offset > 0) {
                for (int i = 0; i < abs; i++) {
                    mDataSource.Add(new FrameDataDisplay(0, INVALID_FRAME_DATA));
                }
            } else {
                mDataSource.RemoveRange(newLen, abs);
            }

            // then change viewer control
            mDataGrid.SetItemCount(newLen);

            // apply new value
            mListLength = newLen;

            // then refresh
            RefreshDisplay();
        }

        public void RefreshDisplay() {
            mFile.Get(mDataSource, mListLength);
            mDataGrid.RefreshDataSources();
            mDataGrid.RefreshSelectionHighlight();
        }

        public void ChangeToolMode(ToolMode mode) {
            mSelectionHelp.SetMode(mode);
            OnUpdateToolMode();
        }

        public int GetItemCountInPage() {
            return mListLength;
        }

        public void ProcessOperation(OperationEnum oper) {
            switch (oper) {
                case OperationEnum.Set:
                case OperationEnum.Unset: {
                        mFile.Set(mSelectionHelp.GetFieldRange(), mSelectionHelp.GetRange(), oper == OperationEnum.Set);
                        RefreshDisplay();
                    }
                    break;
                case OperationEnum.Cut: {
                        // cut is a hybrid operation, first, do copy
                        // then delete selected item
                        // due to copy is not affect TASFile and only delete oper affect it
                        // so this is a revocable oper
                        var data = new LinkedList<FrameData>();
                        mFile.Copy(mSelectionHelp.GetRange(), data);
                        if (!ClipboardUtil.SetFrameData(data)) {
                            MessageBox.Show("Fail to cut due to unknow reason!");
                            break;  // if fail to cut, do not delete selected items.
                        }

                        // do delete
                        mFile.Remove(mSelectionHelp.GetRange());
                        mSelectionHelp.Reset();
                        mSlider.UpdateRange(mFile);
                        RefreshDisplay();
                    }
                    break;
                case OperationEnum.Copy: {
                        var data = new LinkedList<FrameData>();
                        mFile.Copy(mSelectionHelp.GetRange(), data);
                        if (!ClipboardUtil.SetFrameData(data))
                            MessageBox.Show("Fail to copy due to unknow reason!");
                    }
                    break;
                case OperationEnum.PasteAfter:
                case OperationEnum.PasteBefore: {
                        var data = new LinkedList<FrameData>();
                        if (ClipboardUtil.GetFrameData(data)) {
                            mFile.Insert(mSelectionHelp.GetPoint(), data, oper == OperationEnum.PasteBefore, mOverwrittenPaste);
                            mSelectionHelp.Reset();
                            mSlider.UpdateRange(mFile);
                            RefreshDisplay();
                        } else MessageBox.Show("Fail to paste due to unknow reason or blank clipboard!");
                    }
                    break;
                case OperationEnum.Delete: {
                        mFile.Remove(mSelectionHelp.GetRange());
                        mSelectionHelp.Reset();
                        mSlider.UpdateRange(mFile);
                        RefreshDisplay();
                    }
                    break;
                case OperationEnum.DeleteAfter:
                case OperationEnum.DeleteBefore: {
                        var pos = mSelectionHelp.GetPoint();
                        if (oper == OperationEnum.DeleteBefore) pos -= 1;   // delete after mean delete current selected item
                        if (pos < 0 || pos >= mFile.mFrameCount) return;

                        // only delete before need shift selection
                        // delete before couldn't cause empty list, so we just need to directly shift
                        if (oper == OperationEnum.DeleteBefore)
                            mSelectionHelp.ShiftTo(false);
                        // also, if we use delete after and delete the tail of item list, we also need to shift pos(use `else if` to prevent double shift)
                        else if (oper == OperationEnum.DeleteAfter && pos == mFile.mFrameCount) {
                            // but delete after may cause empty list error(delete the item within only 1 item list)
                            // so we need prevent this situation
                            if (mFile.mFrameCount == 1) mSelectionHelp.Reset(); //yes, reset selection to prevent error
                            else mSelectionHelp.ShiftTo(false); //no, shift selection.
                        }

                        // do real operation
                        mFile.Remove(new SelectionRange(pos, pos));

                        mSlider.UpdateRange(mFile);
                        RefreshDisplay();
                    }
                    break;
                case OperationEnum.AddAfter:
                case OperationEnum.AddBefore: {
                        if (!DialogUtil.AddItemDialog(out int count, out float deltaTime)) return;

                        var pos = mSelectionHelp.GetPoint();
                        mFile.Add(pos, count, deltaTime, oper == OperationEnum.AddBefore);
                        mSelectionHelp.Reset();
                        mSlider.UpdateRange(mFile);
                        RefreshDisplay();
                    }
                    break;
                case OperationEnum.Undo: {
                        mFile.Undo();
                        mSelectionHelp.Reset();
                        mSlider.UpdateRange(mFile);
                        RefreshDisplay();
                    }
                    break;
                case OperationEnum.Redo: {
                        mFile.Redo();
                        mSelectionHelp.Reset();
                        mSlider.UpdateRange(mFile);
                        RefreshDisplay();
                    }
                    break;
            }
        }

    }
}
