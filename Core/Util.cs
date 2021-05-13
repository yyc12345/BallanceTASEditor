using BallanceTASEditor.Core.TASStruct;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.Core {
    public static class Util {
        public static bool ToBool(this UInt32 num) {
            return (num != 0);
        }
        public static UInt32 ToUInt32(this bool b) {
            return (UInt32)(b ? 1 : 0);
        }
        //public static void RemoveRange(this ModifiedObservableCollection<FrameData> list, int index, int count) {
        //    if (index >= list.Count) return;
        //    if (index + count > list.Count) count = list.Count - index;
        //    for (int i = 0; i < count; i++) list.RemoveAt(index);
        //}
    }
}
