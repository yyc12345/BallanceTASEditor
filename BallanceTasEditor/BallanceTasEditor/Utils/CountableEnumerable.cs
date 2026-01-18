using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Utils {
    /// <summary>
    /// 一种提前给定元素个数的的IEnumerable。
    /// </summary>
    public sealed class CountableEnumerable<T> {
        /// <summary>
        /// 以直接方式构建。
        /// </summary>
        /// <param name="enumerable">一个迭代器，其最多只能迭代给定次数。</param>
        /// <param name="count">迭代器会迭代的次数。</param>
        public CountableEnumerable(IEnumerable<T> enumerable, int count) {
            m_Inner = enumerable;
            m_Count = count;
        }

        /// <summary>
        /// 从数组便捷构建。
        /// </summary>
        /// <param name="array">要使用的数组。</param>
        public CountableEnumerable(T[] array) {
            m_Inner = array;
            m_Count = array.Length;
        }

        private IEnumerable<T> m_Inner;
        private int m_Count;

        /// <summary>
        /// 获取迭代器对象。
        /// </summary>
        /// <returns>用于迭代的迭代器。</returns>
        /// <exception cref="ArgumentException">当迭代器迭代次数与给定次数不匹配时。</exception>
        public IEnumerable<T> GetInner() {
            int counter = 0;
            foreach (var item in m_Inner) {
                if (counter >= m_Count) {
                    throw new ArgumentException("Given IEnumerable<T> is not stopped at given count.");
                } else {
                    yield return item;
                    ++counter;
                }
            }

            if (counter != m_Count) {
                throw new ArgumentException("Given IEnumerable<T> is not stopped at given count.");
            }
        }

        /// <summary>
        /// 获取该迭代器会迭代的次数。
        /// </summary>
        /// <returns>迭代器会迭代的次数，用于给使用该结构的方法提前分配必要的空间。</returns>
        public int GetCount() {
            return m_Count;
        }

    }
}
