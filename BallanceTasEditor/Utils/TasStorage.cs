using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BallanceTasEditor.Utils {

    /// <summary>
    /// 所有用于在内存中存储TAS帧的结构都必须实现此interface。
    /// </summary>
    public interface ITasStorage<T> {
        /// <summary>
        /// 访问给定索引的值。
        /// </summary>
        /// <param name="index">要访问的单元的索引。</param>
        /// <returns>被访问的单元。</returns>
        /// <exception cref="ArgumentException">给定的索引超出范围。</exception>
        T Visit(int index);
        /// <summary>
        /// 在给定的索引<b>之前</b>插入给定的项目。
        /// </summary>
        /// <remarks>
        /// 按照此函数约定，如果要在头部插入数据，则可以通过指定0来实现。
        /// 然而对于在尾部插入数据，或在空的存储中插入数据，可以指定存储结构的长度来实现。
        /// 即指定最大Index + 1的值来实现。
        /// 实现此函数时需要格外注意。
        /// </remarks>
        /// <param name="index">要在前方插入数据的元素的索引。</param>
        /// <param name="items">要插入的元素的迭代器。</param>
        /// <exception cref="ArgumentException">给定的索引超出范围。</exception>
        void Insert(int index, IEnumerable<T> items);
        /// <summary>
        /// 从给定单元开始，移除给定个数的元素。
        /// </summary>
        /// <param name="index">要开始移除的单元的索引。</param>
        /// <param name="count">要移除的元素的个数。</param>
        /// <exception cref="ArgumentException">给定的索引超出范围。</exception>
        void Remove(int index, int count);

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
    public class GapBufferTasStorage<T> : ITasStorage<T> {
        public GapBufferTasStorage() {

        }

        public T Visit(int index) {
            throw new NotImplementedException();
        }

        public void Insert(int index, IEnumerable<T> items) {
            throw new NotImplementedException();
        }

        public void Remove(int index, int count) {
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
    }

    /// <summary>
    /// 基于简单的List的TAS存储器。
    /// </summary>
    /// <remarks>
    /// 由于List的InsertRange的复杂度是O(n*m)，可能不符合要求。
    /// </remarks>
    public class ListTasStorage<T> : ITasStorage<T> {
        public ListTasStorage() {
            m_Container = new List<T>();
        }

        private List<T> m_Container;

        public T Visit(int index) {
            return m_Container[index];
        }

        public void Insert(int index, IEnumerable<T> items) {
            m_Container.InsertRange(index, items);
        }

        public void Remove(int index, int count) {
            m_Container.RemoveRange(index, count);
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
    }

    /// <summary>
    /// 传统的基于LinkedList的TAS存储器。
    /// </summary>
    public class LegacyTasStorage<T> : ITasStorage<T> {
        public LegacyTasStorage() {
            m_Container = new LinkedList<T>();
            m_Cursor = null;
            m_CursorIndex = null;
        }

        private LinkedList<T> m_Container;
        private LinkedListNode<T> m_Cursor;
        private int? m_CursorIndex;

        private enum NodeSeekOrigin {
            Head,
            Cursor,
            Tail,
        }

        private struct NodeSeekInfo : IComparable<NodeSeekInfo> {
            public NodeSeekOrigin Origin;
            public int Offset;

            public int CompareTo(NodeSeekInfo other) {
                return Math.Abs(this.Offset).CompareTo(Math.Abs(other.Offset));
            }
        }

        /// <summary>
        /// 快速将内部游标移动到指定Index，并更新与之匹配的Index。
        /// </summary>
        /// <param name="desiredIndex"></param>
        /// <exception cref="Exception"></exception>
        private void MoveToIndex(int desiredIndex) {
            // 检查基本环境
            if (desiredIndex < 0 || desiredIndex >= GetCount())
                throw new ArgumentOutOfRangeException("Index out of range");
            if (m_Cursor is null || !m_CursorIndex.HasValue || IsEmpty())
                throw new InvalidOperationException("Can not move cursor when container is empty.");

            // 创建三个候选方案。
            var candidates = new NodeSeekInfo[3] {
                new NodeSeekInfo() { Origin = NodeSeekOrigin.Head, Offset = desiredIndex },
                new NodeSeekInfo() { Origin = NodeSeekOrigin.Tail, Offset = desiredIndex - (GetCount() - 1) },
                new NodeSeekInfo() { Origin = NodeSeekOrigin.Cursor, Offset = desiredIndex - m_CursorIndex.Value },
            };
            // 确定哪个候选方案最短。
            var bestCandidate = candidates.Min();
            // 用最短候选方案移动。
            int pickedOffset = bestCandidate.Offset;
            LinkedListNode<T> pickedNode = null;
            switch (bestCandidate.Origin) {
                case NodeSeekOrigin.Head:
                    pickedNode = m_Container.First;
                    break;
                case NodeSeekOrigin.Cursor:
                    pickedNode = m_Cursor;
                    break;
                case NodeSeekOrigin.Tail:
                    pickedNode = m_Container.Last;
                    break;
            }
            long alreadyMoved = 0;
            if (pickedOffset < 0) {
                while (alreadyMoved != pickedOffset) {
                    pickedNode = pickedNode.Previous;
                    alreadyMoved--;
                }
            } else if (pickedOffset > 0) {
                while (alreadyMoved != pickedOffset) {
                    pickedNode = pickedNode.Next;
                    alreadyMoved++;
                }
            }

            // 设置Cursor和Index
            m_Cursor = pickedNode;
            m_CursorIndex = desiredIndex;
        }

        public T Visit(int index) {
            if (index < 0 || index >= GetCount()) {
                throw new ArgumentOutOfRangeException("Index out of range.");
            } else {
                MoveToIndex(index);
                return m_Cursor.Value;
            }
        }

        public void Insert(int index, IEnumerable<T> items) {
            if (index < 0 || index > GetCount()) {
                throw new ArgumentOutOfRangeException("Index out of range.");
            } else if (index == GetCount()) {
                foreach (T item in items) {
                    m_Container.AddLast(item);
                }

                m_Cursor = m_Container.First;
                if (m_Cursor is null) m_CursorIndex = null;
                else m_CursorIndex = 0;
            } else {
                MoveToIndex(index);

                int count = 0;
                foreach (T item in items) {
                    m_Container.AddBefore(m_Cursor, item);
                    ++count;
                }
                m_CursorIndex += count;
            }
        }

        public void Remove(int index, int count) {
            if (count == 0)
                return;
            if (index + count > GetCount())
                throw new ArgumentOutOfRangeException("Expected removed items out of range.");

            MoveToIndex(index);

            // 我们总是获取要删除的项目的前一项来作为参照。
            // 如果获取到的是null，则说明是正在删第一项，从m_Container里获取First来删除就行，
            // 否则就继续用这个Node的Next来删除。
            var prevNode = m_Cursor.Previous;
            if (prevNode is null) {
                for (int i = 0; i < count; ++i) {
                    m_Container.RemoveFirst();
                }
            } else {
                for (int i = 0; i < count; ++i) {
                    m_Container.Remove(prevNode.Next);
                }
            }

            // 然后设置Cursor和Index
            if (IsEmpty()) {
                // 如果全部删完了，就清除这两个的设置。
                m_Cursor = null;
                m_CursorIndex = null;
            } else {
                if (prevNode is null) {
                    // 如果是按头部删除的，则直接获取头部及其Index。
                    m_Cursor = m_Container.First;
                    m_CursorIndex = 0;
                } else {
                    // 否则就以prevNode为当前Cursor，Index--为对应Index。
                    m_Cursor = prevNode;
                    --m_CursorIndex;
                }
            }
        }

        public void Clear() {
            m_Container.Clear();
            m_Cursor = null;
            m_CursorIndex = null;
        }

        public int GetCount() {
            return m_Container.Count();
        }

        public bool IsEmpty() {
            return GetCount() == 0;
        }
    }
}
