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

        public SetOperation(SelectionRange _field, SelectionRange _absoluteRange, bool? _isSet) : base() {
            field = _field;
            absoluteRange = _absoluteRange;
            isSet = _isSet;

            // calc offset first for following operation
            internalOffset = 0;
            for (int i = (int)field.start; i <= (int)field.end; i++) {
                internalOffset |= ConstValue.Mapping[(FrameDataField)i];
            }
        }

        public override void Do(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Do(ref mMem, ref mPointer, ref mPointerIndex);
            if (mPointer == null) return;

            foreach (var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                if (isSet == null) item.Value.ReverseKeyStates(internalOffset);
                else if (isSet == true) item.Value.SetKeyStates(internalOffset);
                else if (isSet == false) item.Value.UnsetKeyStates(internalOffset);
            }
        }

        public override void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            if (mPointer == null) return;

            foreach (var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                // just like do operation, but switch set and unset operation
                if (isSet == null) item.Value.ReverseKeyStates(internalOffset);
                else if (isSet == true) item.Value.UnsetKeyStates(internalOffset);
                else if (isSet == false) item.Value.SetKeyStates(internalOffset);
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
            removeStartNode = absoluteRange.start == 1 ? null : mMem.FastGetNode(mPointer, mPointerIndex, absoluteRange.start - 1);

            // find proper pointer after remove first. but we do not apply it in there.
            // if state is true, it mean the deleted content is placed before pointer previously. we should consider pointer data and we should correct them.
            LinkedListNode <FrameData> newPointer;
            long newPointerIndex;
            if (mPointerIndex >= absoluteRange.start) {
                // if point within removed content, we need to shift it to the head of removed content, 
                // otherwise we only need to minus index with the length of removed content.
                if (absoluteRange.Within(mPointerIndex)) {
                    // this contains 3 situation
                    // if full delete, mPointer is null and mPointerIndex is invalid(with wrong data: 0)
                    // if delete from head, mPointer and mPointerIndex all are valid. but it is the tail of removed content
                    // otherwise, just find the head of removed content and shift to it.
                    if (absoluteRange.start == 1 && absoluteRange.end == mMem.Count) {
                        // fully remove
                        newPointer = null;
                        newPointerIndex = 0;
                    } else if (absoluteRange.start == 1) {
                        // remove from head
                        newPointerIndex = absoluteRange.end + 1;
                        newPointer = mMem.FastGetNode(mPointer, mPointerIndex, newPointerIndex);
                    } else {
                        // simple remove
                        newPointerIndex = absoluteRange.start - 1;
                        newPointer = mMem.FastGetNode(mPointer, mPointerIndex, newPointerIndex);
                    }
                } else {
                    newPointer = mPointer;
                    newPointerIndex = mPointerIndex - absoluteRange.GetCount() + 1;
                }
            } else {
                // not affected situation
                newPointer = mPointer;
                newPointerIndex = mPointerIndex;
            }

            // the real remove operation
            foreach (var item in mMem.IterateWithSelectionRange(absoluteRange, mPointer, mPointerIndex)) {
                removedItems.AddLast(item); // backups node first;
                mMem.Remove(item);
            }

            // apply gotten new pointer
            mPointer = newPointer;
            mPointerIndex = newPointerIndex;

        }

        public override void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            if (mPointer == null) return;

            // re-insert data
            foreach (var item in removedItems.IterateFullReversed()) {
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
                if (mPointerIndex > absolutePos)
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
        private LinkedList<FrameData> oldItems;
        private LinkedListNode<FrameData> oldPointer;
        private long oldPointerIndex;

        public InsertOperation(long _absolutePos, LinkedList<FrameData> _data, bool _isInsertBefore, bool _isOverwritten) : base() {
            absolutePos = _absolutePos;
            data = _data;
            isInsertBefore = _isInsertBefore;
            isOverwritten = _isOverwritten;

            oldItems = new LinkedList<FrameData>();
        }

        public override void Do(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Do(ref mMem, ref mPointer, ref mPointerIndex);
            if (data.Count == 0) return;

            // init backups list and backups 2 data
            oldItems.Clear();
            oldPointer = mPointer;
            oldPointerIndex = mPointerIndex;

            // if the list is blank, overwritten is invalid, just normal add them.
            if (mPointer == null) {
                // backups start pointer
                addStartNode = null;

                foreach (var item in data.IterateFull()) {
                    mMem.AddFirst(item.Value);
                }
                mPointer = mMem.First;
                mPointerIndex = 0;
            } else {
                LinkedListNode<FrameData> node = mMem.FastGetNode(mPointer, mPointerIndex, absolutePos);
                // if list is not a blank list, we should consider overwritten
                // if in overwritten mode, we need to overwrite data from selected item.
                // otherwise, not in overwritten mode, just normally add them just like add operation.
                if (isOverwritten) {
                    // in overwritten mode, if follwoing item is not enough to fufill the count of overwritten data
                    // normally add them
                    // we use delete and add method to do this
                    // another protential dangerous spot is pointer can be overwritten, its dangerous,
                    // so we need create a backup pos to store its relative postion and in add stage set pointer to the new data.
                    
                    var backupsNode = isInsertBefore ? node.Next : node.Previous;
                    // backups start pointer
                    addStartNode = backupsNode;

                    long gottenPointerPos = -1;
                    for(long i = 0; i < data.Count; i++) {
                        if (node == null) break;
                        if (node == mPointer) gottenPointerPos = i;
                        mMem.Remove(node);

                        // backup and shift to next
                        if (isInsertBefore) {
                            oldItems.AddFirst(node);
                            node = node.Previous;
                        } else {
                            oldItems.AddLast(node);
                            node = node.Next;
                        }
                    }

                    node = backupsNode;
                    if (node == null) {
                        foreach (var item in data.IterateFullReversed()) {
                            // add from head
                            mMem.AddFirst(item);
                        }
                    } else {
                        long counter = 0;
                        if (isInsertBefore) {
                            foreach (var item in data.IterateFull()) {
                                if (node == null) {
                                    // insert from tail instead
                                    mMem.AddLast(item.Value);
                                } else {
                                    mMem.AddBefore(node, item.Value);
                                }
                                
                                if (counter == gottenPointerPos)
                                    mPointer = node.Previous;
                                counter++;
                            }
                        } else {
                            foreach (var item in data.IterateFullReversed()) {
                                if (node == null) {
                                    // insert from head instead
                                    mMem.AddFirst(item.Value);
                                } else {
                                    mMem.AddAfter(node, item.Value);
                                }
                                
                                if (counter == gottenPointerPos)
                                    mPointer = node.Next;
                                counter++;
                            }
                        }
                    }

                } else {
                    // backups start pointer
                    addStartNode = node;

                    if (isInsertBefore) {
                        foreach (var item in data.IterateFull()) {
                            mMem.AddBefore(node, item.Value);
                        }
                    } else {
                        foreach (var item in data.IterateFullReversed()) {
                            mMem.AddAfter(node, item.Value);
                        }
                    }
                    if (mPointerIndex > absolutePos)
                        mPointerIndex += data.Count;
                }

            }
        }

        public override void Undo(ref LinkedList<FrameData> mMem, ref LinkedListNode<FrameData> mPointer, ref long mPointerIndex) {
            base.Undo(ref mMem, ref mPointer, ref mPointerIndex);
            if (data.Count == 0) return;

            if (addStartNode == null) {
                // original state is blank
                // just clear mmem is ok
                mMem.Clear();
            } else {
                // in overwrite or not in overwrite mode, we all need to remove inserted data first
                if (isInsertBefore) {
                    for (long i = 0; i < data.Count; i++) {
                        mMem.Remove(addStartNode.Previous);
                    }
                } else {
                    for (long i = 0; i < data.Count; i++) {
                        mMem.Remove(addStartNode.Next);
                    }
                }

                // if we use overwrite mode, we need re-add lost data
                if (isOverwritten) {
                    if (isInsertBefore) {
                        foreach (var item in data.IterateFull()) {
                            mMem.AddBefore(addStartNode, item);
                        }
                    } else {
                        foreach (var item in data.IterateFullReversed()) {
                            mMem.AddAfter(addStartNode, item);
                        }
                    }
                }
            }

            // re-set pointer
            mPointer = oldPointer;
            mPointerIndex = oldPointerIndex;
        }
    }

}
