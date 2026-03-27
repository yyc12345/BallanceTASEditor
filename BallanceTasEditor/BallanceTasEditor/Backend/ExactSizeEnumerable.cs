using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {

    /// <summary>
    /// 一种提前给定元素个数的的IEnumerable。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExactSizeEnumerable<out T> : IEnumerable<T> {
        /// <summary>
        /// 该迭代器会返回的元素的个数。
        /// </summary>
        /// <remarks>
        /// 如果迭代器返回的元素个数与该方法给定的个数不同，
        /// 则是未定义行为。
        /// </remarks>
        /// <returns>迭代器会返回的元素的准确个数。大于等于0。</returns>
        public int GetCount();
    }

    /// <summary>
    /// 将普通IEnumerable转变为IExactSizeEnumerable的适配器。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ExactSizeEnumerableAdapter<T> : IExactSizeEnumerable<T> {
        /// <summary>
        /// 以迭代器和指定长度构建适配器。
        /// </summary>
        /// <remarks>
        /// 如果迭代器返回的元素个数与该方法给定的个数不同，
        /// 则是未定义行为。
        /// </remarks>
        /// <param name="enumerable">一个迭代器，其最多只能迭代给定次数。</param>
        /// <param name="count">迭代器会迭代的次数。</param>
        public ExactSizeEnumerableAdapter(IEnumerable<T> enumerable, int count) {
            m_Inner = enumerable;
            m_Count = count;
        }

        private readonly IEnumerable<T> m_Inner;
        private readonly int m_Count;

        public IEnumerator<T> GetEnumerator() {
            return m_Inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return m_Inner.GetEnumerator();
        }

        public int GetCount() {
            return m_Count;
        }

    }

}
