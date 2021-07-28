using BallanceTASEditor.Core.TASStruct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.Core {
    public class ZlibUtil {
        private const int COPY_STREAM_UNIT = 1024;

        public static void CompressTAS(LinkedList<FrameData> mem, FileStream file) {
            file.Write(BitConverter.GetBytes(mem.Count * ConstValue.FRAMEDATA_SIZE), 0, 4);

            var zo = new zlib.ZOutputStream(file, 9);
            var node = mem.First;
            while (node != null) {
                zo.Write(BitConverter.GetBytes(node.Value.deltaTime), 0, 4);
                zo.Write(BitConverter.GetBytes(node.Value.keystates), 0, 4);
                node = node.Next;
            }
            zo.finish();
            zo.Close();
        }

        public static void DecompressTAS(LinkedList<FrameData> ls, FileStream file) {
            var lengthTemp = new byte[4];
            var mem = new MemoryStream();
            file.Read(lengthTemp, 0, 4);
            Int32 expectedLength = BitConverter.ToInt32(lengthTemp, 0);
            long expectedCount = expectedLength / ConstValue.FRAMEDATA_SIZE;

            var zo = new zlib.ZOutputStream(mem);
            CopyStream(file, zo);
            zo.finish();

            mem.Seek(0, SeekOrigin.Begin);
            for(long i = 0; i < expectedCount; i++) {
                ls.AddLast(new FrameData(mem));
            }
            mem.Close();
            zo.Close();
        }

        public static void CopyStream(Stream origin, Stream target) {
            var buffer = new byte[COPY_STREAM_UNIT];
            int len;
            while ((len = origin.Read(buffer, 0, COPY_STREAM_UNIT)) > 0) {
                target.Write(buffer, 0, len);
            }
            //target.Flush();
        }

    }
}
