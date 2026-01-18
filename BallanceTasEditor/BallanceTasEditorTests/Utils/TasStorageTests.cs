using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BallanceTasEditor.Utils;

namespace BallanceTasEditorTests.Utils {
    [TestClass]
    public class TasStorageTests {

        private static readonly int[] BLANK = { };
        private static readonly int[] PROBE = { 10, 20, 30, 40, 50 };

        private static CountableEnumerable<int> GetCountableProbe() {
            return new CountableEnumerable<int>(PROBE);
        }

        private static CountableEnumerable<int> GetCountableBlank() {
            return new CountableEnumerable<int>(BLANK);
        }

        private static IEnumerable<object[]> TasStorageInstanceProvider {
            get {
                yield return new object[] { new ListTasStorage<int>() };
                yield return new object[] { new LegacyTasStorage<int>() };
                // TODO: Add GapBufferTasStorage once we finish it.
                //yield return new object[] { new GapBufferTasStorage<int>() };
            }
        }

        /// <summary>
        /// Visit函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasStorageInstanceProvider))]
        public void VisitTest(ITasStorage<int> storage) {
            // 空时访问
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(-1));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(0));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(1));

            // 设置数据
            storage.Insert(0, GetCountableProbe());
            // 访问数据
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(-1));
            for (int i = 0; i < PROBE.Length; i++) {
                Assert.AreEqual(storage.Visit(i), PROBE[i]);
            }
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(PROBE.Length));
        }

        /// <summary>
        /// Insert函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasStorageInstanceProvider))]
        public void InsertTest(ITasStorage<int> storage) {
            // 需要在不同的存储器上，分别检测在空的时候插入，
            // 和在非空时的头，中，尾分别插入的结果。

            // 先检测空插入
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Insert(-1, GetCountableProbe()));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Insert(1, GetCountableProbe()));
            storage.Insert(0, GetCountableProbe());
            for (int i = 0; i < PROBE.Length; i++) {
                Assert.AreEqual(storage.Visit(i), PROBE[i]);
            }

            // 再检测有数据的插入，分别在头尾和中间进行
            var indices = new int[] { 0, PROBE.Length / 2, PROBE.Length - 1, PROBE.Length };
            foreach (var index in indices) {
                // 清空，一次插入，然后二次插入
                storage.Clear();
                storage.Insert(0, GetCountableProbe());
                storage.Insert(index, GetCountableProbe());

                // 用List做正确模拟
                var expected = new List<int>();
                expected.AddRange(PROBE);
                expected.InsertRange(index, PROBE);

                // 检查结果
                Assert.AreEqual(storage.GetCount(), expected.Count);
                for (int i = 0; i < expected.Count; i++) {
                    Assert.AreEqual(storage.Visit(i), expected[i]);
                }
            }

        }

        /// <summary>
        /// Remove函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasStorageInstanceProvider))]
        public void RemoveTest(ITasStorage<int> storage) {
            // 在空的时候删除0项
            storage.Remove(0, 0);

            // 插入项目后尝试在头中尾分别删除
            var indices = new int[] { 0, PROBE.Length / 2, PROBE.Length - 1 };
            foreach (var index in indices) {
                // 清空，插入，删除
                storage.Clear();
                storage.Insert(0, GetCountableProbe());
                storage.Remove(index, 1);

                // 用List做正确模拟
                var expected = new List<int>();
                expected.AddRange(PROBE);
                expected.RemoveRange(index, 1);

                // 检查结果
                Assert.AreEqual(storage.GetCount(), expected.Count);
                for (int i = 0; i < expected.Count; i++) {
                    Assert.AreEqual(storage.Visit(i), expected[i]);
                }
            }
        }

        /// <summary>
        /// Clear函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasStorageInstanceProvider))]
        public void ClearTest(ITasStorage<int> storage) {
            // 设置数据后清空
            storage.Insert(0, GetCountableProbe());
            storage.Clear();

            // 检查是否为空
            Assert.IsTrue(storage.IsEmpty());
        }

        /// <summary>
        /// IsEmpty函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasStorageInstanceProvider))]
        public void IsEmptyTest(ITasStorage<int> storage) {
            // 检查是否为空
            Assert.IsTrue(storage.IsEmpty());

            // 插入数据后再检查
            storage.Insert(0, GetCountableProbe());
            Assert.IsFalse(storage.IsEmpty());
        }

        /// <summary>
        /// GetCount函数独立测试。
        /// </summary>
        [TestMethod]
        [DynamicData(nameof(TasStorageInstanceProvider))]
        public void GetCountTest(ITasStorage<int> storage) {
            // 检查长度为0
            Assert.AreEqual(storage.GetCount(), 0);

            // 插入数据后再检查
            storage.Insert(0, GetCountableProbe());
            Assert.AreEqual(storage.GetCount(), PROBE.Length);
        }

        /// <summary>
        /// 混合检查Visit，Clear，GetCount，IsEmpty。
        /// </summary>
        /// <param name="storage"></param>
        [TestMethod]
        [DynamicData(nameof(TasStorageInstanceProvider))]
        public void HybridTest(ITasStorage<int> storage) {
            // 检查空和大小
            Assert.IsTrue(storage.IsEmpty());
            Assert.AreEqual(storage.GetCount(), 0);

            // 设置内容
            storage.Insert(0, GetCountableProbe());
            // 并再次检查大小
            Assert.IsFalse(storage.IsEmpty());
            Assert.AreEqual(storage.GetCount(), PROBE.Length);

            // 访问数据
            var len = PROBE.Length;
            for (int i = 0; i < len; ++i) {
                Assert.AreEqual(storage.Visit(i), PROBE[i]);
            }

            // 清空数据
            storage.Clear();
            // 再次检查数据
            Assert.IsTrue(storage.IsEmpty());
            Assert.AreEqual(storage.GetCount(), 0);

            // 清空后插入0项，然后确认
            storage.Clear();
            storage.Insert(0, GetCountableBlank());
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(-1));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(0));
            AssertExtension.ThrowsDerivedException<ArgumentException>(() => storage.Visit(1));
        }
    }
}
