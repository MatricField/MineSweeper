using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using static MineSweep.Model.CellState;

namespace MineSweep.Model
{
    public class Game:
        INotifyPropertyChanged
    {
        #pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
        #pragma warning restore CS0067

        public event EventHandler<ExplosionEventArgs> Exploded;

        protected CellCollection CellData { get; set; }

        protected int Version { get; set; }

        public GameState GameState { get; protected set; }

        public Difficulty Difficulty { get; protected set; }

        public int Width => Difficulty.Width;

        public int Height => Difficulty.Height;

        public IEnumerable<Cell> Cells => CellData;

        public int MineCount => Difficulty.MineCount;

        public int UnmarkedMines { get; protected set; }

        public Game()
        {
            Version = default;
            Difficulty = Difficulty.Beginner;
            CellData = GenerateEmptyCollection();
            GameState = GameState.PreStart;
        }

        public void Reset()
        {
            GameState = GameState.PreStart;
            CellData = GenerateEmptyCollection();
            ++Version;
        }

        protected CellCollection GenerateEmptyCollection()
        {
            var ret = new CellCollection();
            for(int i = 0; i < Width; ++i)
            {
                for(int j = 0; j < Height; ++j)
                {
                    ret.Add(Cell.CreateRegularCell(i, j, 0));
                }
            }
            return ret;
        }

        public void Initialize(int firstClickX, int firstClickY)
        {
            const int isMine = -1;

            var cellCounting = new int[Width, Height];
            var rand = new Random();
            for(var k = 0; k != MineCount; ++k)
            {
                int x, y;
                bool notToBeMine;
                do
                {
                    x = rand.Next(Width);
                    y = rand.Next(Height);
                    notToBeMine = 
                        cellCounting[x, y] == isMine ||
                        (x == firstClickX && y == firstClickY);
                } while (notToBeMine);

                cellCounting[x, y] = isMine;
                foreach(var (x1, y1) in GetSurroundingCellsOf(x, y, (i, j) => CheckIndex(i, j) && cellCounting[i, j] != isMine))
                {
                    ++cellCounting[x1, y1];
                }
            }

            var cells = new CellCollection();

            for(var x = 0; x != Width; ++x)
            {
                for(var y = 0; y != Height; ++y)
                {
                    var proximalCount = cellCounting[x, y];
                    var cellIsMine = proximalCount == isMine;
                    if(cellIsMine)
                    {
                        cells.Add(Cell.CreateMine(x, y));
                    }
                    else
                    {
                        cells.Add(Cell.CreateRegularCell(x, y, proximalCount));
                    }
                }
            }

            CellData = cells;
            ++Version;
            Explore(firstClickX, firstClickY);
            GameState = GameState.OnGoing;
        }

        public void ChangeDifficulty(in Difficulty newDifficulty)
        {
            if(GameState == GameState.OnGoing)
            {
                throw new InvalidOperationException("Cannot change difficulty while game is in running state");
            }
            Difficulty = newDifficulty;
        }

        public void Explore(int x, int y)
        {
            var cell = CellData[(x, y)];
            if(cell.State == Explored)
            {
                return;
            }
            cell.State = Explored;
            if(cell.IsMine)
            {
                OnExploded(x, y);
                return;
            }
            else if (cell.ProximalMineCount == 0)
            {
                foreach (var (x1, y1) in GetSurroundingCellsOf(x, y))
                {
                    Explore(x1, y1);
                } 
            }
        }

        public void MarkAsMine(int x, int y)
        {
            var cell = CellData[(x, y)];
            if(cell.State == Explored)
            {
                var ex = new InvalidOperationException("Cell is already explored");
                ex.Data.Add("x", x);
                ex.Data.Add("y", y);
                ex.Data.Add("cell", cell);
                throw ex;
            }
            cell.State = MarkedAsMine;
            --UnmarkedMines;
        }

        protected IEnumerable<(int, int)> GetSurroundingCellsOf(int x, int y, Func<int, int, bool> IndexCondition)
        {

            for(int i = -1; i <=1; ++i)
            {
                var x1 = x + i;
                for(int j = -1; j <= 1; ++j)
                {
                    var y1 = y + j;
                    if(IndexCondition(x1, y1))
                    {
                        yield return (x1, y1);
                    }
                }
            }

        }

        protected IEnumerable<(int, int)> GetSurroundingCellsOf(int x, int y) =>
            GetSurroundingCellsOf(x, y, CheckIndex);

        protected bool CheckIndex(int i, int j) =>
            i >= 0 && i < Width &&
            j >= 0 && j < Height;

        protected virtual void OnExploded(int x, int y)
        {
            Exploded?.Invoke(this, new ExplosionEventArgs(x, y));
            GameState = GameState.Ended;
        }
    }
}
