using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace BallanceTASEditor.Core.TASStruct {
    public class FrameDataDisplay : INotifyPropertyChanged {
        public FrameDataDisplay(long index, FrameData fd) {
            Reload(index, fd);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reload(long index, FrameData fd) {
            this.index = index;
            this.deltaTime = fd.deltaTime;

            OnPropertyChanged("index");
            OnPropertyChanged("deltaTime");

            this.keystates = fd.keystates;
        }

        public long index { get; set; }
        public float deltaTime { get; set; }
        public UInt32 keystates {
            get {
                UInt32 result = 0;
                if (key_enter) result |= 1; result <<= 1;
                if (key_esc) result |= 1; result <<= 1;
                if (key_q) result |= 1; result <<= 1;
                if (key_space) result |= 1; result <<= 1;
                if (key_shift) result |= 1; result <<= 1;
                if (key_right) result |= 1; result <<= 1;
                if (key_left) result |= 1; result <<= 1;
                if (key_down) result |= 1; result <<= 1;
                if (key_up) result |= 1; result <<= 1;

                return result;
            }
            set {
                key_up = (value & (1 << 0)).ToBool();
                key_down = (value & (1 << 1)).ToBool();
                key_left = (value & (1 << 2)).ToBool();
                key_right = (value & (1 << 3)).ToBool();
                key_shift = (value & (1 << 4)).ToBool();
                key_space = (value & (1 << 5)).ToBool();
                key_q = (value & (1 << 6)).ToBool();
                key_esc = (value & (1 << 7)).ToBool();
                key_enter = (value & (1 << 8)).ToBool();

                OnPropertyChanged("key_up");
                OnPropertyChanged("key_down");
                OnPropertyChanged("key_left");
                OnPropertyChanged("key_right");
                OnPropertyChanged("key_shift");
                OnPropertyChanged("key_space");
                OnPropertyChanged("key_q");
                OnPropertyChanged("key_esc");
                OnPropertyChanged("key_enter");
            }
        }
        public bool key_up { get; set; }
        public bool key_down { get; set; }
        public bool key_left { get; set; }
        public bool key_right { get; set; }
        public bool key_shift { get; set; }
        public bool key_space { get; set; }
        public bool key_q { get; set; }
        public bool key_esc { get; set; }
        public bool key_enter { get; set; }
    }

    public class FrameData {

        public FrameData(Stream st) {
            var temp = new byte[ConstValue.FRAMEDATA_SIZE];
            st.Read(temp, 0, ConstValue.FRAMEDATA_SIZE);

            deltaTime = BitConverter.ToSingle(temp, ConstValue.FRAMEDATA_OFFSET_DELTATIME);
            keystates = BitConverter.ToUInt32(temp, ConstValue.FRAMEDATA_OFFSET_KEY_STATES);
        }
        public FrameData(FrameDataDisplay fdd) {
            this.deltaTime = fdd.deltaTime;
            this.keystates = fdd.keystates;
        }

        public FrameData() {
            this.deltaTime = 0f;
            this.keystates = 0;
        }

        public FrameData(float d, UInt32 k) {
            this.deltaTime = d;
            this.keystates = k;
        }

        public void SetKeyStates(UInt32 offset) {
            keystates |= offset;
        }

        public void UnsetKeyStates(UInt32 offset) {
            keystates &= ~offset;
        }

        public float deltaTime;
        public UInt32 keystates;
    }

    public class ConstValue {
        public static readonly Dictionary<FrameDataField, UInt32> Mapping = new Dictionary<FrameDataField, UInt32>() {
            {FrameDataField.Key_Up, (1 << 0)},
            {FrameDataField.Key_Down, (1 << 1)},
            {FrameDataField.Key_Left, (1 << 2)},
            {FrameDataField.Key_Right, (1 << 3)},
            {FrameDataField.Key_Shift, (1 << 4)},
            {FrameDataField.Key_Space, (1 << 5)},
            {FrameDataField.Key_Q, (1 << 6)},
            {FrameDataField.Key_Esc, (1 << 7)},
            {FrameDataField.Key_Enter, (1 << 8)}
        };
        public const int FRAMEDATA_SIZE = 8;
        public const int FRAMEDATA_OFFSET_DELTATIME = 0;
        public const int FRAMEDATA_OFFSET_KEY_STATES = 4;
    }

    public enum FrameDataField {
        Key_Up,
        Key_Down,
        Key_Left,
        Key_Right,
        Key_Shift,
        Key_Space,
        Key_Q,
        Key_Esc,
        Key_Enter
    }

}
