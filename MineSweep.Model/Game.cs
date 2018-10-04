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

        public event EventHandler GameWon;

        protected CellCollection CellData { get; set; }

        protected int Version { get; set; }

        public GameState GameState { get; protected set; }

        public Difficulty Difficulty { get; protected set; }

        public int Width => Difficulty.Width;

        public int Height => Difficulty.Height;

        public IEnumerable<Cell> Cells => CellData;

        public int MineCount => Difficulty.MineCount;

        public int UnmarkedMines { get; protected set; }

        protected int UnmakredCells { get; set; }

        public Game()
        {
            Version = default;
            Difficulty = Difficulty.Beginner;
            PropertyChanged += WinConditionCheck;
            Reset();
        }

        public void Reset()
        {
            GameState = GameState.PreStart;
            CellData = GenerateEmptyCollection();
            UnmakredCells = CellData.Count;
            UnmarkedMines = Difficulty.MineCount;
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
            UnmakredCells = cells.Count;
            UnmarkedMines = Difficulty.MineCount;
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
            Reset();
        }

        public void Explore(int x, int y)
        {
            var cell = CellData[(x, y)];
            if(cell.State == Explored)
            {
                return;
            }
            else if(cell.IsMine)
            {
                cell.State = MineTriggered;
                --UnmakredCells;
                OnExploded(x, y);
                return;
            }
            else
            {
                cell.State = Explored;
                --UnmakredCells;
                if (cell.ProximalMineCount == 0)
                {
                    foreach (var (x1, y1) in GetSurroundingCellsOf(x, y))
                    {
                        Explore(x1, y1);
                    }
                }
            }
        }

        public void Mark(int x, int y)
        {
            var cell = CellData[(x, y)];
            switch(cell.State)
            {
                case Unexplored:
                    cell.State = MarkedAsMine;
                    --UnmarkedMines;
                    --UnmakredCells;
                    break;
                case MarkedAsMine:
                    cell.State = MarkedAsInterest;
                    ++UnmarkedMines;
                    ++UnmakredCells;
                    break;
                case MarkedAsInterest:
                    cell.State = Unexplored;
                    break;
                default:
                    var ex = new InvalidOperationException("Cell cannot be marked");
                    ex.Data.Add("x", x);
                    ex.Data.Add("y", y);
                    ex.Data.Add("cell", cell);
                    throw ex;
            }
            ++Version;
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

        protected virtual void WinConditionCheck(object o, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(UnmakredCells) || e.PropertyName == nameof(UnmarkedMines))
            {
                if(UnmarkedMines == 0 && UnmakredCells == 0)
                {
                    GameState = GameState.Won;
                    GameWon?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        protected virtual void OnExploded(int x, int y)
        {
            Exploded?.Invoke(this, new ExplosionEventArgs(x, y));
            GameState = GameState.Over;
        }
    }
}
