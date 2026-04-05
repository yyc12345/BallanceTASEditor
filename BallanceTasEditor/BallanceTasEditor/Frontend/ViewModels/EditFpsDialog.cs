using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {
    public partial class EditFpsDialog : ObservableObject {
        public EditFpsDialog() {
            Fps = Shared.Constant.DEFAULT_FPS.ToString();
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string fps;

        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok() {
            OnRequestCloseDialog(true);
        }

        private bool CanOk() {
            // TODO
            return true;
        }

        [RelayCommand]
        private void Cancel() {
            OnRequestCloseDialog(false);
        }


        public event Shared.RequestCloseDialogEventHandler? RequestCloseDialog;

        private void OnRequestCloseDialog(bool result) {
            RequestCloseDialog?.Invoke(new Shared.RequestCloseDialogEventArgs { Result = result });
        }

    }
}
