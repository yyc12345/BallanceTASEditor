using BallanceTASEditor.Core.TASStruct;
using BallanceTASEditor.UI;
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
        public static IEnumerable<LinkedListNode<FrameData>> IterateFull(this LinkedList<FrameData> ls) {
            var pos = ls.First;
            
            while(pos != null) {
                yield return pos;
                pos = pos.Next;
            }
        }

        public static LinkedListNode<FrameData> FastGetNode(this LinkedList<FrameData> ls, LinkedListNode<FrameData> refNode, long refIndex, long targetIndex) {
            long count = ls.Count;
            if (targetIndex >= count || refIndex >= count) throw new Exception("Index is invalid!");
            var span = new StupidSortStruct[3] {
                new StupidSortStruct() { type = 1, data = targetIndex },
                new StupidSortStruct() { type = 2, data = count - targetIndex },
                new StupidSortStruct() { type = 3, data = targetIndex - refIndex }
            };

            // sort to get the min value
            StupidSortStruct tmp;
            if (Math.Abs(span[0].data) < Math.Abs(span[1].data)) {
                tmp = span[0];
                span[0] = span[1];
                span[1] = tmp;
            }
            if (Math.Abs(span[1].data) < Math.Abs(span[2].data)) {
                tmp = span[1];
                span[2] = span[1];
                span[2] = tmp;
            }

            LinkedListNode<FrameData> iterateNode;
            if (span[2].type == 1) iterateNode = ls.First;
            else if (span[2].type == 2) iterateNode = ls.Last;
            else if (span[2].type == 3) iterateNode = refNode;
            else throw new Exception("Unknow node type");

            return iterateNode.ShiftTo(span[2].data);
        }

        // remove safety. because it store the next node.
        public static IEnumerable<LinkedListNode<FrameData>> IterateWithSelectionRange(this LinkedList<FrameData> ls, SelectionRange absoluteRange, LinkedListNode<FrameData> refNode, long refIndex) {
            // goto header first
            var cache = ls.FastGetNode(refNode, refIndex, absoluteRange.start);

            var counter = absoluteRange.start;
            LinkedListNode<FrameData> cacheNextNode;
            while (counter <= absoluteRange.end) {
                if (cache == null) throw new Exception("Unexpected head or tail of linked list!");
                cacheNextNode = cache.Next;
                yield return cache;
                cache = cacheNextNode;
                counter++;
            }
        }

        public static LinkedListNode<FrameData> ShiftTo(this LinkedListNode<FrameData> node, long offset) {
            var cache = node;

            long realShifted = 0;
            if (offset < 0) {
                while (realShifted != offset) {
                    if (cache.Previous == null) throw new Exception("Unexpected head or tail of linked list!");
                    cache = cache.Previous;
                    realShifted--;
                }
            } else if (offset > 0) {
                while (realShifted != offset) {
                    if (cache.Next == null) throw new Exception("Unexpected head or tail of linked list!");
                    cache = cache.Next;
                    realShifted++;
                }
            }

            return cache;
        }
    }


    public struct SelectionRange {
        public SelectionRange(long value1, long value2) {
            if (value1 > value2) {
                start = value2;
                end = value1;
            } else {
                start = value1;
                end = value2;
            }
        }
        public long start;
        public long end;
        public SelectionRange GetRelative(long refer) {
            var res = new SelectionRange();
            res.start = start - refer;
            res.end = end - refer;
            return res;
        }
        public bool Within(long num) {
            return (num >= start && num <= end);
        }
        public long GetCount() {
            return end - start;
        }
    }

    public struct StupidSortStruct {
        public int type;
        public long data;
    }

}
