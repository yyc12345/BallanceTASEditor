using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {

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


}
