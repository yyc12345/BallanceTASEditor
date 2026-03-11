using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {

    /// <summary>
    /// 所有用于在内存中存储TAS帧的结构都必须实现此interface。
    /// </summary>
    public interface ITasSequence : IEnumerable<TasFrame> {
        /// <summary>
        /// 访问给定索引的帧的值。
        /// </summary>
        /// <remarks>
        /// 实现此函数时需要格外注意以下事项：
        /// <para/>
        /// 该函数应当保证在访问临近项时有较高的效率。
        /// <para/>
        /// 该函数理论上的复杂度应为O(1)。
        /// </remarks>
        /// <param name="index">要访问的单元的索引。</param>
        /// <returns>被访问的单元。</returns>
        /// <exception cref="ArgumentOutOfRangeException">给定的索引无效。</exception>
        TasFrame Visit(int index);
        /// <summary>
        /// 在给定的帧索引<b>之前</b>插入给定的项目。
        /// </summary>
        /// <remarks>
        /// 实现此函数时需要格外注意以下事项：
        /// <para/>
        /// 按照函数约定，如果要在头部插入数据，则可以通过指定0来实现。
        /// 然而对于在尾部插入数据，或在空的存储中插入数据，可以指定存储结构的长度来实现。
        /// 即指定<c>(最大Index + 1)</c>作为参数来实现。
        /// <para/>
        /// 该函数理论上的复杂度应为O(1)。
        /// </remarks>
        /// <param name="index">要在前方插入数据的元素的索引。</param>
        /// <param name="items">要插入的元素的迭代器。</param>
        /// <exception cref="ArgumentOutOfRangeException">给定的索引无效。</exception>
        void Insert(int index, CountableEnumerable<TasFrame> items);
        /// <summary>
        /// 从序列中移出给定帧区间的元素。
        /// </summary>
        /// <remarks>
        /// 实现此函数时需要格外注意以下事项：
        /// <para/>
        /// 该函数理论上的复杂度应为O(1)。
        /// </remarks>
        /// <param name="startIndex">要移除的帧区间的起始索引（包含）。</param>
        /// <param name="endIndex">要移除的帧区间的终止索引（包含）</param>
        /// <exception cref="ArgumentOutOfRangeException">给定的索引无效。</exception>
        void Remove(int startIndex, int endIndex);

        /// <summary>
        /// 清空存储结构。
        /// </summary>
        void Clear();
        /// <summary>
        /// 获取当前存储的TAS帧的个数。
        /// </summary>
        /// <returns>存储的TAS帧的个数。</returns>
        int GetCount();
        /// <summary>
        /// 获取当前存储结构是不是空的。
        /// </summary>
        /// <returns>如果是空的就返回true，否则返回false。</returns>
        bool IsEmpty();
    }

    /// <summary>
    /// 基于Gap Buffer思想的TAS存储器。
    /// </summary>
    /// <remarks>
    /// 其实就是把List的InsertRange的复杂度从O(n*m)修正为O(n)。
    /// </remarks>
    public class GapBufferSequence : ITasSequence, IEnumerable<TasFrame> {
        public GapBufferSequence() {
            // Do nothing.
        }

        public TasFrame Visit(int index) {
            throw new NotImplementedException();
        }

        public void Insert(int index, CountableEnumerable<TasFrame> items) {
            throw new NotImplementedException();
        }

        public void Remove(int startIndex, int endIndex) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public int GetCount() {
            throw new NotImplementedException();
        }

        public bool IsEmpty() {
            throw new NotImplementedException();
        }

        public IEnumerator<TasFrame> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 基于简单的List的TAS存储器。
    /// </summary>
    /// <remarks>
    /// 由于List的InsertRange的复杂度是O(n*m)，可能不符合要求。
    /// </remarks>
    public class ListTasSequence : ITasSequence, IEnumerable<TasFrame> {
        public ListTasSequence() {
            m_Container = new List<TasFrame>();
        }

        private List<TasFrame> m_Container;

        public TasFrame Visit(int index) {
            if (index >= m_Container.Count || index < 0) {
                throw new IndexOutOfRangeException("Invalid index for frame.");
            } else {
                return m_Container[index];
            }
        }

        public void Insert(int index, CountableEnumerable<TasFrame> items) {
            if (index == m_Container.Count) {
                m_Container.AddRange(items.GetInner());
            } else {
                if (index > m_Container.Count || index < 0) {
                    throw new IndexOutOfRangeException("Invalid index for frame.");
                } else {
                    m_Container.InsertRange(index, items.GetInner());
                }
            }
        }

        public void Remove(int startIndex, int endIndex) {
            if (endIndex < startIndex || startIndex < 0 || endIndex >= m_Container.Count) {
                throw new IndexOutOfRangeException("Invalid index for frame.");
            } else {
                m_Container.RemoveRange(startIndex, endIndex - startIndex);
            }
        }

        public void Clear() {
            m_Container.Clear();
        }

        public int GetCount() {
            return m_Container.Count;
        }

        public bool IsEmpty() {
            return GetCount() == 0;
        }

        public IEnumerator<TasFrame> GetEnumerator() {
            return m_Container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// 传统的基于LinkedList的TAS存储器。
    /// </summary>
    public class LegacyTasSequence : ITasSequence, IEnumerable<TasFrame> {
        public LegacyTasSequence() {
            m_Container = new LinkedList<TasFrame>();
            m_Cursor = null;
        }

        private class LinkedListCursor<T> {
            public LinkedListCursor(LinkedListNode<T> node, int index) {
                this.Node = node;
                this.Index = index;
            }

            public LinkedListNode<T> Node;
            public int Index;
        }

        private LinkedList<TasFrame> m_Container;
        private LinkedListCursor<TasFrame>? m_Cursor;

        private enum NodeSeekOrigin {
            Head,
            Cursor,
            Tail,
        }

        private struct NodeSeekInfo : IComparable<NodeSeekInfo> {
            public required NodeSeekOrigin Origin;
            public required int Offset;

            public int CompareTo(NodeSeekInfo other) {
                return Math.Abs(this.Offset).CompareTo(Math.Abs(other.Offset));
            }
        }

        /// <summary>
        /// 快速将内部游标移动到指定Index，并更新与之匹配的Index。
        /// </summary>
        /// <param name="desiredIndex"></param>
        /// <exception cref="Exception"></exception>
        [MemberNotNull(nameof(m_Cursor))]
        private void MoveToIndex(int desiredIndex) {
            // 检查基本环境
            if (desiredIndex < 0 || desiredIndex >= GetCount())
                throw new IndexOutOfRangeException("Invalid index for frame.");
            if (m_Cursor is null || IsEmpty())
                throw new InvalidOperationException("Can not move cursor when container is empty.");

            // 创建三个候选方案。
            var candidates = new NodeSeekInfo[3] {
                new NodeSeekInfo() { Origin = NodeSeekOrigin.Head, Offset = desiredIndex },
                new NodeSeekInfo() { Origin = NodeSeekOrigin.Tail, Offset = desiredIndex - (GetCount() - 1) },
                new NodeSeekInfo() { Origin = NodeSeekOrigin.Cursor, Offset = desiredIndex - m_Cursor.Index },
            };
            // 确定哪个候选方案最短。
            var bestCandidate = candidates.Min();
            // 用最短候选方案移动。
            int pickedOffset = bestCandidate.Offset;
            LinkedListNode<TasFrame> pickedNode = bestCandidate.Origin switch {
                NodeSeekOrigin.Head => m_Container.First.Unwrap(),
                NodeSeekOrigin.Cursor => m_Cursor.Node,
                NodeSeekOrigin.Tail => m_Container.Last.Unwrap(),
                _ => throw new UnreachableException("Unknown NodeSeekOrigin"),
            };
            int alreadyMoved = 0;
            if (pickedOffset < 0) {
                while (alreadyMoved != pickedOffset) {
                    pickedNode = pickedNode.Previous.Unwrap();
                    alreadyMoved--;
                }
            } else if (pickedOffset > 0) {
                while (alreadyMoved != pickedOffset) {
                    pickedNode = pickedNode.Next.Unwrap();
                    alreadyMoved++;
                }
            }

            // 设置Cursor
            m_Cursor = new LinkedListCursor<TasFrame>(pickedNode, desiredIndex);
        }

        public TasFrame Visit(int index) {
            if (index >= m_Container.Count || index < 0) {
                throw new IndexOutOfRangeException("Invalid index for frame.");
            } else {
                MoveToIndex(index);
                return m_Cursor.Node.Value;
            }
        }

        public void Insert(int index, CountableEnumerable<TasFrame> items) {
            if (index >= m_Container.Count || index < 0) {
                throw new IndexOutOfRangeException("Invalid index for frame.");
            } else {
                if (index == m_Container.Count) {
                    foreach (TasFrame item in items.GetInner()) {
                        m_Container.AddLast(item);
                    }

                    var pendingCursor = m_Container.First;
                    if (pendingCursor is null) {
                        m_Cursor = null;
                    } else {
                        m_Cursor = new LinkedListCursor<TasFrame>(pendingCursor, 0);
                    }
                } else {
                    MoveToIndex(index);

                    foreach (TasFrame item in items.GetInner()) {
                        m_Container.AddBefore(m_Cursor.Node, item);
                    }
                    m_Cursor.Index += items.GetCount();
                }
            }
        }

        public void Remove(int startIndex, int endIndex) {
            if (endIndex < startIndex || startIndex < 0 || endIndex >= m_Container.Count) {
                throw new IndexOutOfRangeException("Invalid index for frame.");
            }

            // Compute count and move to index.
            var count = endIndex - startIndex;
            MoveToIndex(startIndex);

            // 我们总是获取要删除的项目的前一项来作为参照。
            // 如果获取到的是null，则说明是正在删第一项，从m_Container里获取First来删除就行，
            // 否则就继续用这个Node的Next来删除。
            var prevNode = m_Cursor.Node.Previous;
            if (prevNode is null) {
                for (int i = 0; i < count; ++i) {
                    m_Container.RemoveFirst();
                }
            } else {
                for (int i = 0; i < count; ++i) {
                    m_Container.Remove(prevNode.Next.Unwrap());
                }
            }

            // 然后设置Cursor和Index
            if (IsEmpty()) {
                // 如果全部删完了，就清除这两个的设置。
                m_Cursor = null;
            } else {
                if (prevNode is null) {
                    // 如果是按头部删除的，则直接获取头部及其Index。
                    m_Cursor = new LinkedListCursor<TasFrame>(m_Container.First.Unwrap(), 0);
                } else {
                    // 否则就以prevNode为当前Cursor，Index--为对应Index。
                    m_Cursor.Node = prevNode;
                    --m_Cursor.Index;
                }
            }
        }

        public void Clear() {
            m_Container.Clear();
            m_Cursor = null;
        }

        public int GetCount() {
            return m_Container.Count;
        }

        public bool IsEmpty() {
            return GetCount() == 0;
        }

        public IEnumerator<TasFrame> GetEnumerator() {
            return m_Container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
