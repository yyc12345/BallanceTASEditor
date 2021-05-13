using BallanceTASEditor.Core.TASStruct;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.Core {
    public class TASFile {
        public TASFile(string filename) {
            mFilename = filename;
            mMem = new LinkedList<FrameData>();
            var fs = new FileStream(mFilename, FileMode.Open, FileAccess.Read, FileShare.Read);
            ZlibUtil.DecompressTAS(mMem, fs);
            fs.Close();
            fs.Dispose();
            mPointer = mMem.First;
        }

        public string mFilename { get; private set; }
        public long mFrameCount { get { return mMem.Count; } }
        LinkedList<FrameData> mMem;
        LinkedListNode<FrameData> mPointer;

        public void Shift(long shiftNum) {
            if (mPointer == null) return;
            if (shiftNum == 0) return;
            var absNum = Math.Abs(shiftNum);
            if (shiftNum > 0) {
                for(long num = 0; num < absNum && mPointer.Next != null; num++) {
                    mPointer = mPointer.Next;
                }
            } else {
                for (long num = 0; num < absNum && mPointer.Previous != null; num++) {
                    mPointer = mPointer.Previous;
                }
            }
        }

        public int Get(ObservableCollection<FrameDataDisplay> container, long startIndex, int count) {
            if (mPointer == null) return 0;
            var cachePointer = mPointer;
            int i;
            for(i = 0; i < count && cachePointer != null; i++, startIndex++) {
                container[i].Reload(startIndex, cachePointer.Value);
                cachePointer = cachePointer.Next;
            }
            return i;
        }

        public void Set(FrameDataField field, long prevRange, long nextRange, bool isSet) {
            if (mPointer == null) return;

            var cachePointer = mPointer;
            var offset = ConstValue.Mapping[field];
            for (long i = 0; i < nextRange && cachePointer != null; i++) {
                if (isSet) cachePointer.Value.SetKeyStates(offset);
                else cachePointer.Value.UnsetKeyStates(offset);
                cachePointer = cachePointer.Next;
            }
            for (long i = 0; i < prevRange && cachePointer != null; i++) {
                if (isSet) cachePointer.Value.SetKeyStates(offset);
                else cachePointer.Value.UnsetKeyStates(offset);
                cachePointer = cachePointer.Previous;
            }
        }

        public void Save() {
            var fs = new FileStream(mFilename, FileMode.Create, FileAccess.Write, FileShare.None);
            ZlibUtil.CompressTAS(mMem, fs);
            fs.Close();
            fs.Dispose();
        }

        public void SaveAs(string newfile) {
            mFilename = newfile;
            Save();
        }
    }
}
