using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {
    public partial class MainWindow : ObservableObject {
        public MainWindow(IDialogService dialogService) {
            m_DialogService = dialogService;

            this.TasFile = null;
            this.TasFilePath = null;
        }

        private IDialogService m_DialogService;

        #region File Operation

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        [NotifyCanExecuteChangedFor(nameof(NewFileCommand))]
        [NotifyCanExecuteChangedFor(nameof(OpenFileCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveFileCommand))]
        [NotifyCanExecuteChangedFor(nameof(SaveFileAsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CloseFileCommand))]
        private Backend.ITasSequence? tasFile;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(WindowTitle))]
        private string? tasFilePath;

        [RelayCommand(CanExecute = nameof(CanNewFile))]
        private void NewFile() {
            // Request new file properties
            var dialog = m_DialogService.ShowNewFileDialog();
            if (dialog is null) return;

            // Initialize sequence
            var seq = new Backend.ListTasSequence();
            // Initialize items
            Backend.TasStorage.Init(seq, dialog.Count, dialog.Fps);
            // Set members
            this.TasFile = seq;
            this.TasFilePath = null;
        }

        private bool CanNewFile() {
            return this.TasFile is null;
        }

        [RelayCommand(CanExecute = nameof(CanOpenFile))]
        private void OpenFile() {
            // Request file path
            var dialog = m_DialogService.ShowOpenFileDialog();
            if (dialog is null) return;

            // Initialize sequence
            var seq = new Backend.ListTasSequence();
            // Load into sequence
            try {
                Backend.TasStorage.Load(dialog.Path, seq);
            } catch (Exception e) {
                m_DialogService.ShowOpenFileFailedDialog(e);
                return;
            }
            // Set members
            this.TasFile = seq;
            this.TasFilePath = dialog.Path;
        }

        private bool CanOpenFile() {
            return this.TasFile is null;
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
                Backend.TasStorage.Save(filePath, this.TasFile.Unwrap());
            } catch (Exception e) {
                m_DialogService.ShowSaveFileFailedDialog(e);
                return;
            }
            // Update member
            this.TasFilePath = filePath;
        }

        private bool CanSaveFile() {
            return this.TasFile is not null;
        }

        [RelayCommand(CanExecute = nameof(CanSaveFileAs))]
        private void SaveFileAs() {
            // We always request a new path when saving as
            var dialog = m_DialogService.ShowSaveFileDialog();
            if (dialog is null) return;

            // Save file
            try {
                Backend.TasStorage.Save(dialog.Path, this.TasFile.Unwrap());
            } catch (Exception e) {
                m_DialogService.ShowSaveFileFailedDialog(e);
                return;
            }
            // Set file path
            TasFilePath = dialog.Path;
        }
        
        private bool CanSaveFileAs() {
            return this.TasFile is not null;
        }

        [RelayCommand(CanExecute = nameof(CanCloseFile))]
        private void CloseFile() {
            this.TasFile = null;
            this.TasFilePath = null;
        }

        private bool CanCloseFile() {
            return this.TasFile is not null;
        }

        #endregion

        #region UI Only

        public string WindowTitle {
            get {
                if (TasFile is null) {
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
