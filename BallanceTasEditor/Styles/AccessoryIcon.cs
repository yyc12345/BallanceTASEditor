using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BallanceTasEditor.Styles {

    public class AccessoryIcon {
        public static Object GetAccessoryIcon(DependencyObject obj) {
            return (Object)obj.GetValue(AccessoryIconProperty);
        }

        public static void SetAccessoryIcon(DependencyObject obj, Object value) {
            obj.SetValue(AccessoryIconProperty, value);
        }

        public static readonly DependencyProperty AccessoryIconProperty =
            DependencyProperty.RegisterAttached("AccessoryIcon", typeof(Object), typeof(AccessoryIcon), new PropertyMetadata(null));
    }

    public class AccessoryIconContentTemplateSelector : DataTemplateSelector {
        private static ImageSourceConverter Converter = new ImageSourceConverter();

        /// <summary>
        /// 给定的图标是已经加载的数据。
        /// </summary>
        public DataTemplate LoadedIconTemplate { get; set; }
        /// <summary>
        /// 给定的图标是来源字符串。
        /// </summary>
        public DataTemplate SourceIconTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            var ctl = VisualTreeHelper.GetParent(container);
            var accessoryIcon = ctl.GetValue(AccessoryIcon.AccessoryIconProperty);

            if (accessoryIcon is null || !Converter.CanConvertFrom(accessoryIcon.GetType())) {
                return LoadedIconTemplate;
            } else {
                return SourceIconTemplate;
            }
        }
    }

}
