using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Backend {
    /// <summary>
    /// FPS converter
    /// </summary>
    public static class FpsConverter {
        /// <summary>
        /// Check if the FPS is valid
        /// </summary>
        /// <param name="fps">FPS in integer</param>
        /// <returns>Is valid</returns>
        public static bool IsValidFps(uint fps) {
            return fps > 0;
        }
        /// <summary>
        /// Check if the FPS is valid
        /// </summary>
        /// <param name="fps">FPS in float point</param>
        /// <returns>Is valid</returns>
        public static bool IsValidFps(float fps) {
            return fps > 0;
        }
        /// <summary>
        /// Check if the delta time is valid
        /// </summary>
        /// <param name="delta">Delta time in float point</param>
        /// <returns>Is valid</returns>
        public static bool IsValidDelta(float delta) {
            return delta > 0;
        }

        /// <summary>
        /// Convert float point delta time to float point FPS
        /// </summary>
        /// <param name="delta">Delta time in float point</param>
        /// <returns>FPS in float point</returns>
        public static float ToFps(float delta) {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(delta, nameof(delta));
            return 1f / delta;
        }

        /// <summary>
        /// Convert float point delta time to integer FPS
        /// </summary>
        /// <param name="delta">Delta time in float point</param>
        /// <returns>FPS in round integer</returns>
        public static uint ToRoundFps(float delta) {
            return Convert.ToUInt32(ToFps(delta));
        }

        /// <summary>
        /// Convert integer FPS to float point delta time
        /// </summary>
        /// <param name="fps">FPS in integer</param>
        /// <returns>Delta time in float point</returns>
        public static float ToDelta(uint fps) {
            return ToDelta((float)fps);
        }

        /// <summary>
        /// Convert float point FPS to float point delta time
        /// </summary>
        /// <param name="fps">FPS in float point</param>
        /// <returns>Delta time in float point</returns>
        public static float ToDelta(float fps) {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fps, nameof(fps));
            return 1f / fps;
        }
    }
}
