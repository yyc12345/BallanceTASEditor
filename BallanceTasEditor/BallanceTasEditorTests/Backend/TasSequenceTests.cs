using BallanceTasEditor.Backend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditorTests.Backend {
    [TestClass]
    public class TasSequenceTests {

        private static readonly TasFrame[] BLANK = { };
        private static readonly TasFrame[] PROBE = {
            new TasFrame(10), 
            new TasFrame(20), 
            new TasFrame(30), 
            new TasFrame(40), 
            new TasFrame(50),
        };

        private static CountableEnumerable<TasFrame> GetCountableProbe() {
            return new CountableEnumerable<TasFrame>(PROBE);
        }

        private static CountableEnumerable<TasFrame> GetCountableBlank() {
            return new CountableEnumerable<TasFrame>(BLANK);
        }

        private static IEnumerable<object[]> TasSequenceInstanceProvider {
            get {
                yield return new object[] { new ListTasSequence() };
                yield return new object[] { new LegacyTasSequence() };
                // TODO: Add GapBufferTasSequence once we finish it.
                //yield return new object[] { new GapBufferTasSequence() };
            }
        }

        /// <summary>
        /// Visit函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void VisitTest(ITasSequence sequence) {
            // 空时访问
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(-1));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(0));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(1));

            // 设置数据
            sequence.Insert(0, GetCountableProbe());
            // 访问数据
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(-1));
            for (int i = 0; i < PROBE.Length; i++) {
                Assert.AreEqual(sequence.Visit(i), PROBE[i]);
            }
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(PROBE.Length));
        }

        /// <summary>
        /// Insert函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void InsertTest(ITasSequence sequence) {
            // 需要在不同的存储器上，分别检测在空的时候插入，
            // 和在非空时的头，中，尾分别插入的结果。

            // 先检测空插入
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Insert(-1, GetCountableProbe()));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Insert(1, GetCountableProbe()));
            sequence.Insert(0, GetCountableProbe());
            for (int i = 0; i < PROBE.Length; i++) {
                Assert.AreEqual(sequence.Visit(i), PROBE[i]);
            }

            // 再检测有数据的插入，分别在头尾和中间进行
            var indices = new int[] { 0, PROBE.Length / 2, PROBE.Length - 1, PROBE.Length };
            foreach (var index in indices) {
                // 清空，一次插入，然后二次插入
                sequence.Clear();
                sequence.Insert(0, GetCountableProbe());
                sequence.Insert(index, GetCountableProbe());

                // 用List做正确模拟
                var expected = new List<TasFrame>();
                expected.AddRange(PROBE);
                expected.InsertRange(index, PROBE);

                // 检查结果
                Assert.AreEqual(sequence.GetCount(), expected.Count);
                for (int i = 0; i < expected.Count; i++) {
                    Assert.AreEqual(sequence.Visit(i), expected[i]);
                }
            }

        }

        /// <summary>
        /// Remove函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void RemoveTest(ITasSequence sequence) {
            // 在空的时候删除0项
            sequence.Remove(0, 0);

            // 插入项目后尝试在头中尾分别删除
            var indices = new int[] { 0, PROBE.Length / 2, PROBE.Length - 1 };
            foreach (var index in indices) {
                // 清空，插入，删除
                sequence.Clear();
                sequence.Insert(0, GetCountableProbe());
                sequence.Remove(index, 1);

                // 用List做正确模拟
                var expected = new List<TasFrame>();
                expected.AddRange(PROBE);
                expected.RemoveRange(index, 1);

                // 检查结果
                Assert.AreEqual(sequence.GetCount(), expected.Count);
                for (int i = 0; i < expected.Count; i++) {
                    Assert.AreEqual(sequence.Visit(i), expected[i]);
                }
            }
        }

        /// <summary>
        /// Clear函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void ClearTest(ITasSequence sequence) {
            // 设置数据后清空
            sequence.Insert(0, GetCountableProbe());
            sequence.Clear();

            // 检查是否为空
            Assert.IsTrue(sequence.IsEmpty());
        }

        /// <summary>
        /// IsEmpty函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void IsEmptyTest(ITasSequence sequence) {
            // 检查是否为空
            Assert.IsTrue(sequence.IsEmpty());

            // 插入数据后再检查
            sequence.Insert(0, GetCountableProbe());
            Assert.IsFalse(sequence.IsEmpty());
        }

        /// <summary>
        /// GetCount函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void GetCountTest(ITasSequence sequence) {
            // 检查长度为0
            Assert.AreEqual(sequence.GetCount(), 0);

            // 插入数据后再检查
            sequence.Insert(0, GetCountableProbe());
            Assert.AreEqual(sequence.GetCount(), PROBE.Length);
        }

        /// <summary>
        /// 混合检查Visit，Clear，GetCount，IsEmpty。
        /// </summary>
        /// <param name="sequence"></param>
        [TestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void HybridTest(ITasSequence sequence) {
            // 检查空和大小
            Assert.IsTrue(sequence.IsEmpty());
            Assert.AreEqual(sequence.GetCount(), 0);

            // 设置内容
            sequence.Insert(0, GetCountableProbe());
            // 并再次检查大小
            Assert.IsFalse(sequence.IsEmpty());
            Assert.AreEqual(sequence.GetCount(), PROBE.Length);

            // 访问数据
            var len = PROBE.Length;
            for (int i = 0; i < len; ++i) {
                Assert.AreEqual(sequence.Visit(i), PROBE[i]);
            }

            // 清空数据
            sequence.Clear();
            // 再次检查数据
            Assert.IsTrue(sequence.IsEmpty());
            Assert.AreEqual(sequence.GetCount(), 0);

            // 清空后插入0项，然后确认
            sequence.Clear();
            sequence.Insert(0, GetCountableBlank());
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(-1));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(0));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => sequence.Visit(1));
        }
    }
}
