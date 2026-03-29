using CommunityToolkit.HighPerformance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {
    public static class TasStorage {
        internal const int SIZEOF_I32 = sizeof(int);
        internal const int SIZEOF_F32 = sizeof(float);
        internal const int SIZEOF_U32 = sizeof(uint);
        internal const int SIZEOF_RAW_TAS_FRAME = SIZEOF_F32 + SIZEOF_U32;

        public static void Save(string filepath, ITasSequence seq) {
            using (var fs = new FileStream(filepath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)) {
                Save(fs, seq);
                fs.Close();
            }
        }

        public static void Save(Stream fs, ITasSequence seq) {
            var totalByte = seq.GetCount() * SIZEOF_RAW_TAS_FRAME;
            fs.Write(BitConverter.GetBytes(totalByte), 0, SIZEOF_I32);

            using (var zo = new Ionic.Zlib.ZlibStream(fs, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.Level9, true)) {
                foreach (var item in seq) {
                    var rawItem = item.ToRaw();
                    zo.Write(BitConverter.GetBytes(rawItem.TimeDelta), 0, SIZEOF_F32);
                    zo.Write(BitConverter.GetBytes(rawItem.KeyFlags), 0, SIZEOF_U32);
                }
                zo.Close();
            }

            //var zo = new zlib.ZOutputStream(file, 9);
            //var node = mem.First;
            //while (node != null) {
            //    zo.Write(BitConverter.GetBytes(node.Value.deltaTime), 0, 4);
            //    zo.Write(BitConverter.GetBytes(node.Value.keystates), 0, 4);
            //    node = node.Next;
            //}
            //zo.finish();
            //zo.Close();
        }

        public static void Load(string filepath, ITasSequence seq) {
            using (var fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                Load(fs, seq);
                fs.Close();
            }
        }

        public static void Load(Stream fs, ITasSequence seq) {
            // Read total bytes
            var lenCache = new byte[SIZEOF_I32];
            fs.Read(lenCache, 0, 4);
            int expectedLength = BitConverter.ToInt32(lenCache, 0);
            // Check length and compute count
            int expectedCount = Math.DivRem(expectedLength, SIZEOF_RAW_TAS_FRAME, out var remainder);
            ArgumentOutOfRangeException.ThrowIfNotEqual(remainder, 0);

            using (var mem = new MemoryStream()) {
                using (var zo = new Ionic.Zlib.ZlibStream(mem, Ionic.Zlib.CompressionMode.Decompress, true)) {
                    CopyStream(fs, zo);
                    zo.Close();
                }

                var memWrapper = new EnumerableMemoryStream(mem, expectedCount);
                seq.Clear();
                seq.Insert(0, memWrapper);

                mem.Close();
            }

            //mem.Seek(0, SeekOrigin.Begin);
            //for (long i = 0; i < expectedCount; i++) {
            //    ls.AddLast(new FrameData(mem));
            //}
            //mem.Close();
            //zo.Close();

            //var zo = new zlib.ZOutputStream(mem);
            //CopyStream(file, zo);
            //zo.finish();

            //mem.Seek(0, SeekOrigin.Begin);
            //for (long i = 0; i < expectedCount; i++) {
            //    ls.AddLast(new FrameData(mem));
            //}
            //mem.Close();
            //zo.Close();
        }

        private const int STREAM_COPY_CHUNK_SIZE = 10240;

        private static void CopyStream(Stream origin, Stream target) {
            var buffer = new byte[STREAM_COPY_CHUNK_SIZE];
            int len;
            while ((len = origin.Read(buffer, 0, STREAM_COPY_CHUNK_SIZE)) > 0) {
                target.Write(buffer, 0, len);
            }
            //target.Flush();
        }

        private sealed class EnumerableMemoryStream : IExactSizeEnumerable<TasFrame> {
            public EnumerableMemoryStream(MemoryStream mem, int frameCnt) {
                m_MemoryStream = mem;
                m_FrameCount = frameCnt;
            }

            private MemoryStream m_MemoryStream;
            private int m_FrameCount;

            public IEnumerator<TasFrame> GetEnumerator() {
                // Get the view of underlying array
                var memory = m_MemoryStream.GetBuffer().AsMemory();
                // Get the span which actually storing the data,
                // because the length of buffer is equal or longer than the length of all stored data.
                var exactMemory = memory.Slice(0, m_FrameCount * SIZEOF_RAW_TAS_FRAME);
                // Convert to raw frame type.
                var frameMemory = exactMemory.Cast<byte, RawTasFrame>();
                // Map it and return.
                return MemoryMarshal.ToEnumerable<RawTasFrame>(frameMemory).Select((rawFrame) => TasFrame.FromRaw(rawFrame)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public int GetCount() {
                return m_FrameCount;
            }
        }

    }
}
