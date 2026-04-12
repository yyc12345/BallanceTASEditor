using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {

    public partial class NewFileDialog : ObservableValidator {
        public NewFileDialog() {
            Count = Shared.Constant.DEFAULT_NEW_COUNT.ToString();
            Fps = Shared.Constant.DEFAULT_FPS.ToString();
        }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(NewFileDialog), nameof(ValidateCount))]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string count;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(NewFileDialog), nameof(ValidateFps))]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string fps;

        #region Validators

        private static readonly Validators.ValidatorAdapter<string, int, Validators.CountValidator> g_CountValidator =
            new Validators.ValidatorAdapter<string, int, Validators.CountValidator>(new Validators.CountValidator());

        public static ValidationResult? ValidateCount(string value, ValidationContext context) {
            return g_CountValidator.Validate(value, context);
        }

        private static readonly Validators.ValidatorAdapter<string, uint, Validators.FpsValidator> g_FpsValidator =
            new Validators.ValidatorAdapter<string, uint, Validators.FpsValidator>(new Validators.FpsValidator());

        public static ValidationResult? ValidateFps(string value, ValidationContext context) {
            return g_FpsValidator.Validate(value, context);
        }

        public Shared.NewFileDialogResult GetUserInput() {
            return new Shared.NewFileDialogResult {
                Count = g_CountValidator.Conclude(Count),
                Fps = g_FpsValidator.Conclude(Fps)
            };
        }

        #endregion

        #region Commands

        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok() {
            OnRequestCloseDialog(true);
        }

        private bool CanOk() {
            return !HasErrors;
        }

        [RelayCommand]
        private void Cancel() {
            OnRequestCloseDialog(false);
        }

        public event Shared.RequestCloseDialogEventHandler? RequestCloseDialog;

        private void OnRequestCloseDialog(bool result) {
            RequestCloseDialog?.Invoke(new Shared.RequestCloseDialogEventArgs { Result = result });
        }

        #endregion

    }
}
