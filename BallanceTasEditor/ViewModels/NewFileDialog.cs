using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BallanceTasEditor.ViewModels {
    public partial class NewFileDialog : ObservableObject {
        public NewFileDialog() {
            Count = 10000;
            // 132 or 264
            Fps = 264;
        }

        [ObservableProperty]
        private int count;

        [ObservableProperty]
        private int fps;

    }
}
