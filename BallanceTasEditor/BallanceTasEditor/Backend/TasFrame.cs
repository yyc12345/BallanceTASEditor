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
    /// 描述TAS文件中一帧的结构。
    /// </summary>
    public class TasFrame {
        /// <summary>
        /// 以指定的FPS，无任何按键初始化当前帧。
        /// </summary>
        public TasFrame(uint fps = 60) {
            m_TimeDelta = FpsConverter.ToDelta(fps);
            m_KeyFlags = 0;
        }

        /// <summary>
        /// 从原始TAS数据初始化。
        /// </summary>
        /// <param name="raw">要用来初始化的原始数据。</param>
        public TasFrame(RawTasFrame raw) {
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
        public void ToImplaceRaw(ref RawTasFrame raw) {
            raw.TimeDelta = m_TimeDelta;
            raw.KeyFlags = m_KeyFlags;
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
            return (m_KeyFlags & (1u << (int)key)) != 0;
        }

        /// <summary>
        /// 设置按键状态。
        /// </summary>
        /// <param name="key">要设置的按键。</param>
        /// <param name="pressed">true表示设置为按下，否则为松开。</param>
        public void SetKeyPressed(TasKey key, bool pressed = true) {
            if (pressed) m_KeyFlags |= (1u << (int)key);
            else m_KeyFlags &= ~(1u << (int)key);
        }

        /// <summary>
        /// 反转按键状态。
        /// </summary>
        /// <param name="key">要反转的按键。</param>
        public void FlipKeyPressed(TasKey key) {
            m_KeyFlags ^= (1u << (int)key);
        }

        /// <summary>
        /// 获取或设置Up键的按下状态。
        /// </summary>
        public bool KeyUpPressed { get { return IsKeyPressed(TasKey.KeyUp); } set { SetKeyPressed(TasKey.KeyUp, value); } }
        /// <summary>
        /// 获取或设置Down键的按下状态。
        /// </summary>
        public bool KeyDownPressed { get { return IsKeyPressed(TasKey.KeyDown); } set { SetKeyPressed(TasKey.KeyDown, value); } }
        /// <summary>
        /// 获取或设置Left键的按下状态。
        /// </summary>
        public bool KeyLeftPressed { get { return IsKeyPressed(TasKey.KeyLeft); } set { SetKeyPressed(TasKey.KeyLeft, value); } }
        /// <summary>
        /// 获取或设置Right键的按下状态。
        /// </summary>
        public bool KeyRightPressed { get { return IsKeyPressed(TasKey.KeyRight); } set { SetKeyPressed(TasKey.KeyRight, value); } }
        /// <summary>
        /// 获取或设置Shift键的按下状态。
        /// </summary>
        public bool KeyShiftPressed { get { return IsKeyPressed(TasKey.KeyShift); } set { SetKeyPressed(TasKey.KeyShift, value); } }
        /// <summary>
        /// 获取或设置Space键的按下状态。
        /// </summary>
        public bool KeySpacePressed { get { return IsKeyPressed(TasKey.KeySpace); } set { SetKeyPressed(TasKey.KeySpace, value); } }
        /// <summary>
        /// 获取或设置Q键的按下状态。
        /// </summary>
        public bool KeyQPressed { get { return IsKeyPressed(TasKey.KeyQ); } set { SetKeyPressed(TasKey.KeyQ, value); } }
        /// <summary>
        /// 获取或设置Esc键的按下状态。
        /// </summary>
        public bool KeyEscPressed { get { return IsKeyPressed(TasKey.KeyEsc); } set { SetKeyPressed(TasKey.KeyEsc, value); } }
        /// <summary>
        /// 获取或设置回车键的按下状态。
        /// </summary>
        public bool KeyEnterPressed { get { return IsKeyPressed(TasKey.KeyEnter); } set { SetKeyPressed(TasKey.KeyEnter, value); } }

        /// <summary>
        /// 清除所有按键，将所有按键设置为不按下。
        /// </summary>
        public void ClearKeyPressed() {
            m_KeyFlags = 0;
        }

    }

    /// <summary>
    /// 描述TAS文件中的可能的按键。
    /// </summary>
    public enum TasKey : int {
        KeyUp = 0,
        KeyDown = 1,
        KeyLeft = 2,
        KeyRight = 3,
        KeyShift = 4,
        KeySpace = 5,
        KeyQ = 6,
        KeyEsc = 7,
        KeyEnter = 8,
    }

}
