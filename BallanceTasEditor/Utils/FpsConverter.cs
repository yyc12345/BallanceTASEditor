using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Utils {
    public static class FpsConverter {
        public static float ToFps(float delta) {
            if (delta <= 0f)
                throw new ArgumentOutOfRangeException("invalid time delta (not positive)");

            return 1f / delta;
        }

        public static uint ToFloorFps(float delta) {
            return (uint)Math.Floor(ToFps(delta));
        }

        public static float ToDelta(uint fps) {
            return ToDelta((float)fps);
        }

        public static float ToDelta(float fps) {
            if (fps <= 0f)
                throw new ArgumentOutOfRangeException("invalid fps (not positive)");

            return 1f / fps;
        }
    }
}
