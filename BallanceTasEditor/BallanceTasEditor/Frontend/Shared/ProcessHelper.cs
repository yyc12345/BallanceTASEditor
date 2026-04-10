using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.Shared {
    public static class ProcessHelper {
        public static void OpenUrl(string url) {
            if (string.IsNullOrWhiteSpace(url)) {
                throw new ArgumentException("The content of URL should not be empty.", nameof(url));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                // Windows 必须设置 UseShellExecute = true 才能识别 URL
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                Process.Start("xdg-open", url);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                Process.Start("open", url);
            } else {
                throw new PlatformNotSupportedException("Not supported operating system.");
            }
        }
    }
}
