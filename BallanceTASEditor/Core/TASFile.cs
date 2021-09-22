using BallanceTASEditor.Core.FileOperation;
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
            mPointerIndex = mPointer == null ? -1 : 0;

            mRedoStack = new LimitedStack<RevocableOperation>();
            mUndoStack = new LimitedStack<RevocableOperation>();
        }

        public string mFilename { get; private set; }
        public long mFrameCount { get { return mMem.Count; } }
        LinkedList<FrameData> mMem;
        LinkedListNode<FrameData> mPointer;
        long mPointerIndex;

        LimitedStack<RevocableOperation> mRedoStack;
        LimitedStack<RevocableOperation> mUndoStack;

        public bool IsEmpty() {
            return (mPointer == null);
        }

        public long GetPointerIndex() {
            // return invalid data to prevent error
            if (mPointer == null) return -1;//throw new Exception("Data is not ready");
            return mPointerIndex;
        }

        public void Shift(long absoluteIndex) {
            if (mPointer == null) return;

            mPointer = mMem.FastGetNode(mPointer, mPointerIndex, absoluteIndex);
            mPointerIndex = absoluteIndex;
        }

        public void Get(List<FrameDataDisplay> container, int count) {
            // no item. clean container
            if (mPointer == null) {
                for (int j = 0; j < count; j++) {
                    container[j].isEnable = false;
                }
                return;
            }

            // fill container
            var cachePointer = mPointer;
            var startIndex = mPointerIndex;
            int i;
            for (i = 0; i < count && cachePointer != null; i++, startIndex++) {
                container[i].Reload(startIndex, cachePointer.Value);
                container[i].isEnable = true;
                cachePointer = cachePointer.Next;
            }
            for (; i < count; i++) {
                container[i].isEnable = false;
            }
        }

        // if isSet is null, mean flip state
        public void Set(SelectionRange field, SelectionRange absoluteRange, bool? isSet) {
            var oper = new SetOperation(field, absoluteRange, isSet);
            oper.Do(ref mMem, ref mPointer, ref mPointerIndex);
            mUndoStack.Push(oper);
            mRedoStack.Clear();
            /*
            if (mPointer == null) return;

            uint offset = 0;
            for(int i = (int)field.start; i <= (int)field.end; i++) {
                offset |= ConstValue.Mapping[(FrameDataField)i];
            }
            foreach(var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                if (isSet == null) item.Value.ReverseKeyStates(offset);
                else if (isSet == true) item.Value.SetKeyStates(offset);
                else if (isSet == false) item.Value.UnsetKeyStates(offset);
            }
            */
        }

        public void Remove(SelectionRange absoluteRange) {
            var oper = new RemoveOperation(absoluteRange);
            oper.Do(ref mMem, ref mPointer, ref mPointerIndex);
            mUndoStack.Push(oper);
            mRedoStack.Clear();
            /*
            if (mPointer == null) return;

            // remove
            foreach(var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                mMem.Remove(item);
            }

            // correct index data
            // if state is true, it mean the deleted content is placed before pointer previously.
            // so we need shift the pointer to the head of selection range.
            // and we should consider 2 situations, the full delete of LinkedList and delete from head
            if (mPointerIndex >= absoluteRange.start) {
                var newIndex = absoluteRange.start - 1;
                if (newIndex < 0) {
                    // this contains 2 situation
                    // if full delete, mPointer is null and mPointerIndex is invalid(with wrong data: 0)
                    // if delete from head, mPointer and mPointerIndex all are valid.
                    mPointer = mMem.First;
                    mPointerIndex = 0;
                } else {
                    mPointer = mMem.FastGetNode(mPointer, mPointerIndex, newIndex);
                    mPointerIndex = newIndex;
                }
            }
            */
        }

        public void Add(long absolutePos, long count, float deltaTime, bool isAddBefore) {
            var oper = new AddOperation(absolutePos, count, deltaTime, isAddBefore);
            oper.Do(ref mMem, ref mPointer, ref mPointerIndex);
            mUndoStack.Push(oper);
            mRedoStack.Clear();
            /*
            if (count <= 0) return;

            if (mPointer == null) {
                // add into blank list, absolutePos and isAddBefore parameters are invalid
                // specially process
                for(long i = 0; i < count; i++) {
                    mMem.AddFirst(new FrameData(deltaTime, 0));
                }
                mPointer = mMem.First;
                mPointerIndex = 0;
            } else {
                // normal add
                // normal add doesn't affect pointer
                LinkedListNode<FrameData> node = mMem.FastGetNode(mPointer, mPointerIndex, absolutePos);
                if (isAddBefore) {
                    for (long i = 0; i < count; i++) {
                        mMem.AddBefore(node, new FrameData(deltaTime, 0));
                    }
                } else {
                    for (long i = 0; i < count; i++) {
                        mMem.AddAfter(node, new FrameData(deltaTime, 0));
                    }
                }
            }
            */
        }

        public void Insert(long absolutePos, LinkedList<FrameData> data, bool isInsertBefore, bool isOverwritten) {
            var oper = new InsertOperation(absolutePos, data, isInsertBefore, isOverwritten);
            oper.Do(ref mMem, ref mPointer, ref mPointerIndex);
            mUndoStack.Push(oper);
            mRedoStack.Clear();
            /*
            if (data.Count == 0) return;

            // the same process route with add function
            if (mPointer == null) {
                foreach (var item in data.IterateFull()) {
                    mMem.AddFirst(item.Value);
                }
                mPointer = mMem.First;
                mPointerIndex = 0;
            } else {
                LinkedListNode<FrameData> node = mMem.FastGetNode(mPointer, mPointerIndex, absolutePos);
                if (isInsertBefore) {
                    foreach (var item in data.IterateFull()) {
                        mMem.AddBefore(node, item.Value);
                    }
                } else {
                    foreach (var item in data.IterateFullReversed()) {
                        mMem.AddAfter(node, item.Value);
                    }
                }
            }
            */
        }

        public void Redo() {
            if (mRedoStack.IsEmpty()) return;
            var oper = mRedoStack.Pop();
            oper.Do(ref mMem, ref mPointer, ref mPointerIndex);
            mUndoStack.Push(oper);
        }

        public void Undo() {
            if (mUndoStack.IsEmpty()) return;
            var oper = mUndoStack.Pop();
            oper.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            mRedoStack.Push(oper);
        }

        public void Copy(SelectionRange absoluteRange, LinkedList<FrameData> data) {
            if (mPointer == null) return;

            foreach (var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
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


#if DEBUG
        // following code only should be used in debug mode and served for testbench
        public TASFile(LinkedList<FrameData> items) {
            mFilename = "";
            mMem = items;
            mPointer = mMem.First;
            mPointerIndex = mPointer == null ? -1 : 0;

            mRedoStack = new LimitedStack<RevocableOperation>();
            mUndoStack = new LimitedStack<RevocableOperation>();
        }

        public string Output2TestString() {
            StringBuilder sb = new StringBuilder();

            if (mPointer == null) sb.Append("null;");
            else sb.Append($"{mPointer.Value.keystates.ToString()};");
            sb.Append($"{mPointerIndex};");

            foreach (var item in mMem.IterateFull()) {
                sb.Append(item.Value.keystates.ToString());
                sb.Append(",");
            }
            return sb.ToString();
        }
#endif

    }
}
