using BallanceTASEditor.Core.TASStruct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.Core.FileOperation {
    public abstract class RevocableOperation {
        public RevocableOperation() {
            hasBeenDone = false;
        }

        private bool hasBeenDone;
        public virtual void Do(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            if (hasBeenDone) throw new Exception("Try to call operation.do when the operation has been done.");
            hasBeenDone = true;
        }

        public virtual void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            if (!hasBeenDone) throw new Exception("Try to call operation.undo when the operation has not been done.");
            hasBeenDone = false;
        }
    }


    public class SetOperation : RevocableOperation {
        private SelectionRange field;
        private SelectionRange absoluteRange;
        private bool? isSet;

        private uint internalOffset;
        private List<uint> changedItems;

        public SetOperation(SelectionRange _field, SelectionRange _absoluteRange, bool? _isSet) : base() {
            field = _field;
            absoluteRange = _absoluteRange;
            isSet = _isSet;

            // calc offset first for following operation
            internalOffset = 0;
            for (int i = (int)field.start; i <= (int)field.end; i++) {
                internalOffset |= ConstValue.Mapping[(FrameDataField)i];
            }

            changedItems = new List<uint>();
        }

        public override void Do(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Do(ref mMem, ref mPointer, ref mPointerIndex);
            if (mPointer == null) return;

            changedItems.Clear();
            foreach (var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                // backup item first
                changedItems.Add(item.Value.keystates);

                if (isSet == null) item.Value.ReverseKeyStates(internalOffset);
                else if (isSet == true) item.Value.SetKeyStates(internalOffset);
                else if (isSet == false) item.Value.UnsetKeyStates(internalOffset);
            }
        }

        public override void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            if (mPointer == null) return;

            int counter = 0;
            foreach (var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                // restore data
                item.Value.keystates = changedItems[counter];
                counter++;
            }
        }
    }

    public class RemoveOperation : RevocableOperation {
        private SelectionRange absoluteRange;

        private LinkedList<FrameData> removedItems;
        private LinkedListNode<FrameData> oldPointer;
        private long oldPointerIndex;
        private LinkedListNode<FrameData> removeStartNode;

        public RemoveOperation(SelectionRange _absoluteRange) : base() {
            absoluteRange = _absoluteRange;

            removedItems = new LinkedList<FrameData>();
        }

        public override void Do(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Do(ref mMem, ref mPointer, ref mPointerIndex);
            if (mPointer == null) return;

            // init backups list and backups 2 data
            // and backups remove start node(ps: if it is null, mean removed from head)
            removedItems.Clear();
            oldPointer = mPointer;
            oldPointerIndex = mPointerIndex;
            removeStartNode = absoluteRange.start == 0 ? null : mMem.FastGetNode(mPointer, mPointerIndex, absoluteRange.start - 1);

            // find proper pointer after remove first. but we do not apply it in there.
            // if state is true, it mean the deleted content is placed before pointer previously. we should consider pointer data and we should correct them.
            LinkedListNode<FrameData> newPointer;
            long newPointerIndex;
            if (mPointerIndex >= absoluteRange.start) {
                // if point within removed content, we need to shift it to the head of removed content, 
                // otherwise we only need to minus index with the length of removed content.
                if (absoluteRange.Within(mPointerIndex)) {
                    // this contains 3 situation
                    // if full delete, mPointer is null and mPointerIndex is invalid(with wrong data: -1)
                    // if delete from head, mPointer and mPointerIndex all are valid. but it is the tail of removed content
                    // otherwise, just find the head of removed content and shift to it.
                    if (absoluteRange.start == 0 && absoluteRange.end == mMem.Count - 1) {
                        // fully remove
                        newPointer = null;
                        newPointerIndex = -1;
                    } else if (absoluteRange.start == 0) {
                        // remove from head
                        newPointerIndex = 0;
                        newPointer = mMem.FastGetNode(mPointer, mPointerIndex, absoluteRange.end + 1);
                    } else {
                        // simple remove
                        newPointerIndex = absoluteRange.start - 1;
                        newPointer = mMem.FastGetNode(mPointer, mPointerIndex, absoluteRange.start - 1);
                    }
                } else {
                    newPointer = mPointer;
                    newPointerIndex = mPointerIndex - absoluteRange.GetCount();
                }
            } else {
                // not affected situation
                newPointer = mPointer;
                newPointerIndex = mPointerIndex;
            }

            // the real remove operation
            foreach (var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                mMem.Remove(item);
                removedItems.AddLast(item); // backups node;
            }

            // apply gotten new pointer
            mPointer = newPointer;
            mPointerIndex = newPointerIndex;

        }

        public override void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            // may recovered from empty list
            //if (mPointer == null) return;

            // re-insert data
            foreach (var item in removedItems.IterateFullReversed()) {
                removedItems.Remove(item);
                if (removeStartNode == null) {
                    // insert at first
                    mMem.AddFirst(item);
                } else {
                    // insert after this node
                    mMem.AddAfter(removeStartNode, item);
                }
            }

            // reset pointer
            mPointer = oldPointer;
            mPointerIndex = oldPointerIndex;
        }
    }

    public class AddOperation : RevocableOperation {
        private long absolutePos;
        private long count;
        private float deltaTime;
        private bool isAddBefore;

        private LinkedListNode<FrameData> addStartNode;
        private LinkedListNode<FrameData> oldPointer;
        private long oldPointerIndex;

        public AddOperation(long _absolutePos, long _count, float _deltaTime, bool _isAddBefore) : base() {
            absolutePos = _absolutePos;
            count = _count;
            deltaTime = _deltaTime;
            isAddBefore = _isAddBefore;
        }

        public override void Do(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Do(ref mMem, ref mPointer, ref mPointerIndex);
            if (count <= 0) return;

            // backups 2 data
            oldPointer = mPointer;
            oldPointerIndex = mPointerIndex;

            // real add operation
            if (mPointer == null) {
                // backups start pointer
                addStartNode = null;

                // add into blank list, absolutePos and isAddBefore parameters are invalid
                // specially process
                for (long i = 0; i < count; i++) {
                    mMem.AddFirst(new FrameData(deltaTime, 0));
                }
                mPointer = mMem.First;
                mPointerIndex = 0;
            } else {
                // normal add
                LinkedListNode<FrameData> node = mMem.FastGetNode(mPointer, mPointerIndex, absolutePos);
                // backups start pointer
                addStartNode = node;
                if (isAddBefore) {
                    for (long i = 0; i < count; i++) {
                        mMem.AddBefore(node, new FrameData(deltaTime, 0));
                    }
                } else {
                    for (long i = 0; i < count; i++) {
                        mMem.AddAfter(node, new FrameData(deltaTime, 0));
                    }
                }

                // if the items are added before pointer, the index should add with the count of added items
                // but pointer don't need to be shifted.
                if ((isAddBefore && mPointerIndex >= absolutePos) ||
                    (!isAddBefore && mPointerIndex > absolutePos))
                    mPointerIndex += count;
            }
        }

        public override void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            if (count <= 0) return;

            if (addStartNode == null) {
                // original state is blank
                // just clear mmem is ok
                mMem.Clear();
            } else {
                if (isAddBefore) {
                    for (long i = 0; i < count; i++) {
                        mMem.Remove(addStartNode.Previous);
                    }
                } else {
                    for (long i = 0; i < count; i++) {
                        mMem.Remove(addStartNode.Next);
                    }
                }
            }

            // re-set pointer
            mPointer = oldPointer;
            mPointerIndex = oldPointerIndex;
        }
    }

    public class InsertOperation : RevocableOperation {
        private long absolutePos;
        private LinkedList<FrameData> data;
        private bool isInsertBefore;
        private bool isOverwritten;

        private LinkedListNode<FrameData> addStartNode;
        private bool isBlankList;
        private LinkedListNode<FrameData> oldPointer;
        private long oldPointerIndex;

        // because insert including remove oeration(overwritten mode)
        // so we need include this for code re-use
        private RemoveOperation internalRemoveOper;

        private const long LINKEDLIST_HEAD = -1;
        private const long LINKEDLIST_TAIL = -2;

        public InsertOperation(long _absolutePos, LinkedList<FrameData> _data, bool _isInsertBefore, bool _isOverwritten) : base() {
            absolutePos = _absolutePos;
            data = _data;
            isInsertBefore = _isInsertBefore;
            isOverwritten = _isOverwritten;

        }

        public override void Do(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Do(ref mMem, ref mPointer, ref mPointerIndex);
            if (data.Count == 0) return;

            // because this oper have internal oper, so we need backup data after potential remove oper
            // so in there, no object need to be backuped

            // if the list is blank, overwritten is invalid, just normal add them.
            if (mPointer == null) {
                // backups
                oldPointer = mPointer;
                oldPointerIndex = mPointerIndex;
                addStartNode = null;
                isBlankList = true;

                foreach (var item in data.IterateFull()) {
                    mMem.AddFirst(item.Value);
                }
                mPointer = mMem.First;
                mPointerIndex = 0;
            } else {
                LinkedListNode<FrameData> node = mMem.FastGetNode(mPointer, mPointerIndex, absolutePos);

                // absolutePos is class member and shouldn't be changed.
                // but in overwritten mode, this value need to be changed so we create a temp value in there
                // to instead the fucntion of original variable
                var modifiedAbsolutePos = absolutePos;

                // if list is not a blank list, we should consider overwritten
                // if in overwritten mode, we need to overwrite data from selected item.
                // otherwise, not in overwritten mode, just normally add them just like add operation.
                if (isOverwritten) {
                    // in overwritten mode, if follwoing item is not enough to fufill the count of overwritten data
                    // normally add them
                    // we use delete and add method to do this

                    // now, try init internal remove oper if in overwritten mode
                    // first, we need compare the length of remained item located in mMem and the length of added item
                    // then construct remove oper
                    long remainLength;
                    if (isInsertBefore) remainLength = absolutePos + 1;
                    else remainLength = mMem.Count - absolutePos;

                    long dataLength = data.Count;
                    long expectedLength = dataLength > remainLength ? remainLength : dataLength;
                    long expectedPos;
                    if (isInsertBefore) expectedPos = absolutePos - expectedLength + 1;
                    else expectedPos = absolutePos + expectedLength - 1;

                    if (isInsertBefore)
                        internalRemoveOper = new RemoveOperation(new SelectionRange(expectedPos, absolutePos));
                    else
                        internalRemoveOper = new RemoveOperation(new SelectionRange(absolutePos, expectedPos));
                    
                    node = isInsertBefore ? node.Next : node.Previous;
                    internalRemoveOper.Do(ref mMem, ref mPointer, ref mPointerIndex);
                    // now, we can treat it as normal insert(without overwritten)
                    // but with one exception: absolutePos
                    // we need re calc absolutePos bucause we have called remove oper

                    if (isInsertBefore) {
                        if (node == null)
                            modifiedAbsolutePos = LINKEDLIST_TAIL;
                        else
                            modifiedAbsolutePos = absolutePos + 1 - expectedLength;
                    } else {
                        if (node == null)
                            modifiedAbsolutePos = LINKEDLIST_HEAD;
                        else
                            modifiedAbsolutePos -= 1;
                    }

                }
                // backups
                oldPointer = mPointer;
                oldPointerIndex = mPointerIndex;
                addStartNode = node;
                isBlankList = false;

                if (isInsertBefore) {
                    foreach (var item in data.IterateFull()) {
                        if (node == null)
                            mMem.AddLast(item.Value);
                        else
                            mMem.AddBefore(node, item.Value);
                    }
                } else {
                    foreach (var item in data.IterateFullReversed()) {
                        if (node == null)
                            mMem.AddFirst(item.Value);
                        else
                            mMem.AddAfter(node, item.Value);
                    }
                }

                if (modifiedAbsolutePos != LINKEDLIST_TAIL && modifiedAbsolutePos != LINKEDLIST_HEAD) {
                    if ((isInsertBefore && mPointerIndex >= modifiedAbsolutePos) ||
                        (!isInsertBefore && mPointerIndex > modifiedAbsolutePos))
                        mPointerIndex += data.Count;
                }
                else if (modifiedAbsolutePos == LINKEDLIST_HEAD)
                    mPointerIndex += data.Count;

                // remove have chance to remove entire list
                // so we need restore pointer in that situation
                if (mPointer == null) {
                    mPointer = mMem.First;
                    mPointerIndex = mPointer == null ? -1 : 0;
                }
            }
        }

        public override void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            if (data.Count == 0) return;

            if (isBlankList) {
                // original state is blank
                // just clear mmem is ok
                mMem.Clear();

                // re-set pointer
                mPointer = oldPointer;
                mPointerIndex = oldPointerIndex;
            } else {
                // in overwrite or not in overwrite mode, we all need to remove inserted data first
                if (isInsertBefore) {
                    for (long i = 0; i < data.Count; i++) {
                        if (addStartNode == null)
                            mMem.RemoveLast();
                        else
                            mMem.Remove(addStartNode.Previous);
                    }
                } else {
                    for (long i = 0; i < data.Count; i++) {
                        if (addStartNode == null)
                            mMem.RemoveFirst();
                        else
                            mMem.Remove(addStartNode.Next);
                    }
                }

                // re-set pointer
                mPointer = oldPointer;
                mPointerIndex = oldPointerIndex;

                // if we use overwrite mode, we need re-add lost data
                if (isOverwritten) {
                    internalRemoveOper.Undo(ref mMem, ref mPointer, ref mPointerIndex);
                }
            }
        }
    }

}
