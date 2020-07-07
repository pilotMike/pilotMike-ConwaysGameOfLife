using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;

namespace ConwaysGameOfLife.Grids
{
    public static class Grid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int NeighborCount<TGrid>(Coordinate c, TGrid grid) where TGrid : IConwayGrid
        {
            Span<Coordinate> neighbors = stackalloc Coordinate[8]
            {
                (c.X - 1, c.Y - 1),
                (c.X, c.Y - 1),
                (c.X + 1, c.Y - 1),
                (c.X - 1, c.Y),
                (c.X + 1, c.Y),
                (c.X - 1, c.Y + 1),
                (c.X, c.Y + 1),
                (c.X + 1, c.Y + 1)
            };

            int count = 0;
            foreach (var cell in neighbors)
            {
                bool hasCell = grid.HasLiveCell(cell);
                count += hasCell ? 1 : 0;
            }

            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void AddNeighbors<TDictionary>(TDictionary buffer,
            HashSet<Coordinate> original, Coordinate c)
            where TDictionary : IDictionary<Coordinate, bool>
        {
            Span<Coordinate> neighbors = stackalloc Coordinate[8]
            {
                (c.X - 1, c.Y - 1),
                (c.X, c.Y - 1),
                (c.X + 1, c.Y - 1),
                (c.X - 1, c.Y),
                (c.X + 1, c.Y),
                (c.X - 1, c.Y + 1),
                (c.X, c.Y + 1),
                (c.X + 1, c.Y + 1)
            };

            foreach (var n in neighbors)
            {
                if (!original.Contains(n))
                    buffer[n] = false;
            }
        }
    }
}
