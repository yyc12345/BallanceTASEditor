using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace BallanceTASEditor.UI {
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

        public static bool InputNumber(string title, int min, int max, ref int result) {
            while (true) {
                var dialog = Interaction.InputBox(title, "Input number", "");
                if (dialog == "") return false;
                if (int.TryParse(dialog, out result)) {
                    if (result <= max && result >= min) break;
                }
                MessageBox.Show("Invalid number. Please input again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return true;
        }

    }
}
