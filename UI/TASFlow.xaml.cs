﻿using BallanceTASEditor.Core.TASStruct;
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
            mRectMap = new Dictionary<Rectangle, CellPosition>();
            mItemCount = 0;
            SetItemCount(1);
        }

        private int mItemCount;
        private List<TASFlowUIItem> mItemList;
        private Dictionary<Rectangle, CellPosition> mRectMap;
        public SelectionHelp SelectionHelp { get; set; }
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
                    newItem.Add(uiCoreWindow, mRectMap, Rectangle_MouseDown);
                    mItemList.Add(newItem);
                }
            } else {
                for(int i = 0; i < abs; i++) {
                    mItemList[newCount + i].Remove(uiCoreWindow, mRectMap, Rectangle_MouseDown);
                }
                mItemList.RemoveRange(newCount, abs);
            }

            // apply new count
            mItemCount = newCount;
        }

        public void RefreshDataMenu() {
            if (SelectionHelp == null) return;

            ToolMode mode = SelectionHelp.GetToolMode();
            bool showCursorPasteAdd = mode == ToolMode.Cursor && SelectionHelp.IsDataPartialReady();
            bool showFill = mode == ToolMode.Fill && SelectionHelp.IsDataReady();
            bool showCursorCopyDelete = mode == ToolMode.Cursor && SelectionHelp.IsDataReady();

            uiDataMenu_Set.IsEnabled = showFill;
            uiDataMenu_Unset.IsEnabled = showFill;
            uiDataMenu_Copy.IsEnabled = showCursorCopyDelete;
            uiDataMenu_Delete.IsEnabled = showCursorCopyDelete;
            uiDataMenu_PasteAfter.IsEnabled = showCursorPasteAdd;
            uiDataMenu_PasteBefore.IsEnabled = showCursorPasteAdd;
            uiDataMenu_AddAfter.IsEnabled = showCursorPasteAdd;
            uiDataMenu_AddBefore.IsEnabled = showCursorPasteAdd;
        }

        public void RefreshSelectionHighlight() {
            ToolMode mode = SelectionHelp.GetToolMode();

            if (mode == ToolMode.Cursor) {
                if (SelectionHelp.IsDataReady()) {
                    var data = SelectionHelp.GetRange();
                    foreach (var item in mItemList) {
                        if (data.Within(item.rawFrame)) {
                            item.SelectFull();
                        } else {
                            item.Unselect();
                        }
                    }
                    return;
                } else if (SelectionHelp.IsDataPartialReady()) {
                    var data = SelectionHelp.GetPoint();
                    foreach (var item in mItemList) {
                        if (data == item.rawFrame) {
                            item.SelectFull();
                        } else {
                            item.Unselect();
                        }
                    }
                    return;
                }
            } else if (mode == ToolMode.Fill) {
                if (SelectionHelp.IsDataReady()) {
                    var data = SelectionHelp.GetRange();
                    var field = SelectionHelp.GetFieldRange();
                    foreach (var item in mItemList) {
                        if (data.Within(item.rawFrame)) {
                            item.SelectField(field);
                        } else {
                            item.Unselect();
                        }
                    }
                    return;
                }
            }

            // fail if prog run into this position, clear
            foreach (var item in mItemList) {
                item.Unselect();
            }
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e) {
            if (SelectionHelp == null) return;

            // because the first column is header
            // so all pos.column should -1
            var rect = sender as Rectangle;
            var pos = mRectMap[rect];
            if (!mItemList[pos.column - 1].rawIsEnable) return;
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed) {
                SelectionHelp.FirstClick(mItemList[pos.column - 1].rawFrame, pos.field);
            } else if (e.MouseDevice.RightButton == MouseButtonState.Pressed) {
                SelectionHelp.LastClick(mItemList[pos.column - 1].rawFrame, pos.field);
            }
        }

    }

    public class TASFlowUIItem {
        private static readonly Thickness DEFAULT_MARGIN = new Thickness(2);
        private static readonly Thickness RECT_MARGIN = new Thickness(1);
        private static readonly SolidColorBrush SEL_RECT_NORMAL_BRUSH = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush SEL_RECT_SELECTED_BRUSH = new SolidColorBrush(Colors.Orange);
        private static readonly SolidColorBrush SEL_RECT_STROKE = new SolidColorBrush(Colors.Gray);
        private static readonly Color COLOR_SET = Color.FromRgb(30, 144, 255);
        private static readonly Color COLOR_UNSET = Color.FromArgb(0, 255, 255, 255);
        private static readonly Color COLOR_SELECTED = Colors.Orange;
        private static readonly Color COLOR_UNSELECTED = Colors.Gray;
        private const int KEY_COUNT = 9;
        private const double SELECTION_HEADER_HEIGHT = 10.0f;

        public TASFlowUIItem(int column) {
            // basic item
            sel_rect = new Rectangle();
            frame = new TextBlock();
            deltaTime = new TextBlock();

            Grid.SetRow(sel_rect, 0);
            Grid.SetRow(frame, 1);
            Grid.SetRow(deltaTime, 2);
            Grid.SetColumn(sel_rect, column);
            Grid.SetColumn(frame, column);
            Grid.SetColumn(deltaTime, column);

            sel_rect.Margin = RECT_MARGIN;
            frame.Margin = DEFAULT_MARGIN;
            deltaTime.Margin = DEFAULT_MARGIN;

            sel_rect.StrokeThickness = 3;
            sel_rect.Stroke = SEL_RECT_STROKE;
            sel_rect.Height = SELECTION_HEADER_HEIGHT;

            // keystates item
            keystates = new Rectangle[KEY_COUNT];
            keystatesFill = new SolidColorBrush[KEY_COUNT];
            keystatesStroke = new SolidColorBrush[KEY_COUNT];
            for (int i = 0; i < KEY_COUNT; i++) {
                keystates[i] = new Rectangle();
                keystatesFill[i] = new SolidColorBrush(COLOR_UNSET);
                keystatesStroke[i] = new SolidColorBrush(COLOR_UNSELECTED);
                Grid.SetRow(keystates[i], 3 + i);
                Grid.SetColumn(keystates[i], column);
                keystates[i].Margin = RECT_MARGIN;
                keystates[i].StrokeThickness = 3;
                keystates[i].Stroke = keystatesStroke[i];
                keystates[i].Fill = keystatesFill[i];
            }

            this.column = column;
            rawFrame = 0;
            rawIsEnable = false;
        }

        public void Add(Grid target, Dictionary<Rectangle, CellPosition> map, MouseButtonEventHandler func) {
            target.Children.Add(sel_rect);
            target.Children.Add(frame);
            target.Children.Add(deltaTime);
            for (int i = 0; i < KEY_COUNT; i++) {
                target.Children.Add(keystates[i]);
                keystates[i].MouseDown += func;
                map.Add(keystates[i], new CellPosition(column, (FrameDataField)i));
            }
        }

        public void Remove(Grid target, Dictionary<Rectangle, CellPosition> map, MouseButtonEventHandler func) {
            target.Children.Remove(sel_rect);
            target.Children.Remove(frame);
            target.Children.Remove(deltaTime);
            for (int i = 0; i < KEY_COUNT; i++) {
                target.Children.Remove(keystates[i]);
                keystates[i].MouseDown -= func;
                map.Remove(keystates[i]);
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

            rawIsEnable = isEnable;
            rawFrame = fdd.index;
        }

        public void SelectFull() {
            sel_rect.Fill = SEL_RECT_SELECTED_BRUSH;
        }

        public void SelectField(SelectionRange range) {
            for (int i = 0; i < KEY_COUNT; i++) {
                keystatesStroke[i].Color = range.Within(i) ? COLOR_SELECTED : COLOR_UNSELECTED;
            }
        }

        public void Unselect() {
            sel_rect.Fill = SEL_RECT_NORMAL_BRUSH;
            for (int i = 0; i < KEY_COUNT; i++) {
                keystatesStroke[i].Color = COLOR_UNSELECTED;
            }
        }

        public long rawFrame;
        public bool rawIsEnable;

        public Rectangle sel_rect;
        public TextBlock frame;
        public TextBlock deltaTime;
        public Rectangle[] keystates;
        private SolidColorBrush[] keystatesFill;
        private SolidColorBrush[] keystatesStroke;
        private int column;
    }
}
