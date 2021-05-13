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
    /// <summary>
    /// TASFlow.xaml 的交互逻辑
    /// </summary>
    public partial class TASFlow : UserControl {
        public TASFlow() {
            InitializeComponent();
            mItemList = new List<TASFlowUIItem>();
            mItemCount = 0;
            SetItemCount(1);
        }

        private int mItemCount;
        private List<TASFlowUIItem> mItemList;
        public List<FrameDataDisplay> DataSources { get; set; }

        public void RefreshDataSources() {
            if (DataSources == null) return;

            for (int i = 0; i < mItemCount; i++) {
                mItemList[i].Reload(DataSources[i]);
            }
        }

        public void SetItemCount(int newCount) {
            var offset = newCount - mItemCount;
            var abs = Math.Abs(offset);
            if (offset == 0) return;

            // change column defination first
            if (offset > 0) {
                for(int i = 0; i < abs; i++) {
                    var item = new ColumnDefinition();
                    item.Width = GridLength.Auto;
                    uiCoreWindow.ColumnDefinitions.Add(item);
                }
            } else {
                uiCoreWindow.ColumnDefinitions.RemoveRange(newCount + 1, abs);  // the first col is sheet header, so add 1 additionally
            }

            // add / remove item
            if (offset > 0) {
                for (int i = 0; i < abs; i++) {
                    var newItem = new TASFlowUIItem(mItemCount + 1 + i);    // the first col is sheet header, so add 1 additionally
                    newItem.Add(uiCoreWindow);
                    mItemList.Add(newItem);
                }
            } else {
                for(int i = 0; i < abs; i++) {
                    mItemList[mItemCount].Remove(uiCoreWindow);
                }
                mItemList.RemoveRange(newCount, abs);
            }

            // apply new count
            mItemCount = newCount;
        }

    }

    public class TASFlowUIItem {
        private static readonly Thickness DEFAULT_MARGIN = new Thickness(2);
        private static readonly Thickness RECT_MARGIN = new Thickness(1);
        private static readonly SolidColorBrush RECT_STROKE = new SolidColorBrush(Colors.Gray);
        private static readonly Color COLOR_SET = Color.FromRgb(30, 144, 255);
        private static readonly Color COLOR_UNSET = Color.FromArgb(0, 255, 255, 255);
        private const int KEY_COUNT = 9;

        public TASFlowUIItem(int column) {
            // basic item
            frame = new TextBlock();
            deltaTime = new TextBlock();

            Grid.SetRow(frame, 0);
            Grid.SetRow(deltaTime, 1);
            Grid.SetColumn(frame, column);
            Grid.SetColumn(deltaTime, column);

            frame.Margin = DEFAULT_MARGIN;
            deltaTime.Margin = DEFAULT_MARGIN;

            // keystates item
            keystates = new Rectangle[KEY_COUNT];
            keystatesFill = new SolidColorBrush[KEY_COUNT];
            for (int i = 0; i < KEY_COUNT; i++) {
                keystates[i] = new Rectangle();
                keystatesFill[i] = new SolidColorBrush(COLOR_UNSET);
                Grid.SetRow(keystates[i], 2 + i);
                Grid.SetColumn(keystates[i], column);
                keystates[i].Margin = RECT_MARGIN;
                keystates[i].StrokeThickness = 1;
                keystates[i].Stroke = RECT_STROKE;
                keystates[i].Fill = keystatesFill[i];
            }
        }

        public void Add(Grid target) {
            target.Children.Add(frame);
            target.Children.Add(deltaTime);
            for (int i = 0; i < KEY_COUNT; i++) {
                target.Children.Add(keystates[i]);
            }
        }

        public void Remove(Grid target) {
            target.Children.Remove(frame);
            target.Children.Remove(deltaTime);
            for (int i = 0; i < KEY_COUNT; i++) {
                target.Children.Remove(keystates[i]);
            }
        }

        public void Reload(FrameDataDisplay fdd) {
            var isEnable = fdd.isEnable;
            frame.Text = isEnable ? fdd.index.ToString() : "";
            deltaTime.Text = isEnable ? fdd.deltaTime.ToString() : "";
            keystatesFill[0].Color = isEnable && fdd.key_up ? COLOR_SET : COLOR_UNSET;
            keystatesFill[1].Color = isEnable && fdd.key_down ? COLOR_SET : COLOR_UNSET;
            keystatesFill[2].Color = isEnable && fdd.key_left ? COLOR_SET : COLOR_UNSET;
            keystatesFill[3].Color = isEnable && fdd.key_right ? COLOR_SET : COLOR_UNSET;
            keystatesFill[4].Color = isEnable && fdd.key_shift ? COLOR_SET : COLOR_UNSET;
            keystatesFill[5].Color = isEnable && fdd.key_space ? COLOR_SET : COLOR_UNSET;
            keystatesFill[6].Color = isEnable && fdd.key_q ? COLOR_SET : COLOR_UNSET;
            keystatesFill[7].Color = isEnable && fdd.key_esc ? COLOR_SET : COLOR_UNSET;
            keystatesFill[8].Color = isEnable && fdd.key_enter ? COLOR_SET : COLOR_UNSET;

            frame.Visibility = isEnable ? Visibility.Visible : Visibility.Collapsed;
            deltaTime.Visibility = isEnable ? Visibility.Visible : Visibility.Collapsed;
            for (int i = 0; i < KEY_COUNT; i++) {
                keystates[i].Visibility = isEnable ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public TextBlock frame;
        public TextBlock deltaTime;
        public Rectangle[] keystates;
        private SolidColorBrush[] keystatesFill;
    }
}
