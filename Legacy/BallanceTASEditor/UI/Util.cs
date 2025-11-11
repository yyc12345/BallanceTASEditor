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

    public static class Util {
        public static void GridRowAdder(Grid container, GridLength size) {
            var item = new RowDefinition();
            item.Height = size;
            container.RowDefinitions.Add(item);
        }

        public static void GridColumnAdder(Grid container, GridLength size) {
            var item = new ColumnDefinition();
            item.Width = size;
            container.ColumnDefinitions.Add(item);
        }

        public static void SwapGridItemRC(UIElement item) {
            var swap = Grid.GetColumn(item);
            Grid.SetColumn(item, Grid.GetRow(item));
            Grid.SetRow(item, swap);
        }
    }

    public enum ToolMode {
        Cursor,
        Fill,
        Overwrite
    }

    public struct CellPosition {
        public CellPosition(int column, FrameDataField field) {
            this.column = column;
            this.field = field;
        }
        public int column;
        public FrameDataField field;
    }
}
