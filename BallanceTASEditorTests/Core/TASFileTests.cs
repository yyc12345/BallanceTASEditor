using BallanceTASEditor.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BallanceTASEditor.Core.TASStruct;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BallanceTASEditor.Core.Tests {
    [TestClass()]
    public class TASFileTests {

        [DataTestMethod]
        [DataRow("1,2,3,4,5", 0, "15;0;15,15,15,4,5,", FrameDataField.Key_Up, FrameDataField.Key_Right, 0, 2, true)]
        [DataRow("1,2,3,4,5", 0, "0;0;0,0,0,4,5,", FrameDataField.Key_Up, FrameDataField.Key_Right, 0, 2, false)]
        [DataRow("1,2,3,4,5", 0, "14;0;14,13,12,4,5,", FrameDataField.Key_Up, FrameDataField.Key_Right, 0, 2, null)]
        public void SetTest(string originStr, long prevShift, string expectedStr, FrameDataField field_start, FrameDataField field_end, long absoluteRange_start, long absoluteRange_end, bool? isSet) {
            var test = new TASFile(dataGenerator(originStr));
            test.Shift(prevShift);
            var originalText = test.Output2TestString();

            // test function
            test.Set(new SelectionRange((long)field_start, (long)field_end), new SelectionRange(absoluteRange_start, absoluteRange_end), isSet);
            var changedText = test.Output2TestString();
            Assert.AreEqual(expectedStr, changedText);

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());

            // test redo
            test.Redo();
            Assert.AreEqual(changedText, test.Output2TestString());

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());
        }

        [DataTestMethod]
        [DataRow("1,2,3,4,5", 0, "3;0;3,4,5,", 0, 1)]
        [DataRow("1,2,3,4,5", 0, "null;-1;", 0, 4)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,3,", 3, 4)]
        [DataRow("1,2,3,4,5", 0, "2;0;2,3,4,5,", 0, 0)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,3,4,", 4, 4)]
        [DataRow("1,2,3,4,5", 2, "1;0;1,5,", 1, 3)]
        [DataRow("1,2,3,4,5", 2, "5;0;5,", 0, 3)]
        [DataRow("1,2,3,4,5", 2, "1;0;1,", 1, 4)]
        [DataRow("1,2,3,4,5", 3, "4;0;4,5,", 0, 2)]
        [DataRow("1,2,3,4,5", 4, "5;1;4,5,", 0, 2)]
        public void RemoveTest(string originStr, long prevShift, string expectedStr, long absoluteRange_start, long absoluteRange_end) {
            var test = new TASFile(dataGenerator(originStr));
            test.Shift(prevShift);
            var originalText = test.Output2TestString();

            // test function
            test.Remove(new SelectionRange(absoluteRange_start, absoluteRange_end));
            var changedText = test.Output2TestString();
            Assert.AreEqual(expectedStr, changedText);

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());

            // test redo
            test.Redo();
            Assert.AreEqual(changedText, test.Output2TestString());

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());
        }

        [DataTestMethod]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,0,0,0,3,4,5,", 2, 3, 240 / 1f, true)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,3,0,0,0,4,5,", 2, 3, 240 / 1f, false)]
        [DataRow("1,2,3,4,5", 2, "3;5;1,2,0,0,0,3,4,5,", 2, 3, 240 / 1f, true)]
        [DataRow("1,2,3,4,5", 2, "3;2;1,2,3,0,0,0,4,5,", 2, 3, 240 / 1f, false)]

        [DataRow("1,2,3,4,5", 0, "1;3;0,0,0,1,2,3,4,5,", 0, 3, 240 / 1f, true)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,3,4,5,0,0,0,", 4, 3, 240 / 1f, false)]
        public void AddTest(string originStr, long prevShift, string expectedStr, long absolutePos, long count, float deltaTime, bool isAddBefore) {
            var test = new TASFile(dataGenerator(originStr));
            test.Shift(prevShift);
            var originalText = test.Output2TestString();

            // test function
            test.Add(absolutePos, count, deltaTime, isAddBefore);
            var changedText = test.Output2TestString();
            Assert.AreEqual(expectedStr, changedText);

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());

            // test redo
            test.Redo();
            Assert.AreEqual(changedText, test.Output2TestString());

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());
        }

        [DataTestMethod]
        [DataRow("1,2,3,4,5", 0, "6;0;6,7,8,9,10,", 0, "6,7,8,9,10", false, true)]
        [DataRow("1,2,3,4,5", 0, "6;0;6,7,8,9,10,", 4, "6,7,8,9,10", true, true)]

        [DataRow("1,2,3,4,5", 0, "1;0;1,2,6,7,8,3,4,5,", 2, "6,7,8", true, false)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,3,6,7,8,4,5,", 2, "6,7,8", false, false)]

        [DataRow("1,2,3,4,5", 0, "1;0;1,2,3,4,5,6,7,", 4, "6,7", false, false)]
        [DataRow("1,2,3,4,5", 0, "1;2;6,7,1,2,3,4,5,", 0, "6,7", true, false)]

        [DataRow("1,2,3,4,5", 0, "1;0;1,6,7,4,5,", 2, "6,7", true, true)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,6,7,5,", 2, "6,7", false, true)]
        [DataRow("1,2,3,4,5", 0, "4;3;6,7,8,4,5,", 2, "6,7,8", true, true)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,6,7,8,", 2, "6,7,8", false, true)]
        [DataRow("1,2,3,4,5", 0, "4;4;6,7,8,9,4,5,", 2, "6,7,8,9", true, true)]
        [DataRow("1,2,3,4,5", 0, "1;0;1,2,6,7,8,9,", 2, "6,7,8,9", false, true)]

        [DataRow("1,2,3,4,5", 2, "1;0;1,6,7,4,5,", 2, "6,7", true, true)]
        [DataRow("1,2,3,4,5", 2, "2;1;1,2,6,7,5,", 2, "6,7", false, true)]
        [DataRow("1,2,3,4,5", 2, "4;3;6,7,8,4,5,", 2, "6,7,8", true, true)]
        [DataRow("1,2,3,4,5", 2, "2;1;1,2,6,7,8,", 2, "6,7,8", false, true)]
        [DataRow("1,2,3,4,5", 2, "4;4;6,7,8,9,4,5,", 2, "6,7,8,9", true, true)]
        [DataRow("1,2,3,4,5", 2, "2;1;1,2,6,7,8,9,", 2, "6,7,8,9", false, true)]

        [DataRow("1,2,3,4,5", 2, "3;5;1,2,6,7,8,3,4,5,", 2, "6,7,8", true, false)]
        [DataRow("1,2,3,4,5", 2, "3;2;1,2,3,6,7,8,4,5,", 2, "6,7,8", false, false)]
        public void InsertTest(string originStr, long prevShift, string expectedStr, long absolutePos, string insertedData, bool isInsertBefore, bool isOverwritten) {
            var test = new TASFile(dataGenerator(originStr));
            test.Shift(prevShift);
            var originalText = test.Output2TestString();

            // test function
            test.Insert(absolutePos, dataGenerator(insertedData), isInsertBefore, isOverwritten);
            var changedText = test.Output2TestString();
            Assert.AreEqual(expectedStr, changedText);

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());

            // test redo
            test.Redo();
            Assert.AreEqual(changedText, test.Output2TestString());

            // test undo
            test.Undo();
            Assert.AreEqual(originalText, test.Output2TestString());
        }

        // example input: 1,2,3,4,5
        private LinkedList<FrameData> dataGenerator(string inputData) {
            var ls = new LinkedList<FrameData>();
            foreach (var item in inputData.Split(',')) {
                ls.AddLast(new FrameData(float.Parse(item), uint.Parse(item)));
            }
            return ls;
        }

    }
}