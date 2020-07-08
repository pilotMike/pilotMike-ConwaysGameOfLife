using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ConwaysGameOfLife.Grids
{
    public interface IParallelConwayGrid : IConwayGrid
    {
        ConcurrentDictionary<Coordinate, bool> ActiveCellsParallel();
        void Set((Coordinate c, bool alive) cell);
    }
}
