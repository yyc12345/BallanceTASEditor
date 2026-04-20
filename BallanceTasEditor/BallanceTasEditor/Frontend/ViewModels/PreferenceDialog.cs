using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {
    public partial class PreferenceDialog : ObservableValidator {
        public PreferenceDialog() {

        }

        [ObservableProperty]
        private Shared.SequenceKind sequenceKind;
        [ObservableProperty]
        private Shared.EditorLayoutKind layoutKind;
        [ObservableProperty]
        private Shared.EditorPasteBehavior pasteBehavior;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(PreferenceDialog), nameof(ValidateCount))]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string frameCount;
        [ObservableProperty]
        private string gamePath;

        #region Validators

        private static readonly Validators.ValidatorAdapter<string, int, Validators.CountValidator> g_CountValidator =
            new Validators.ValidatorAdapter<string, int, Validators.CountValidator>(new Validators.CountValidator());

        public static ValidationResult? ValidateCount(string value, ValidationContext context) {
            return g_CountValidator.Validate(value, context);
        }

        //public Shared.NewFileDialogResult GetUserInput() {
        //    return
        //}

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
