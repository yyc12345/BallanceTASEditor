using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using BallanceTASEditor.Core;

namespace BallanceTASEditor.UI {
    public class DialogUtil {

        public static string OpenFileDialog() {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.RestoreDirectory = true;
            op.Multiselect = false;
            op.Filter = I18NProcessor.GetI18N("code_DialogUtil_FileFilter");
            if (!(bool)op.ShowDialog()) return "";
            return op.FileName;
        }

        public static string SaveFileDialog() {
            Microsoft.Win32.SaveFileDialog op = new Microsoft.Win32.SaveFileDialog();
            op.RestoreDirectory = true;
            op.Filter = I18NProcessor.GetI18N("code_DialogUtil_FileFilter");
            if (!(bool)op.ShowDialog()) return "";
            return op.FileName;
        }

        public static bool ConfirmDialog(string str) {
            var result = MessageBox.Show(str, I18NProcessor.GetI18N("code_DialogUtil_Warning"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return (result == MessageBoxResult.Yes);
        }

        public static bool InputNumber(string title, int min, int max, ref int result) {
            while (true) {
                var dialog = Interaction.InputBox(title, I18NProcessor.GetI18N("code_DialogUtil_InputNumber_Title"), "");
                if (dialog == "") return false;
                if (int.TryParse(dialog, out result)) {
                    if (result <= max && result >= min) break;
                }
                MessageBox.Show(I18NProcessor.GetI18N("code_DialogUtil_InputNumber_Wrong"), 
                    I18NProcessor.GetI18N("code_DialogUtil_Warning"), 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return true;
        }

        public static bool AddItemDialog(out int count, out float deltaTime) {
            var win = new AddItem();
            if (!(bool)win.ShowDialog()) {
                count = 0;
                deltaTime = 0f;
                return false;
            }

            count = win.Output_Count;
            deltaTime = win.Output_DeltaTime;
            return true;
        }

    }
}
