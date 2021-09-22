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
        public TASSlider(TASSliderComponents _components) {
            components = _components;
            components.mSlider.Minimum = 0;
            mIsHorizontalLayout = true;

            components.mSlider.ValueChanged += func_SliderValueChanged;
        }

        public event Action<long> ValueChanged;
        TASSliderComponents components;
        bool mIsHorizontalLayout;

        public void MoveSliderManually(bool isPrev, bool isFast, int fastCount) {
            var step = isFast ? fastCount : 1;
            components.mSlider.Value = Core.Util.Clamp(components.mSlider.Value.ToInt32() + (isPrev ? -1 : 1) * step, components.mSlider.Minimum.ToInt32(), components.mSlider.Maximum.ToInt32());
        }

        public void UpdateRange(TASFile mFile) {
            components.mSlider.Maximum = mFile.mFrameCount == 0 ? 0 : mFile.mFrameCount - 1;
            var index = mFile.GetPointerIndex();
            if (index >= 0) {
                components.mSlider.Value = mFile.GetPointerIndex();
                components.mSlider.IsEnabled = true;
            } else {
                // invalid index, mean slider is useless, disable it
                components.mSlider.Value = components.mSlider.Maximum;
                components.mSlider.IsEnabled = false;
            }
        }

        private void func_SliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            ValueChanged?.Invoke(e.NewValue.ToInt64());
        }

        public void ChangeLayout(bool isHorizontal) {
            if (isHorizontal == mIsHorizontalLayout) return;

            // the layout changed, re-construct elements
            mIsHorizontalLayout = isHorizontal;

            // change container
            components.container.RowDefinitions.Clear();
            components.container.ColumnDefinitions.Clear();
            if (mIsHorizontalLayout) {
                for(int i = 0; i < 4; i++) {
                    UI.Util.GridColumnAdder(components.container, GridLength.Auto);
                }
                UI.Util.GridColumnAdder(components.container, new GridLength(1, GridUnitType.Star));
            } else {
                for (int i = 0; i < 4; i++) {
                    UI.Util.GridRowAdder(components.container, GridLength.Auto);
                }
                UI.Util.GridRowAdder(components.container, new GridLength(1, GridUnitType.Star));
            }

            // flip elements
            UI.Util.SwapGridItemRC(components.btnFastPrev);
            UI.Util.SwapGridItemRC(components.btnPrev);
            UI.Util.SwapGridItemRC(components.btnNext);
            UI.Util.SwapGridItemRC(components.btnFastNext);
            UI.Util.SwapGridItemRC(components.mSlider);

            // change transform
            if (mIsHorizontalLayout) {
                // clear all btn's transform and set slider as horizontal style
                components.btnFastPrev.RenderTransform = Transform.Identity;
                components.btnPrev.RenderTransform = Transform.Identity;
                components.btnNext.RenderTransform = Transform.Identity;
                components.btnFastNext.RenderTransform = Transform.Identity;

                components.mSlider.RenderTransform = Transform.Identity;
                components.mSlider.Orientation = Orientation.Horizontal;
                components.mSlider.VerticalAlignment = VerticalAlignment.Center;
                components.mSlider.HorizontalAlignment = HorizontalAlignment.Stretch;
            } else {
                components.btnFastPrev.RenderTransform = new RotateTransform(90);
                components.btnPrev.RenderTransform = new RotateTransform(90);
                components.btnNext.RenderTransform = new RotateTransform(90);
                components.btnFastNext.RenderTransform = new RotateTransform(90);

                components.mSlider.RenderTransform = new RotateTransform(180);
                components.mSlider.Orientation = Orientation.Vertical;
                components.mSlider.VerticalAlignment = VerticalAlignment.Stretch;
                components.mSlider.HorizontalAlignment = HorizontalAlignment.Center;
            }

        }

    }

    public class TASSliderComponents {
        public Grid container;
        public Button btnFastPrev;
        public Button btnPrev;
        public Button btnNext;
        public Button btnFastNext;
        public Slider mSlider;
    }
}
