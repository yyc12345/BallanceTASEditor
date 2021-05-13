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
        public TASViewer(TASFile file, Slider slider, TASFlow datagrid) {
            mFile = file;
            mSlider = slider;
            mDataGrid = datagrid;

            // restore slider
            mSlider.Minimum = 0;
            updateSliderRange();

            // init data
            mPosition = 0;
            mDataSource = new List<FrameDataDisplay>();
            INVALID_FRAME_DATA = new FrameData(-1f, 0);
            for (int i = 0; i < DATA_LIST_LENGTH; i++) {
                mDataSource.Add(new FrameDataDisplay(0, INVALID_FRAME_DATA));
            }
            mFile.Get(mDataSource, 0, DATA_LIST_LENGTH);

            // bind event and source
            mDataGrid.SetItemCount(DATA_LIST_LENGTH);
            mDataGrid.DataSources = mDataSource;
            mDataGrid.RefreshDataSources();
            mSlider.ValueChanged += sliderValueChanged;
        }
        public void Dispose() {
            mDataGrid.DataSources = null;
            mSlider.ValueChanged -= sliderValueChanged;
        }

        const int DATA_LIST_LENGTH = 15;
        FrameData INVALID_FRAME_DATA;
        TASFile mFile;
        Slider mSlider;
        TASFlow mDataGrid;
        long mPosition;
        List<FrameDataDisplay> mDataSource;

        private void sliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            long pos = Convert.ToInt64(Math.Floor(e.NewValue));
            long offset = pos - mPosition;
            mFile.Shift(offset);
            mFile.Get(mDataSource, pos, DATA_LIST_LENGTH);

            mPosition = pos;
            mDataGrid.RefreshDataSources();
        }

        private void updateSliderRange() {
            long newSize = mFile.mFrameCount - 1;
            if (mSlider.Value > newSize)
                mSlider.Value = newSize;
            mSlider.Maximum = newSize;
        }
    }
}
