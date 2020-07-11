using ConwaysGameOfLife.Grids;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ConwaysGameOfLife.GoL
{
    public static class CellState<TGrid> where TGrid : IConwayGrid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static (Coordinate cell, bool cellState) Get(TGrid grid, KeyValuePair<Coordinate, bool> kvp)
            
        {
            var (cell, alive) = kvp;
            var neighbors = Grid.NeighborCount(cell, grid);

            var cellState = neighbors switch
            {
                2 when alive => true,
                3 => true,
                _ => false
            };

            return (cell, cellState);
        }
    }

    public static class CellState
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool IsAlive(bool[,] memory, Coordinate c)
        {
            var neighbors = Grid.NeighborCount(c, memory);
            var cellState = neighbors switch
            {
                2 when memory[c.Y, c.X] => true,
                3 => true,
                _ => false
            };

            return  cellState;
        }
    }
}
