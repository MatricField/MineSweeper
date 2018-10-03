using System;
using System.Collections.Generic;
using System.Text;

namespace MineSweep.Model
{
    public enum CellState
    {
        Unexplored,
        Explored,
        MarkedAsMine,
        MarkedAsInterest,
        MineTriggered
    }
}
