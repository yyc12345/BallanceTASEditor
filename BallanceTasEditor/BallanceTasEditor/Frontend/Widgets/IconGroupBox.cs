using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BallanceTasEditor.Frontend.Widgets {

    public class IconGroupBox : GroupBox {
        static IconGroupBox() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconGroupBox), new FrameworkPropertyMetadata(typeof(IconGroupBox)));
        }

        public string GroupBoxText {
            get { return (string)GetValue(GroupBoxTextProperty); }
            set { SetValue(GroupBoxTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupBoxText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupBoxTextProperty =
            DependencyProperty.Register("GroupBoxText", typeof(string), typeof(IconGroupBox));

        public ImageSource GroupBoxIcon {
            get { return (ImageSource)GetValue(GroupBoxIconProperty); }
            set { SetValue(GroupBoxIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupBoxIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupBoxIconProperty =
            DependencyProperty.Register("GroupBoxIcon", typeof(ImageSource), typeof(IconGroupBox));

    }
}
