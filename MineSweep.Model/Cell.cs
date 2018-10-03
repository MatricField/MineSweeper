using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MineSweep.Model
{
    public class Cell :
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
        #pragma warning restore CS0067

        public int X { get; }

        public int Y { get; }

        public CellState State { get; set; }

        public bool IsMine { get; }

        public int ProximalMineCount { get; }

        public Cell():
            this(default, default, default, default)
        {

        }

        public Cell(int x, int y, int proximalMineCount, bool isMine)
        {
            X = x;
            Y = y;
            State = CellState.Unexplored;
            ProximalMineCount = proximalMineCount;
            IsMine = isMine;
        }

        public static Cell CreateMine(int x, int y)
        {
            return new Cell(x, y, -1, true);
        }

        public static Cell CreateRegularCell(int x, int y, int proximalCount)
        {
            return new Cell(x, y, proximalCount, false);
        }
    }
}
