using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BallanceTasEditor.Frontend.Shared {

    public interface IDialogService {
        NewFileDialogResult? ShowNewFileDialog();
        OpenFileDialogResult? ShowOpenFileDialog();
        void ShowOpenFileFailedDialog(Exception e);
        SaveFileDialogResult? ShowSaveFileDialog();
        void ShowSaveFileFailedDialog(Exception e);
        bool ShowConfirmCloseFileDialog(string message);
        bool ShowConfirmExitWhenOpeningFileDialog();
        bool ShowFileChangedDialog();
        GotoDialogResult? ShowGotoDialog();
        EditFpsDialogResult? ShowEditFpsDialog();
        AddFrameDialogResult? ShowAddFrameDialog();
        PreferenceDialogResult? ShowPreferenceDialog();
        void ShowManuallyReportBugDialog();
        void ShowAboutDialog();
    }

    public record NewFileDialogResult {
        public required uint Fps { get; init; }
        public required int Count { get; init; }
    }

    public record OpenFileDialogResult {
        public required string Path { get; init; }
    }

    public record SaveFileDialogResult {
        public required string Path { get; init; }
    }

    public record GotoDialogResult { }

    public record EditFpsDialogResult { }

    public record AddFrameDialogResult { }

    public record PreferenceDialogResult { }

    public class DialogService : IDialogService {
        public DialogService(Window parent) {
            m_Parent = parent;
        }

        private readonly Window m_Parent;

        public NewFileDialogResult? ShowNewFileDialog() {
            var dialog = new Views.NewFileDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new NewFileDialogResult() { Count = 0, Fps = 60 };
            } else {
                return null;
            }
        }

        public OpenFileDialogResult? ShowOpenFileDialog() {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.RestoreDirectory = true;
            op.Multiselect = false;
            op.Filter = "TAS file(*.tas)|*.tas|All file(*.*)|*.*";
            if (op.ShowDialog() is true) {
                return new OpenFileDialogResult() { Path = op.FileName };
            } else {
                return null;
            }
        }

        public void ShowOpenFileFailedDialog(Exception e) {
            MessageBox.Show("Fail to open file. This file might not a legal TAS file." + e.Message,
                "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public SaveFileDialogResult? ShowSaveFileDialog() {
            Microsoft.Win32.SaveFileDialog op = new Microsoft.Win32.SaveFileDialog();
            op.RestoreDirectory = true;
            op.Filter = "TAS file(*.tas)|*.tas|All file(*.*)|*.*";
            if (op.ShowDialog() is true) {
                return new SaveFileDialogResult() { Path = op.FileName };
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

        public GotoDialogResult? ShowGotoDialog() {
            var dialog = new Views.GotoDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new GotoDialogResult();
            } else {
                return null;
            }
        }

        public EditFpsDialogResult? ShowEditFpsDialog() {
            var dialog = new Views.EditFpsDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new EditFpsDialogResult();
            } else {
                return null;
            }
        }

        public AddFrameDialogResult? ShowAddFrameDialog() {
            var dialog = new Views.AddFrameDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new AddFrameDialogResult();
            } else {
                return null;
            }
        }

        public PreferenceDialogResult? ShowPreferenceDialog() {
            var dialog = new Views.PreferenceDialog();
            dialog.Owner = m_Parent;
            if (dialog.ShowDialog() is true) {
                // TODO: Finish result extraction
                return new PreferenceDialogResult();
            } else {
                return null;
            }
        }

        public void ShowManuallyReportBugDialog() {
            MessageBox.Show($"We can not open browser automatically for you. Please visit {Shared.Constant.REPORT_BUG_URL} manually to report bug.",
                "Can not Open Browser",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowAboutDialog() {
            var dialog = new Views.AboutDialog();
            dialog.Owner = m_Parent;
            dialog.ShowDialog();
        }

    }

}
