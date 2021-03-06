﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.Grids
{
    public class BufferedParallelHashGrid : IConwayGrid
    {
        // value is not actually needed
        private readonly ConcurrentDictionary<Coordinate, bool> _grid;

        public BufferedParallelHashGrid(IEnumerable<Coordinate> state)
        {
            _grid = new ConcurrentDictionary<Coordinate, bool>(state.Select(s => new KeyValuePair<Coordinate, bool>(s, true)));
        }

        public TDictionary ActiveCells<TDictionary>(TDictionary buffer) where TDictionary : IDictionary<Coordinate, bool>
        {
            throw new NotImplementedException();
        }

        public bool HasLiveCell(Coordinate c) => _grid.ContainsKey(c);

        public bool HasLiveCells() => _grid.Count > 0;

        public void Set((Coordinate c, bool alive) cell)
        {
            if (cell.alive)
                _grid.GetOrAdd(cell.c, true);
            else _grid.Remove(cell.c, out var _);
        }

        public void Set<TEnumerable>(TEnumerable state) where TEnumerable : IEnumerable<(Coordinate c, bool alive)>
        {
            _grid.Clear();
            foreach (var (c, alive) in state)
                if (alive)
                    _grid.GetOrAdd(c, true);
        }

        public void SetDictionary<TDictionary>(TDictionary state) where TDictionary : IDictionary<Coordinate, bool>
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a concurrent dictionary populated in parallel.
        /// allocates a concurrent dictionary and a closure on each invocation
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        public void ActiveCellsParallel(ConcurrentDictionary<Coordinate, bool> buffer)
        {
            buffer.Clear();
            Parallel.ForEach(_grid, cell =>
            {
                var c = cell.Key;
                buffer.TryAdd(c, true);

                // copied and modified from Grid.AddNeighbors
                // to use the TryAdd method on the concurrent dictionary
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
                    buffer.TryAdd(n, false);
            });
        }
    }
}
