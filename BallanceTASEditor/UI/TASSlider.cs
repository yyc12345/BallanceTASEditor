using BallanceTASEditor.Core;
using BallanceTASEditor.Core.TASStruct;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BallanceTASEditor.UI {
    public class TASSlider {
        public TASSlider(Slider slider) {
            mSlider = slider;
            mSlider.Minimum = 0;

            mSlider.ValueChanged += func_SliderValueChanged;
        }

        public event Action<long> ValueChanged;
        Slider mSlider;

        public void MoveSliderManually(bool isPrev, bool isFast, int fastCount) {
            var step = isFast ? fastCount : 1;
            mSlider.Value = Util.Clamp(mSlider.Value.ToInt32() + (isPrev ? -1 : 1) * step, mSlider.Minimum.ToInt32(), mSlider.Maximum.ToInt32());
        }

        public void UpdateRange(TASFile mFile) {
            mSlider.Maximum = mFile.mFrameCount - 1;
            mSlider.Value = mFile.GetPointerIndex();
        }

        private void func_SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ValueChanged?.Invoke(e.NewValue.ToInt64());
        }

    }
}
