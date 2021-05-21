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
        public TASViewer(TASFile file, Slider slider, TASFlow datagrid, TextBlock statusbar) {
            mFile = file;
            mSlider = slider;
            mDataGrid = datagrid;
            mStatusbar = statusbar;

            // restore slider
            mSlider.Minimum = 0;
            updateSliderRange();

            // init selection
            mSelectionHelp = new SelectionHelp();
            mSelectionHelp.SelectionChanged += funcSelectionHelp_SelectionChanged;

            // init data
            INVALID_FRAME_DATA = new FrameData(-1f, 0);
            mDataSource = new List<FrameDataDisplay>();
            mListLength = 0;

            // bind event and source
            mDataGrid.DataSources = mDataSource;
            mDataGrid.SelectionHelp = mSelectionHelp;

            mDataGrid.uiDataMenu_Set.Click += funcDataMenu_Set;
            mDataGrid.uiDataMenu_Unset.Click += funcDataMenu_Unset;
            mDataGrid.uiDataMenu_Copy.Click += funcDataMenu_Copy;
            mDataGrid.uiDataMenu_Delete.Click += funcDataMenu_Delete;
            mDataGrid.uiDataMenu_PasteAfter.Click += funcDataMenu_PasteAfter;
            mDataGrid.uiDataMenu_PasteBefore.Click += funcDataMenu_PasteBefore;
            mDataGrid.uiDataMenu_AddAfter.Click += funcDataMenu_AddAfter;
            mDataGrid.uiDataMenu_AddBefore.Click += funcDataMenu_AddBefore;
            mDataGrid.Click += funcDataMenu_Click;

            mSlider.ValueChanged += sliderValueChanged;

            // display data
            ChangeListLength(DATA_LIST_LENGTH);
        }

        public void Dispose() {
            mDataGrid.DataSources = null;

            mDataGrid.uiDataMenu_Set.Click -= funcDataMenu_Set;
            mDataGrid.uiDataMenu_Unset.Click -= funcDataMenu_Unset;
            mDataGrid.uiDataMenu_Copy.Click -= funcDataMenu_Copy;
            mDataGrid.uiDataMenu_Delete.Click -= funcDataMenu_Delete;
            mDataGrid.uiDataMenu_PasteAfter.Click -= funcDataMenu_PasteAfter;
            mDataGrid.uiDataMenu_PasteBefore.Click -= funcDataMenu_PasteBefore;
            mDataGrid.uiDataMenu_AddAfter.Click -= funcDataMenu_AddAfter;
            mDataGrid.uiDataMenu_AddBefore.Click -= funcDataMenu_AddBefore;
            mDataGrid.Click -= funcDataMenu_Click;

            mSlider.ValueChanged -= sliderValueChanged;
        }

        const int DATA_LIST_LENGTH = 15;
        FrameData INVALID_FRAME_DATA;
        TASFile mFile;
        Slider mSlider;
        TextBlock mStatusbar;
        TASFlow mDataGrid;
        SelectionHelp mSelectionHelp;
        int mListLength;
        List<FrameDataDisplay> mDataSource;

        private void sliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            long pos = Convert.ToInt64(Math.Floor(e.NewValue));
            mFile.Shift(pos);

            RefreshDisplay();
        }

        private void updateSliderRange() {
            long newSize = mFile.mFrameCount - 1;
            if (mSlider.Value > newSize)
                mSlider.Value = newSize;
            mSlider.Maximum = newSize;
        }

        private void funcSelectionHelp_SelectionChanged() {
            mDataGrid.RefreshDataMenu();
            mDataGrid.RefreshSelectionHighlight();
            OnStatusbarSelectionChanged();
        }

        private void OnStatusbarSelectionChanged() {
            var mode = mSelectionHelp.GetToolMode();

            switch (mode) {
                case ToolMode.Cursor:
                    if (mSelectionHelp.IsDataReady()) {
                        var data = mSelectionHelp.GetRange();
                        mStatusbar.Text = $"{data.start} - {data.end}";
                    } else if (mSelectionHelp.IsDataPartialReady()) {
                        var data2 = mSelectionHelp.GetPoint();
                        mStatusbar.Text = data2.ToString();
                    } else mStatusbar.Text = "-";
                    break;
                case ToolMode.Fill:
                    if (mSelectionHelp.IsDataReady()) {
                        var data3 = mSelectionHelp.GetRange();
                        mStatusbar.Text = $"{data3.start} - {data3.end}";
                    } else mStatusbar.Text = "-";
                    break;
                case ToolMode.Overwrite:
                    if (mSelectionHelp.IsDataReady()) {
                        var data4 = mSelectionHelp.GetPoint();
                        mStatusbar.Text = data4.ToString();
                    } else mStatusbar.Text = "-";
                    break;
            }
        }

        public void ChangeListLength(int newLen) {
            if (newLen < 5 || newLen > 30) return;
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
        }



        #region data menu

        private void funcDataMenu_AddBefore(object sender, RoutedEventArgs e) {
            if (!DialogUtil.AddItemDialog(out int count, out float deltaTime)) return;

            var pos = mSelectionHelp.GetPoint();
            mFile.Add(pos, count, deltaTime, true);
            RefreshDisplay();
        }

        private void funcDataMenu_AddAfter(object sender, RoutedEventArgs e) {
            if (!DialogUtil.AddItemDialog(out int count, out float deltaTime)) return;

            var pos = mSelectionHelp.GetPoint();
            mFile.Add(pos, count, deltaTime, false);
            RefreshDisplay();
        }

        private void funcDataMenu_PasteBefore(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void funcDataMenu_PasteAfter(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void funcDataMenu_Delete(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void funcDataMenu_Copy(object sender, RoutedEventArgs e) {
            throw new NotImplementedException();
        }

        private void funcDataMenu_Unset(object sender, RoutedEventArgs e) {
            mFile.Set(mSelectionHelp.GetFieldRange(), mSelectionHelp.GetRange(), false);
            RefreshDisplay();
        }

        private void funcDataMenu_Set(object sender, RoutedEventArgs e) {
            mFile.Set(mSelectionHelp.GetFieldRange(), mSelectionHelp.GetRange(), true);
            RefreshDisplay();
        }

        private void funcDataMenu_Click() {
            var data = mSelectionHelp.GetPoint();
            var field = (int)mSelectionHelp.GetPointField();
            mFile.Set(new SelectionRange(field, field), new SelectionRange(data, data), null);
            RefreshDisplay();
        }

        #endregion
    }
}
