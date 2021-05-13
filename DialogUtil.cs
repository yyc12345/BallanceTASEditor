using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BallanceTASEditor {
    public class DialogUtil {

        public static string OpenFileDialog() {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.RestoreDirectory = true;
            op.Multiselect = false;
            op.Filter = "TAS file(*.tas)|*.tas|All file(*.*)|*.*";
            if (!(bool)op.ShowDialog()) return "";
            return op.FileName;
        }
        
        public static string SaveFileDialog() {
            Microsoft.Win32.SaveFileDialog op = new Microsoft.Win32.SaveFileDialog();
            op.RestoreDirectory = true;
            op.Filter = "TAS file(*.tas)|*.tas|All file(*.*)|*.*";
            if (!(bool)op.ShowDialog()) return "";
            return op.FileName;
        }

        public static bool ConfirmDialog(string str) {
            var result = MessageBox.Show(str, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return (result == MessageBoxResult.Yes);
        }

    }
}
