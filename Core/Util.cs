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


        public static IEnumerable<LinkedListNode<FrameData>> IterateWithSelectionRange(this LinkedList<FrameData> ls, SelectionRange relativeRange, LinkedListNode<FrameData> current) {
            if (current == null) goto end;

            // goto header first
            long counter;
            var cache = current.TryShiftTo(relativeRange.start, out counter);

            while (counter <= relativeRange.end && cache != null) {
                yield return cache;
                cache = cache.Next;
                counter++;
            }
            
            end:;
        }

        public static void RemoveWithSelectionRange(this LinkedList<FrameData> ls, SelectionRange relativeRange, LinkedListNode<FrameData> current) {
            if (current == null) goto end;

            // goto header first
            long counter;
            var cache = current.TryShiftTo(relativeRange.start, out counter);

            LinkedListNode<FrameData> cacheNextNode;
            while (counter <= relativeRange.end && cache != null) {
                cacheNextNode = cache.Next;
                ls.Remove(cache);
                cache = cacheNextNode;
                counter++;
            }

            end:;
        }

        public static LinkedListNode<FrameData> TryShiftTo(this LinkedListNode<FrameData> node, long offset, out long realShifted) {
            var cache = node;

            realShifted = 0;
            if (offset < 0) {
                while (realShifted != offset && cache.Previous != null) {
                    cache = cache.Previous;
                    realShifted--;
                }
            } else if (offset > 0) {
                while (realShifted != offset && cache.Next != null) {
                    cache = cache.Next;
                    realShifted++;
                }
            }

            return cache;
        }
    }
}
