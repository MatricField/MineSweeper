using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweep.Model
{
    public struct Difficulty
    {
        public const double MAX_MINE_RATIO = 0.5;

        public int Width { get; }

        public int Height { get; }

        public int MineCount { get; }

        public static int CalculateMaxNumberOfMine(int width, int height) =>
            Convert.ToInt32(Math.Floor(width * height * MAX_MINE_RATIO));

        public Difficulty(int width, int height, int mineCount)
        {
            if (mineCount < 0 || mineCount > CalculateMaxNumberOfMine(width, height))
            {
                throw new ArgumentException("Too many mines");
            }
            Width = width;
            Height = height;
            MineCount = mineCount;
        }

        public static readonly Difficulty Beginner =
            new Difficulty(8, 8, 10);

        public static readonly Difficulty Intermediate =
            new Difficulty(16, 16, 40);

        public static readonly Difficulty Expert =
            new Difficulty(24, 24, 99);
    }
}
