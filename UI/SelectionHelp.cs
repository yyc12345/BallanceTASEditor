using BallanceTASEditor.Core.TASStruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BallanceTASEditor.Core;

namespace BallanceTASEditor.UI {
    public class SelectionHelp {
        public SelectionHelp() {
            SetMode(ToolMode.Cursor);
        }

        public event Action SelectionChanged;

        ToolMode mMode;
        FrameDataField mStartField;
        FrameDataField mEndField;
        long mStart;
        bool mIsStartConfirmed;
        long mEnd;
        bool mIsEndConfirmed;

        public void SetMode(ToolMode mode) {
            switch (mode) {
                case ToolMode.Cursor:
                    mStart = 0;
                    mEnd = 0;
                    mIsStartConfirmed = false;
                    mIsEndConfirmed = false;
                    break;
                case ToolMode.Fill:
                    mStartField = FrameDataField.Key_Up;
                    mEndField = FrameDataField.Key_Up;
                    mStart = 0;
                    mEnd = 0;
                    mIsStartConfirmed = false;
                    mIsEndConfirmed = false;
                    break;
                case ToolMode.Overwrite:
                    mStartField = FrameDataField.Key_Up;
                    mStart = 0;
                    mIsStartConfirmed = false;
                    break;
            }

            mMode = mode;
            OnSelectionChanged();
        }

        public void FirstClick(long index, FrameDataField field) {
            mStartField = field;
            mStart = index;
            mIsStartConfirmed = true;
            mIsEndConfirmed = false;

            OnSelectionChanged();
        }

        public void LastClick(long index, FrameDataField field) {
            if (mMode == ToolMode.Overwrite) return;
            if (!mIsStartConfirmed) return;

            mEndField = field;
            mEnd = index;
            mIsEndConfirmed = true;
            OnSelectionChanged();
        }

        public void Reset() {
            // reuse set mode to reset
            SetMode(mMode);
        }

        public SelectionRange GetRange() {
            if (mMode == ToolMode.Overwrite) throw new Exception("Read with wrong mode.");
            if (!(mIsStartConfirmed && mIsEndConfirmed)) throw new Exception("Data is not ready to read");
            return new SelectionRange(mStart, mEnd);
        }

        public SelectionRange GetFieldRange() {
            if (mMode != ToolMode.Fill) throw new Exception("Read with wrong mode.");
            if (!(mIsStartConfirmed && mIsEndConfirmed)) throw new Exception("Data is not ready to read");
            return new SelectionRange((int)mStartField, (int)mEndField);
        }

        public long GetPoint() {
            if (mMode == ToolMode.Fill) throw new Exception("Read with wrong mode.");
            if (!mIsStartConfirmed) throw new Exception("Data is not ready to read");

            if (mMode == ToolMode.Overwrite) return mStart;
            else {
                // cursor mode
                if (mIsStartConfirmed) return mStart;
                else throw new Exception("Data is not ready to read");
            }
        }

        public FrameDataField GetPointField() {
            if (mMode != ToolMode.Overwrite) throw new Exception("Read with wrong mode.");
            if (!mIsStartConfirmed) throw new Exception("Data is not ready to read");

            return mStartField;
        }

        public bool IsDataReady() {
            switch (mMode) {
                case ToolMode.Cursor:
                case ToolMode.Fill:
                    return (mIsStartConfirmed && mIsEndConfirmed);
                case ToolMode.Overwrite:
                    return mIsStartConfirmed;
            }
            return false;
        }

        public bool IsDataPartialReady() {
            return (mMode == ToolMode.Cursor && mIsStartConfirmed && !mIsEndConfirmed);
        }

        public ToolMode GetToolMode() {
            return mMode;
        }

        private void OnSelectionChanged() {
            SelectionChanged?.Invoke();
        }

    }

}
