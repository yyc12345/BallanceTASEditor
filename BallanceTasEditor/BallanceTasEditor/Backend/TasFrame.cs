using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {

    /// <summary>
    /// 原始的TAS帧结构，与二进制结构保持一致。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct RawTasFrame {
        /// <summary>
        /// 该帧的持续时间（以秒为单位）。
        /// </summary>
        public float TimeDelta;
        /// <summary>
        /// 该帧的按键组合。
        /// </summary>
        public uint KeyFlags;
    }

    /// <summary>
    /// 描述TAS文件中的可能的按键。
    /// </summary>
    public struct TasKey : IEquatable<TasKey> {
        private TasKey(int bitPos) {
            m_BitPos = bitPos;
        }

        private int m_BitPos;

        public static readonly TasKey KEY_UP = new TasKey(0);
        public static readonly TasKey KEY_DOWN = new TasKey(1);
        public static readonly TasKey KEY_LEFT = new TasKey(2);
        public static readonly TasKey KEY_RIGHT = new TasKey(3);
        public static readonly TasKey KEY_SHIFT = new TasKey(4);
        public static readonly TasKey KEY_SPACE = new TasKey(5);
        public static readonly TasKey KEY_Q = new TasKey(6);
        public static readonly TasKey KEY_ESC = new TasKey(7);
        public static readonly TasKey KEY_ENTER = new TasKey(8);

        public const int MIN_KEY_INDEX = 0;
        public const int MAX_KEY_INDEX = 8;

        public static bool IsValidIndex(int index) {
            return index >= MIN_KEY_INDEX && index <= MAX_KEY_INDEX;
        }

        public static TasKey FromIndex(int index) {
            if (index < MIN_KEY_INDEX || index > MAX_KEY_INDEX) {
                throw new ArgumentOutOfRangeException(nameof(index));
            } else {
                return new TasKey(index);
            }
        }

        public int ToIndex() {
            return m_BitPos;
        }

        public uint ToBitMaskKey() {
            return 1u << m_BitPos;
        }

        public bool Equals(TasKey other) {
            return m_BitPos == other.m_BitPos;
        }

        public override bool Equals(object? obj) {
            if (obj is TasKey other) {
                return Equals(other);
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return m_BitPos.GetHashCode();
        }

        public static bool operator ==(TasKey left, TasKey right) {
            return left.Equals(right);
        }

        public static bool operator !=(TasKey left, TasKey right) {
            return !left.Equals(right);
        }

        public override string ToString() {
            return m_BitPos switch {
                0 => "KeyUp",
                1 => "KeyDown",
                2 => "KeyLeft",
                3 => "KeyRight",
                4 => "KeyShift",
                5 => "KeySpace",
                6 => "KeyQ",
                7 => "KeyEsc",
                8 => "KeyEnter",
                _ => $"KeyUnknown<Pos={m_BitPos}>"
            };
        }
    }

    /// <summary>
    /// 描述TAS文件中一帧的结构。
    /// </summary>
    public class TasFrame : IEquatable<TasFrame> {
        private TasFrame(float timeDelta, uint keyFlags) {
            m_TimeDelta = timeDelta;
            m_KeyFlags = keyFlags;
        }

        /// <summary>
        /// 以指定的FPS，无任何按键初始化当前帧。
        /// </summary>
        public static TasFrame FromFps(uint fps = 60) {
            return new TasFrame(FpsConverter.ToDelta(fps), 0);
        }

        /// <summary>
        /// 从原始TAS数据初始化。
        /// </summary>
        /// <param name="raw">要用来初始化的原始数据。</param>
        public static TasFrame FromRaw(RawTasFrame raw) {
            return new TasFrame(raw.TimeDelta, raw.KeyFlags);
        }
        
        /// <summary>
        /// 将原始TAS数据覆写到自身
        /// </summary>
        /// <param name="raw">要写入的原始TAS数据</param>
        public void FromRawImplace(RawTasFrame raw) {
            m_TimeDelta = raw.TimeDelta;
            m_KeyFlags = raw.KeyFlags;
        }

        /// <summary>
        /// 转换为原始TAS数据。
        /// </summary>
        /// <returns>转换后的原始TAS数据。</returns>
        public RawTasFrame ToRaw() {
            return new RawTasFrame() { TimeDelta = m_TimeDelta, KeyFlags = m_KeyFlags };
        }

        /// <summary>
        /// 原位转换为原始TAS数据。
        /// </summary>
        /// <param name="raw">以引用传递的原始TAS数据。</param>
        public void ToRawImplace(ref RawTasFrame raw) {
            raw.TimeDelta = m_TimeDelta;
            raw.KeyFlags = m_KeyFlags;
        }

        /// <summary>
        /// 返回自身的克隆（深拷贝）。
        /// </summary>
        /// <returns>自身的克隆。</returns>
        public TasFrame Clone() {
            return new TasFrame(m_TimeDelta, m_KeyFlags);
        }

        /// <summary>
        /// 该帧的持续时间（以秒为单位）。
        /// </summary>
        private float m_TimeDelta;
        /// <summary>
        /// 该帧的按键组合。
        /// </summary>
        private uint m_KeyFlags;

        /// <summary>
        /// 获取帧时间Delta。
        /// </summary>
        /// <returns>获取到的帧时间Delta。</returns>
        public float GetTimeDelta() {
            return m_TimeDelta;
        }

        /// <summary>
        /// 设置帧时间Delta。
        /// </summary>
        /// <param name="delta">要设置的帧时间Delta。</param>
        public void SetTimeDelta(float delta) {
            m_TimeDelta = delta;
        }

        /// <summary>
        /// 判断按键是否被按下。
        /// </summary>
        /// <param name="key">要检查的按键。</param>
        /// <returns>true表示被按下，否则为false。</returns>
        public bool IsKeyPressed(TasKey key) {
            return (m_KeyFlags & key.ToBitMaskKey()) != 0;
        }

        /// <summary>
        /// 设置按键状态。
        /// </summary>
        /// <param name="key">要设置的按键。</param>
        /// <param name="pressed">true表示设置为按下，否则为松开。</param>
        public void SetKeyPressed(TasKey key, bool pressed = true) {
            if (pressed) m_KeyFlags |= key.ToBitMaskKey();
            else m_KeyFlags &= ~key.ToBitMaskKey();
        }

        /// <summary>
        /// 反转按键状态。
        /// </summary>
        /// <param name="key">要反转的按键。</param>
        public void FlipKeyPressed(TasKey key) {
            m_KeyFlags ^= key.ToBitMaskKey();
        }

        /// <summary>
        /// 获取或设置Up键的按下状态。
        /// </summary>
        public bool KeyUpPressed { get { return IsKeyPressed(TasKey.KEY_UP); } set { SetKeyPressed(TasKey.KEY_UP, value); } }
        /// <summary>
        /// 获取或设置Down键的按下状态。
        /// </summary>
        public bool KeyDownPressed { get { return IsKeyPressed(TasKey.KEY_DOWN); } set { SetKeyPressed(TasKey.KEY_DOWN, value); } }
        /// <summary>
        /// 获取或设置Left键的按下状态。
        /// </summary>
        public bool KeyLeftPressed { get { return IsKeyPressed(TasKey.KEY_LEFT); } set { SetKeyPressed(TasKey.KEY_LEFT, value); } }
        /// <summary>
        /// 获取或设置Right键的按下状态。
        /// </summary>
        public bool KeyRightPressed { get { return IsKeyPressed(TasKey.KEY_RIGHT); } set { SetKeyPressed(TasKey.KEY_RIGHT, value); } }
        /// <summary>
        /// 获取或设置Shift键的按下状态。
        /// </summary>
        public bool KeyShiftPressed { get { return IsKeyPressed(TasKey.KEY_SHIFT); } set { SetKeyPressed(TasKey.KEY_SHIFT, value); } }
        /// <summary>
        /// 获取或设置Space键的按下状态。
        /// </summary>
        public bool KeySpacePressed { get { return IsKeyPressed(TasKey.KEY_SPACE); } set { SetKeyPressed(TasKey.KEY_SPACE, value); } }
        /// <summary>
        /// 获取或设置Q键的按下状态。
        /// </summary>
        public bool KeyQPressed { get { return IsKeyPressed(TasKey.KEY_Q); } set { SetKeyPressed(TasKey.KEY_Q, value); } }
        /// <summary>
        /// 获取或设置Esc键的按下状态。
        /// </summary>
        public bool KeyEscPressed { get { return IsKeyPressed(TasKey.KEY_ESC); } set { SetKeyPressed(TasKey.KEY_ESC, value); } }
        /// <summary>
        /// 获取或设置回车键的按下状态。
        /// </summary>
        public bool KeyEnterPressed { get { return IsKeyPressed(TasKey.KEY_ENTER); } set { SetKeyPressed(TasKey.KEY_ENTER, value); } }

        /// <summary>
        /// 清除所有按键，将所有按键设置为不按下。
        /// </summary>
        public void ClearKeyPressed() {
            m_KeyFlags = 0;
        }

        /// <summary>
        /// 指示当前对象是否等于另一个 TasFrame 对象。
        /// </summary>
        /// <param name="other">要比较的 TasFrame 对象。</param>
        /// <returns>如果两个对象相等则为 true，否则为 false。</returns>
        public bool Equals(TasFrame? other) {
            return other is not null &&
                   m_TimeDelta == other.m_TimeDelta &&
                   m_KeyFlags == other.m_KeyFlags;
        }

        /// <summary>
        /// 指示当前对象是否等于另一个对象。
        /// </summary>
        /// <param name="obj">要比较的对象。</param>
        /// <returns>如果两个对象相等则为 true，否则为 false。</returns>
        public override bool Equals(object? obj) {
            if (obj is TasFrame other) {
                return Equals(other);
            } else {
                return false;
            }
        }

        /// <summary>
        /// 返回此实例的哈希代码。
        /// </summary>
        /// <returns>32 位有符号整数哈希代码。</returns>
        public override int GetHashCode() {
            return HashCode.Combine(m_TimeDelta, m_KeyFlags);
        }

    }

}
