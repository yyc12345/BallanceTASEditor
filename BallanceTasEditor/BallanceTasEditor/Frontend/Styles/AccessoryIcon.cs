using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BallanceTasEditor.Frontend.Styles {

    public class AccessoryIcon {
        public static ImageSource GetIcon(DependencyObject obj) {
            return (ImageSource)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, ImageSource value) {
            obj.SetValue(IconProperty, value);
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(AccessoryIcon), new PropertyMetadata(null));

        public static double GetIconWidth(DependencyObject obj) {
            return (double)obj.GetValue(IconWidthProperty);
        }

        public static void SetIconWidth(DependencyObject obj, double value) {
            obj.SetValue(IconWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconWidthProperty =
            DependencyProperty.RegisterAttached("IconWidth", typeof(double), typeof(AccessoryIcon), new PropertyMetadata(0.0));

        public static double GetIconHeight(DependencyObject obj) {
            return (double)obj.GetValue(IconHeightProperty);
        }

        public static void SetIconHeight(DependencyObject obj, double value) {
            obj.SetValue(IconHeightProperty, value);
        }

        // Using a DependencyProperty as the backing store for IconHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconHeightProperty =
            DependencyProperty.RegisterAttached("IconHeight", typeof(double), typeof(AccessoryIcon), new PropertyMetadata(0.0));

    }

}
