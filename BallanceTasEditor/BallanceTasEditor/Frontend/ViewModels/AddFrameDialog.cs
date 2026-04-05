using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {
    public partial class AddFrameDialog : ObservableObject {
        public AddFrameDialog() {
            Count = Shared.Constant.DEFAULT_INSERT_COUNT.ToString();
            Fps = Shared.Constant.DEFAULT_FPS.ToString();
        }

        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok() {
            OnRequestCloseDialog(true);
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string count;
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string fps;

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
