using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace BallanceTasEditor.Frontend.ViewModels {
    public partial class MainWindow : ObservableObject {
        public MainWindow(Shared.IDialogService dialogService) {
            m_DialogService = dialogService;

            TasFile = new Models.TasFile();
            TasFile.PropertyChanged += TasFile_PropertyChanged;
            TasFilePath = null;

            this.StatusMessage = "";
            m_StatusMessageDimmer = new DispatcherTimer();
            m_StatusMessageDimmer.Interval = TimeSpan.FromSeconds(5);
            m_StatusMessageDimmer.Tick += StatusMessageDimmer_Tick;
            UpdateStatusMessage("Ready");
        }

        private Shared.IDialogService m_DialogService;

        #region File Menu

        #region File Operation

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        private Models.TasFile tasFile;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        private string? tasFilePath;

        private void TasFile_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            // YYC MARK:
            // Due to the shitty limit of MVVM Toolkit,
            // I was forced trigger these command manually.
            NewFileCommand.NotifyCanExecuteChanged();
            OpenFileCommand.NotifyCanExecuteChanged();
            SaveFileCommand.NotifyCanExecuteChanged();
            SaveFileAsCommand.NotifyCanExecuteChanged();
            CloseFileCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanNewFile))]
        private void NewFile() {
            // Request new file properties
            var dialog = m_DialogService.ShowNewFileDialog();
            if (dialog is null) return;

            // Create new file
            TasFile.NewFile(Models.TasSequenceKind.Array, dialog.Count, dialog.Fps);
            // Set members
            TasFilePath = null;
            // Send notification
            UpdateStatusMessage($"New TAS file is created.");
        }

        private bool CanNewFile() {
            return TasFile.IsFileNotLoaded;
        }

        [RelayCommand(CanExecute = nameof(CanOpenFile))]
        private void OpenFile() {
            // Request file path
            var dialog = m_DialogService.ShowOpenFileDialog();
            if (dialog is null) return;

            // Load file
            try {
                TasFile.LoadFile(Models.TasSequenceKind.Array, dialog.Path);
            } catch (Exception e) {
                m_DialogService.ShowOpenFileFailedDialog(e);
                return;
            }
            // Set members
            TasFilePath = dialog.Path;
            // Send notification
            UpdateStatusMessage($"TAS file {this.TasFilePath} is loaded.");
        }

        private bool CanOpenFile() {
            return TasFile.IsFileNotLoaded;
        }

        [RelayCommand(CanExecute = nameof(CanSaveFile))]
        private void SaveFile() {
            // If there is no associated file path,
            // it means that this file is not stored on disk,
            // We must make a request to user for fetching it.
            string? filePath = this.TasFilePath;
            if (filePath is null) {
                var dialog = m_DialogService.ShowSaveFileDialog();
                if (dialog is null) return;
                filePath = dialog.Path;
            }

            // Save file
            try {
                TasFile.SaveFile(filePath);
            } catch (Exception e) {
                m_DialogService.ShowSaveFileFailedDialog(e);
                return;
            }
            // Update member
            TasFilePath = filePath;
            // Send notification
            UpdateStatusMessage($"TAS file {this.TasFilePath} is saved.");
        }

        private bool CanSaveFile() {
            return TasFile.IsFileLoaded;
        }

        [RelayCommand(CanExecute = nameof(CanSaveFileAs))]
        private void SaveFileAs() {
            // We always request a new path when saving as
            var dialog = m_DialogService.ShowSaveFileDialog();
            if (dialog is null) return;

            // Save file
            try {
                TasFile.SaveFile(dialog.Path);
            } catch (Exception e) {
                m_DialogService.ShowSaveFileFailedDialog(e);
                return;
            }
            // Set file path
            TasFilePath = dialog.Path;
            // Send notification
            UpdateStatusMessage($"TAS file {this.TasFilePath} is saved.");
        }
        
        private bool CanSaveFileAs() {
            return TasFile.IsFileLoaded;
        }

        [RelayCommand(CanExecute = nameof(CanCloseFile))]
        private void CloseFile() {
            // Close file
            TasFile.CloseFile();
            // Set members
            TasFilePath = null;
            // Send notification
            UpdateStatusMessage($"TAS file is closed.");
        }

        private bool CanCloseFile() {
            return TasFile.IsFileLoaded;
        }

        #endregion

        #region Exit Stuff

        [RelayCommand]
        private void Exit() {
            // TODO
            OnRequestCloseWindow();
        }

        public event Shared.RequestCloseWindowEventHandler? RequestCloseWindow;

        private void OnRequestCloseWindow() {
            RequestCloseWindow?.Invoke();
        }

        #endregion

        #endregion

        #region Edit Menu


        #region Preference

        [RelayCommand]
        private void Preference() {
            m_DialogService.ShowPreferenceDialog();
        }

        #endregion

        #endregion

        #region Help Menu

        [RelayCommand]
        private void ReportBug() {
            try {
                Shared.ProcessHelper.OpenUrl(Shared.Constant.REPORT_BUG_URL);
            } catch (Exception) {
                m_DialogService.ShowManuallyReportBugDialog();
            }
        }

        [RelayCommand]
        private void About() {
            m_DialogService.ShowAboutDialog();
        }

        #endregion

        #region Status Bar

        #region Status Message

        [ObservableProperty]
        private string statusMessage;

        private DispatcherTimer m_StatusMessageDimmer;

        private void UpdateStatusMessage(string msg) {
            m_StatusMessageDimmer.Stop();
            StatusMessage = msg;
            m_StatusMessageDimmer.Start();
        }

        private void StatusMessageDimmer_Tick(object? sender, EventArgs e) {
            StatusMessage = "";
        }

        #endregion

        #endregion

        #region UI Only

        public string WindowTitle {
            get {
                if (TasFile.IsFileNotLoaded) {
                    return "Ballance TAS Editor";
                } else {
                    if (TasFilePath is null) {
                        return "Ballance TAS Editor - [Untitled]";
                    } else {
                        return $"Ballance TAS Editor - [{TasFilePath}]";
                    }
                }
            }
        }

        #endregion

    }
}
