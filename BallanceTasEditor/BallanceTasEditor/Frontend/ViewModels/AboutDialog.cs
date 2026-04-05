using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.Frontend.ViewModels {
    public partial class AboutDialog : ObservableObject {

        public AboutDialog() { }

        [RelayCommand]
        private void Ok() {
            OnRequestCloseDialog(true);
        }

        public event Shared.RequestCloseDialogEventHandler? RequestCloseDialog;

        private void OnRequestCloseDialog(bool result) {
            RequestCloseDialog?.Invoke(new Shared.RequestCloseDialogEventArgs { Result = result });
        }

    }
}
