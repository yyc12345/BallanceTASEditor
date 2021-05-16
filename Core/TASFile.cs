using BallanceTASEditor.Core.TASStruct;
using BallanceTASEditor.UI;
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

        public void Get(List<FrameDataDisplay> container, long startIndex, int count) {
            // no item. clean container
            if (mPointer == null) {
                for(int j = 0; j < count; j++) {
                    container[j].isEnable = false;
                }
                return;
            }

            // fill container
            var cachePointer = mPointer;
            int i;
            for(i = 0; i < count && cachePointer != null; i++, startIndex++) {
                container[i].Reload(startIndex, cachePointer.Value);
                container[i].isEnable = true;
                cachePointer = cachePointer.Next;
            }
            for(; i < count; i++) {
                container[i].isEnable = false;
            }
        }

        // if isSet is null, mean flip state
        public void Set(SelectionRange field, SelectionRange relativeRange, bool? isSet) {
            if (mPointer == null) return;

            var cachePointer = mPointer;
            uint offset = 0;
            for(int i = (int)field.start; i <= (int)field.end; i++) {
                offset |= ConstValue.Mapping[(FrameDataField)i];
            }
            foreach(var item in mMem.IterateWithSelectionRange(relativeRange, mPointer)) {
                if (isSet == null) item.Value.ReverseKeyStates(offset);
                else if (isSet == true) item.Value.SetKeyStates(offset);
                else if (isSet == false) item.Value.UnsetKeyStates(offset);
            }
        }

        public void Remove(SelectionRange relativeRange) {
            if (mPointer == null) return;

            mMem.RemoveWithSelectionRange(relativeRange, mPointer);
            // todo: fix pointer point to invalid item
        }

        public void Add(long relativePos, bool isAddBefore) {
            
        }

        public void Insert(long relativePos, LinkedList<FrameData> data, bool isInsertBefore) {
            if (mPointer == null) return;

        }

        public void Copy(SelectionRange relativeRange, LinkedList<FrameData> data) {
            if (mPointer == null) return;

            foreach (var item in mMem.IterateWithSelectionRange(relativeRange, mPointer)) {
                data.AddLast(item.Value);
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
