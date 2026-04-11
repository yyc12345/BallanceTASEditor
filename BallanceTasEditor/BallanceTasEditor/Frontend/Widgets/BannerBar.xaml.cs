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
    /// <summary>
    /// Interaction logic for BannerBar.xaml
    /// </summary>
    public partial class BannerBar : UserControl {
        public BannerBar() {
            InitializeComponent();
        }

        public string BannerText {
            get { return (string)GetValue(BannerTextProperty); }
            set { SetValue(BannerTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BannerText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BannerTextProperty =
            DependencyProperty.Register("BannerText", typeof(string), typeof(BannerBar));

        public ImageSource BannerIcon {
            get { return (ImageSource)GetValue(BannerIconProperty); }
            set { SetValue(BannerIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BannerIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BannerIconProperty =
            DependencyProperty.Register("BannerIcon", typeof(ImageSource), typeof(BannerBar));

    }
}
