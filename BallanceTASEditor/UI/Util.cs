using BallanceTASEditor.Core.TASStruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.UI {


    public enum ToolMode {
        Cursor,
        Fill,
        Overwrite
    }

    public struct CellPosition {
        public CellPosition(int column, FrameDataField field) {
            this.column = column;
            this.field = field;
        }
        public int column;
        public FrameDataField field;
    }
}
