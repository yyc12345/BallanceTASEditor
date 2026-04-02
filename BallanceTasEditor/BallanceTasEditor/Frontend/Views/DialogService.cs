using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BallanceTasEditor.Frontend.Views {

    public class DialogService : ViewModels.IDialogService {
        public DialogService(Window parent) {
            m_Parent = parent;
        }

        private readonly Window m_Parent;

        public ViewModels.NewFileDialogResult? ShowNewFileDialog() {
            var dialog = new NewFileDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new ViewModels.NewFileDialogResult() { Count = 0, Fps = 60 };
            } else {
                return null;
            }
        }

        public ViewModels.OpenFileDialogResult? ShowOpenFileDialog() {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.RestoreDirectory = true;
            op.Multiselect = false;
            op.Filter = "TAS file(*.tas)|*.tas|All file(*.*)|*.*";
            if (op.ShowDialog() is true) {
                return new ViewModels.OpenFileDialogResult() { Path = op.FileName };
            } else {
                return null;
            }
        }

        public void ShowOpenFileFailedDialog(Exception e) {
            MessageBox.Show("Fail to open file. This file might not a legal TAS file." + e.Message,
                "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public ViewModels.SaveFileDialogResult? ShowSaveFileDialog() {
            Microsoft.Win32.SaveFileDialog op = new Microsoft.Win32.SaveFileDialog();
            op.RestoreDirectory = true;
            op.Filter = "TAS file(*.tas)|*.tas|All file(*.*)|*.*";
            if (op.ShowDialog() is true) {
                return new ViewModels.SaveFileDialogResult() { Path = op.FileName };
            } else {
                return null;
            }
        }

        public void ShowSaveFileFailedDialog(Exception e) {
            MessageBox.Show(
                "Fail to save file." + e.Message,
                "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool ShowConfirmCloseFileDialog(string message) {
            var rv = MessageBox.Show(
                "Do you want to close this TAS file? All changes will not be saved.",
                "File Is Not Saved",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return rv == MessageBoxResult.Yes;
        }

        public bool ShowConfirmExitWhenOpeningFileDialog() {
            var rv = MessageBox.Show(
                "File is not closed. Do you want to just quit? All changes will not be saved.",
                "File Is Not Saved",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return rv == MessageBoxResult.Yes;
        }

        public bool ShowFileChangedDialog() {
            var rv = MessageBox.Show(
                "File is changed. Do you want to reload it?",
                "File Is Changed",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
            return rv == MessageBoxResult.Yes;
        }

        public ViewModels.GotoDialogResult? ShowGotoDialog() {
            var dialog = new GotoDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new ViewModels.GotoDialogResult();
            } else {
                return null;
            }
        }

        public ViewModels.EditFpsDialogResult? ShowEditFpsDialog() {
            var dialog = new EditFpsDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new ViewModels.EditFpsDialogResult();
            } else {
                return null;
            }
        }

        public ViewModels.AddFrameDialogResult? ShowAddFrameDialog() {
            var dialog = new AddFrameDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new ViewModels.AddFrameDialogResult();
            } else {
                return null;
            }
        }

        public ViewModels.PreferenceDialogResult? ShowPreferenceDialog() {
            var dialog = new PreferenceDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new ViewModels.PreferenceDialogResult();
            } else {
                return null;
            }
        }

        public void ShowAboutDialog() {
            var dialog = new AboutDialog();
            dialog.Owner = m_Parent;
            dialog.ShowDialog();
        }
    }

}
