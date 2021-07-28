using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BallanceTASEditor.UI {
    /// <summary>
    /// AddItem.xaml 的交互逻辑
    /// </summary>
    public partial class AddItem : Window {
        public AddItem() {
            InitializeComponent();
        }

        public int Output_Count { get; private set; }
        public float Output_DeltaTime { get; private set; }

        private void funcBtn_OK(object sender, RoutedEventArgs e) {
            int count;
            float fps;
            if (!int.TryParse(uiTextbox_Count.Text, out count)) return;
            if (!float.TryParse(uiTextbox_FPS.Text, out fps)) return;

            Output_Count = count;
            Output_DeltaTime = 1000f / fps;

            this.DialogResult = true;
        }

        private void funcBtn_Cancel(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }
    }
}
