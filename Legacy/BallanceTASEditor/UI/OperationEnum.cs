using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.UI {
    public enum OperationEnum {
        Set,
        Unset,
        Cut,
        Copy,
        PasteAfter,
        PasteBefore,
        Delete,
        DeleteAfter,
        DeleteBefore,
        AddAfter,
        AddBefore,
        Undo,
        Redo
    }
}
