using BallanceTasEditor.Backend;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditorTests.Backend {
    [TestClass]
    public class TasOperationTests {

        private static IEnumerable<object[]> TasSequenceInstanceProvider {
            get {
                yield return new object[] { new ListTasSequence() };
                yield return new object[] { new LegacyTasSequence() };
                // TODO: Add GapBufferTasSequence once we finish it.
                //yield return new object[] { new GapBufferTasSequence() };
            }
        }

        /// <summary>
        /// CellKeysOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void CellKeysOperationTest(ITasSequence sequence, CellKeysOperationKind kind, int startIndex, int endIndex, TasKey startKey, TasKey endKey) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// FrameFpsOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void FrameFpsOperationTest(ITasSequence sequence) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// RemoveFrameOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void RemoveFrameOperationTest(ITasSequence sequence) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// AddFrameOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void AddFrameOperationTest(ITasSequence sequence) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// InsertFrameOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void InsertFrameOperationTest(ITasSequence sequence) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ClearKeysOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void ClearKeysOperationTest(ITasSequence sequence) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// UniformFpsOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(TasSequenceInstanceProvider))]
        public void UniformFpsOperationTest(ITasSequence sequence) {
            throw new NotImplementedException();
        }

    }
}
