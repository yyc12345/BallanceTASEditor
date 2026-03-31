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

        #region Utilities

        private static IEnumerable<object[]> TestDataCombiner<T>(IEnumerable<ITasSequence> seqs, IEnumerable<T> payloads) where T : notnull {
            // YYC MARK:
            // We must iterate payload as outside "for" and iterate sequence as inner "for",
            // because if we flip this order, each payload will share the same reference to sequence,
            // which cause test error.
            foreach (T payload in payloads) {
                foreach (ITasSequence seq in seqs) {
                    yield return new object[] { seq, payload };
                }
            }
        }

        private static void AssertRevocableOperation(ITasSequence sequence, ITasRevocableOperation op, string source, string expected) {
            // Prepare environment
            sequence.Clear();
            GenerateSequence(sequence, source);

            // Test execute
            op.Execute(sequence);
            Assert.AreEqual(expected, SummarizeSequence(sequence));

            // Test revoke
            op.Revoke(sequence);
            Assert.AreEqual(source, SummarizeSequence(sequence));

            // Test re-execute and re-revoke again
            op.Execute(sequence);
            Assert.AreEqual(expected, SummarizeSequence(sequence));
            op.Revoke(sequence);
            Assert.AreEqual(source, SummarizeSequence(sequence));
        }

        private static void AssertOperation(ITasSequence sequence, ITasOperation op, string source, string expected) {
            // Prepare environment
            sequence.Clear();
            GenerateSequence(sequence, source);

            // Test execute
            op.Execute(sequence);
            Assert.AreEqual(expected, SummarizeSequence(sequence));
        }

        private static void GenerateSequence(ITasSequence sequence, string pattern) {
            var strFrame = pattern.Split(';');
            var frameIter = strFrame.Select((s) => {
                // Extract FPS and Keys
                var framePair = s.Split(',');
                var fps = uint.Parse(framePair[0]);
                var keyFlags = uint.Parse(framePair[1]);
                // Build raw frame and convert it to frame.
                var rawFrame = new RawTasFrame() { TimeDelta = FpsConverter.ToDelta(fps), KeyFlags = keyFlags };
                return TasFrame.FromRaw(rawFrame);
            });
            var frameExactSizeIter = new ExactSizeEnumerableAdapter<TasFrame>(frameIter, strFrame.Length);
            sequence.Insert(0, frameExactSizeIter);
        }

        private static string SummarizeSequence(ITasSequence sequence) {
            return string.Join(
                ";",
                sequence.Select((f) => {
                    var rawFrame = f.ToRaw();
                    return $"{FpsConverter.ToRoundFps(rawFrame.TimeDelta)},{rawFrame.KeyFlags}";
                })
            );
        }

        #endregion

        #region Cell Keys Operation

        public record CellKeysOperationTestPayload {
            public required string Source { get; init; }
            public required string Expected { get; init; }

            public required CellKeysOperationKind Kind { get; init; }
            public required int StartIndex { get; init; }
            public required int EndIndex { get; init; }
            public required TasKey StartKey { get; init; }
            public required TasKey EndKey { get; init; }
        }

        private static IEnumerable<CellKeysOperationTestPayload> GetCellKeysOperationTestPayload() {
            yield return new CellKeysOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,15;1,15;1,15;1,4;1,5",
                Kind = CellKeysOperationKind.Set,
                StartIndex = 0,
                EndIndex = 2,
                StartKey = TasKey.KEY_UP,
                EndKey = TasKey.KEY_RIGHT
            };
            yield return new CellKeysOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,0;1,0;1,0;1,4;1,5",
                Kind = CellKeysOperationKind.Unset,
                StartIndex = 0,
                EndIndex = 2,
                StartKey = TasKey.KEY_UP,
                EndKey = TasKey.KEY_RIGHT
            };
            yield return new CellKeysOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,14;1,13;1,12;1,4;1,5",
                Kind = CellKeysOperationKind.Flip,
                StartIndex = 0,
                EndIndex = 2,
                StartKey = TasKey.KEY_UP,
                EndKey = TasKey.KEY_RIGHT
            };
        }

        private static IEnumerable<object[]> CellKeysOperationTestDataProvider {
            get {
                return TestDataCombiner(TasSequenceUtils.EnumerateTasSequenceImplementation(), GetCellKeysOperationTestPayload());
            }
        }

        /// <summary>
        /// CellKeysOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(CellKeysOperationTestDataProvider))]
        public void CellKeysOperationTest(ITasSequence sequence, CellKeysOperationTestPayload payload) {
            var op = CellKeysOperation.FromCellRange(payload.Kind, payload.StartIndex, payload.EndIndex, payload.StartKey, payload.EndKey);
            AssertRevocableOperation(sequence, op, payload.Source, payload.Expected);
        }

        #endregion

        #region Frame Fps Operation

        public record FrameFpsOperationTestPayload {
            public required string Source { get; init; }
            public required string Expected { get; init; }

            public required int StartIndex { get; init; }
            public required int EndIndex { get; init; }
            public required uint Fps { get; init; }
        }

        private static IEnumerable<FrameFpsOperationTestPayload> GetFrameFpsOperationTestPayload() {
            yield return new FrameFpsOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "2,1;2,2;2,3;2,4;1,5",
                StartIndex = 0,
                EndIndex = 3,
                Fps = 2
            };
            yield return new FrameFpsOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,1;4,2;4,3;4,4;4,5",
                StartIndex = 1,
                EndIndex = 4,
                Fps = 4
            };
        }

        private static IEnumerable<object[]> FrameFpsOperationTestDataProvider {
            get {
                return TestDataCombiner(TasSequenceUtils.EnumerateTasSequenceImplementation(), GetFrameFpsOperationTestPayload());
            }
        }

        /// <summary>
        /// FrameFpsOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(FrameFpsOperationTestDataProvider))]
        public void FrameFpsOperationTest(ITasSequence sequence, FrameFpsOperationTestPayload payload) {
            var op = FrameFpsOperation.FromFrameRange(payload.StartIndex, payload.EndIndex, payload.Fps);
            AssertRevocableOperation(sequence, op, payload.Source, payload.Expected);
        }

        #endregion

        #region Remove Frame Operation

        public record RemoveFrameOperationTestPayload {
            public required string Source { get; init; }
            public required string Expected { get; init; }

            public required int StartIndex { get; init; }
            public required int EndIndex { get; init; }
        }

        private static IEnumerable<RemoveFrameOperationTestPayload> GetRemoveFrameOperationTestPayload() {
            yield return new RemoveFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,3;1,4;1,5",
                StartIndex = 0,
                EndIndex = 1
            };
            yield return new RemoveFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "",
                StartIndex = 0,
                EndIndex = 4
            };
            yield return new RemoveFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,1;1,2;1,3",
                StartIndex = 3,
                EndIndex = 4
            };
            yield return new RemoveFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,2;1,3;1,4;1,5",
                StartIndex = 0,
                EndIndex = 0
            };
            yield return new RemoveFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,1;1,2;1,3;1,4",
                StartIndex = 4,
                EndIndex = 4
            };
        }

        private static IEnumerable<object[]> RemoveFrameOperationTestDataProvider {
            get {
                return TestDataCombiner(TasSequenceUtils.EnumerateTasSequenceImplementation(), GetRemoveFrameOperationTestPayload());
            }
        }

        /// <summary>
        /// RemoveFrameOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(RemoveFrameOperationTestDataProvider))]
        public void RemoveFrameOperationTest(ITasSequence sequence, RemoveFrameOperationTestPayload payload) {
            var op = new RemoveFrameOperation(payload.StartIndex, payload.EndIndex);
            AssertRevocableOperation(sequence, op, payload.Source, payload.Expected);
        }

        #endregion

        #region Add Frame Operation

        public record AddFrameOperationTestPayload {
            public required string Source { get; init; }
            public required string Expected { get; init; }

            public required AddFrameOperationKind Kind { get; init; }
            public required int Index { get; init; }
            public required uint Fps { get; init; }
            public required int Count { get; init; }
        }

        private static IEnumerable<AddFrameOperationTestPayload> GetAddFrameOperationTestPayload() {
            yield return new AddFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,1;1,2;240,0;240,0;240,0;1,3;1,4;1,5",
                Kind = AddFrameOperationKind.Before,
                Index = 2,
                Fps = 240,
                Count = 3
            };
            yield return new AddFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,1;1,2;1,3;240,0;240,0;240,0;1,4;1,5",
                Kind = AddFrameOperationKind.After,
                Index = 2,
                Fps = 240,
                Count = 3
            };
            yield return new AddFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "240,0;240,0;240,0;1,1;1,2;1,3;1,4;1,5",
                Kind = AddFrameOperationKind.Before,
                Index = 0,
                Fps = 240,
                Count = 3
            };
            yield return new AddFrameOperationTestPayload {
                Source = "1,1;1,2;1,3;1,4;1,5",
                Expected = "1,1;1,2;1,3;1,4;1,5;240,0;240,0;240,0",
                Kind = AddFrameOperationKind.After,
                Index = 4,
                Fps = 240,
                Count = 3
            };
        }

        private static IEnumerable<object[]> AddFrameOperationTestDataProvider {
            get {
                return TestDataCombiner(TasSequenceUtils.EnumerateTasSequenceImplementation(), GetAddFrameOperationTestPayload());
            }
        }

        /// <summary>
        /// AddFrameOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(AddFrameOperationTestDataProvider))]
        public void AddFrameOperationTest(ITasSequence sequence, AddFrameOperationTestPayload payload) {
            var op = new AddFrameOperation(payload.Kind, payload.Index, payload.Fps, payload.Count);
            AssertRevocableOperation(sequence, op, payload.Source, payload.Expected);
        }

        #endregion

        #region Insert Frame Operation

        public record InsertFrameOperationTestPayload {
            public required string Source { get; init; }
            public required string Inserted { get; init; }
            public required string Expected { get; init; }

            public required InsertFrameOperationKind Kind { get; init; }
            public required int Index { get; init; }
            public required uint Fps { get; init; }
            public required int Count { get; init; }
        }

        private static IEnumerable<InsertFrameOperationTestPayload> GetInsertFrameOperationTestPayload() {
            yield break;
        }

        private static IEnumerable<object[]> InsertFrameOperationTestDataProvider {
            get {
                return TestDataCombiner(TasSequenceUtils.EnumerateTasSequenceImplementation(), GetInsertFrameOperationTestPayload());
            }
        }

        /// <summary>
        /// InsertFrameOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(InsertFrameOperationTestDataProvider))]
        public void InsertFrameOperationTest(ITasSequence sequence, InsertFrameOperationTestPayload payload) {
            //var op = new InsertFrameOperation(payload.Kind, payload.Index, payload.Fps, payload.Count);
            //AssertRevocableOperation(sequence, op, payload.Source, payload.Expected);
        }

        #endregion

        #region Clear Keys Operation

        public record ClearKeysOperationTestPayload {
            public required string Source { get; init; }
            public required string Expected { get; init; }
        }

        private static IEnumerable<ClearKeysOperationTestPayload> GetClearKeysOperationTestPayload() {
            yield break;
        }

        private static IEnumerable<object[]> ClearKeysOperationTestDataProvider {
            get {
                return TestDataCombiner(TasSequenceUtils.EnumerateTasSequenceImplementation(), GetClearKeysOperationTestPayload());
            }
        }

        /// <summary>
        /// ClearKeysOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(ClearKeysOperationTestDataProvider))]
        public void ClearKeysOperationTest(ITasSequence sequence, ClearKeysOperationTestPayload payload) {
            var op = new ClearKeysOperation();
            AssertOperation(sequence, op, payload.Source, payload.Expected);
        }

        #endregion

        #region Uniform Fps Operation

        public record UniformFpsOperationTestPayload {
            public required string Source { get; init; }
            public required string Expected { get; init; }

            public required uint Fps { get; init; }
        }

        private static IEnumerable<UniformFpsOperationTestPayload> GetUniformFpsOperationTestPayload() {
            yield break;
        }

        private static IEnumerable<object[]> UniformFpsOperationTestDataProvider {
            get {
                return TestDataCombiner(TasSequenceUtils.EnumerateTasSequenceImplementation(), GetUniformFpsOperationTestPayload());
            }
        }

        /// <summary>
        /// UniformFpsOperation测试。
        /// </summary>
        [DataTestMethod]
        [DynamicData(nameof(UniformFpsOperationTestDataProvider))]
        public void UniformFpsOperationTest(ITasSequence sequence, UniformFpsOperationTestPayload payload) {
            var op = new UniformFpsOperation(payload.Fps);
            AssertOperation(sequence, op, payload.Source, payload.Expected);
        }

        #endregion

    }
}
