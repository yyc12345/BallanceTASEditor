using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace BallanceTASEditor {
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application {

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

#if DEBUG
#else
            AppDomain.CurrentDomain.UnhandledException += (sender, ex) => {
                if (ex.ExceptionObject is System.Exception) {
                    var exx = (System.Exception)ex.ExceptionObject;
                    UncatchedErrorHandle(exx.Message, exx.StackTrace);
                }
            };
#endif
        }

        private void UncatchedErrorHandle(string message, string stackTrace) {
            try {
                if (!Directory.Exists("./logs"))
                    Directory.CreateDirectory("./logs");

                int counter = 1;
                var filename = "";
                var datetime = DateTime.Now;
                while (true) {
                    filename = $"./logs/crash-{datetime.ToString("yyyyMMddHHmmss")}-{counter.ToString().PadLeft(2, '0')}.log";
                    if (!File.Exists(filename)) break;
                }

                var fs = new StreamWriter(filename, false, Encoding.UTF8);
                fs.WriteLine("[SYS][ERROR] FATAL ERROR !");
                fs.WriteLine(message);
                fs.WriteLine(stackTrace);
                fs.Close();
                fs.Dispose();
            } catch {
                ;//skip
            }

            MessageBox.Show("A fatal error occurs. The application should exit. Please send the error log, reproduce step and corresponding TAS file(is possible) to use to help fixing this problem.", "Ballance TAS Editor", MessageBoxButton.OK, MessageBoxImage.Error);
            App.Current.Shutdown();
        }


    }
}
