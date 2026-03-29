using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BallanceTasEditor.Backend {
    public class TasClipboard {
        // Reference: https://stackoverflow.com/questions/22272822/copy-binary-data-to-clipboard

        private static readonly string CLIPBOARD_DATA_FORMAT = "BallanceTasEditor.TasFrames";

        public static void SetClipboard(IExactSizeEnumerable<TasFrame> frames) {
            DataObject data = new DataObject();
            var rawFrames = frames.Select((f) => f.ToRaw()).ToArray();
            data.SetData(CLIPBOARD_DATA_FORMAT, rawFrames, false);
            Clipboard.SetDataObject(data, true);
        }

        private static RawTasFrame[]? GetClipboardObject() {
            DataObject? retrievedData = Clipboard.GetDataObject() as DataObject;
            if (retrievedData is null) return null;
            if (!retrievedData.GetDataPresent(CLIPBOARD_DATA_FORMAT)) return null;

            RawTasFrame[]? rawFrames = retrievedData.GetData(CLIPBOARD_DATA_FORMAT) as RawTasFrame[];
            if (rawFrames is null) return null;

            return rawFrames;
        }

        public static bool HasClipboard() {
            return GetClipboardObject() is not null;
        }

        public static IExactSizeEnumerable<TasFrame>? GetClipboard() {
            var rawFrames = GetClipboardObject();
            if (rawFrames is null) return null;

            return new EnumerableArray(rawFrames);
        }

        private sealed class EnumerableArray : IExactSizeEnumerable<TasFrame> {
            public EnumerableArray(RawTasFrame[] rawFrames) {
                m_RawFrames = rawFrames;
            }

            private RawTasFrame[] m_RawFrames;

            public IEnumerator<TasFrame> GetEnumerator() {
                return m_RawFrames.Select((f) => TasFrame.FromRaw(f)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public int GetCount() {
                return m_RawFrames.Length;
            }

        }
    }
}
