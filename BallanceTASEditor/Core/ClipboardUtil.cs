using BallanceTASEditor.Core.TASStruct;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace BallanceTASEditor.Core {
    public class ClipboardUtil {

        // comes from https://stackoverflow.com/questions/22272822/copy-binary-data-to-clipboard

        private static readonly string CLIPBOARD_DATA_FORMAT = "BallanceTASFrameData";
        public static bool SetFrameData(LinkedList<FrameData> ls) {
            try {
                DataObject data = new DataObject();
                using (var mem = new MemoryStream()) {
                    mem.Write(BitConverter.GetBytes(ls.Count), 0, 4);

                    var node = ls.First;
                    while (node != null) {
                        mem.Write(BitConverter.GetBytes(node.Value.deltaTime), 0, 4);
                        mem.Write(BitConverter.GetBytes(node.Value.keystates), 0, 4);
                        node = node.Next;
                    }

                    data.SetData(CLIPBOARD_DATA_FORMAT, mem, false);
                    Clipboard.SetDataObject(data, true);
                }
                return true;
#if DEBUG
            } catch (Exception e) {
#else
            } catch {
#endif
                return false;
            }
        }

        public static bool GetFrameData(LinkedList<FrameData> ls) {
            try {
                // detect
                DataObject retrievedData = Clipboard.GetDataObject() as DataObject;
                if (retrievedData == null || !retrievedData.GetDataPresent(CLIPBOARD_DATA_FORMAT))
                    return false;
                MemoryStream byteStream = retrievedData.GetData(CLIPBOARD_DATA_FORMAT) as MemoryStream;
                if (byteStream == null)
                    return false;

                // read
                byteStream.Seek(0, SeekOrigin.Begin);
                byte[] temp = new byte[8];
                byteStream.Read(temp, 0, 4);
                int count = BitConverter.ToInt32(temp, 0);

                for (int i = 0; i < count; i++) {
                    ls.AddLast(new FrameData(byteStream));
                }

                return true;
#if DEBUG
            } catch (Exception e) {
#else
            } catch {
#endif
                return false;
            }
        } 
    }
}
