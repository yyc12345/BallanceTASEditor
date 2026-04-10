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

        private static readonly Validator.ValidatorAdapter<string, int, Validator.CountValidator> g_CountValidator =
            new Validator.ValidatorAdapter<string, int, Validator.CountValidator>(new Validator.CountValidator());

        public static ValidationResult? ValidateCount(string value, ValidationContext context) {
            return g_CountValidator.Validate(value, context);
        }

        private static readonly Validator.ValidatorAdapter<string, uint, Validator.FpsValidator> g_FpsValidator =
            new Validator.ValidatorAdapter<string, uint, Validator.FpsValidator>(new Validator.FpsValidator());

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
