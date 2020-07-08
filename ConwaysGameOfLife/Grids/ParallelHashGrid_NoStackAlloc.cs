using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.Grids
{
    public class ParallelHashGrid_NoStackAlloc : IParallelConwayGrid
    {
        // assume this isn't written to in parallel.hahahahaha
        private readonly HashSet<Coordinate> _grid;

        public ParallelHashGrid_NoStackAlloc(IEnumerable<Coordinate> state)
        {
            _grid = new HashSet<Coordinate>(state);
        }

        public TDictionary ActiveCells<TDictionary>(TDictionary buffer) where TDictionary : IDictionary<Coordinate, bool>
        {
            throw new NotImplementedException();
        }

        public bool HasLiveCell(Coordinate c) => _grid.Contains(c);

        public bool HasLiveCells() => _grid.Count > 0;

        public void Set((Coordinate c, bool alive) cell)
        {
            if (cell.alive)
                _grid.Add(cell.c);
            else _grid.Remove(cell.c);
        }

        public void Set<TEnumerable>(TEnumerable state) where TEnumerable : IEnumerable<(Coordinate c, bool alive)>
        {
            _grid.Clear();
            foreach (var (c, alive) in state)
                if (alive)
                    _grid.Add(c);
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
        public ConcurrentDictionary<Coordinate, bool> ActiveCellsParallel()
        {
            var distinctCells = new ConcurrentDictionary<Coordinate, bool>();
            Parallel.ForEach(_grid, c =>
            {
                distinctCells.TryAdd(c, true);

                // copied and modified from Grid.AddNeighbors
                // to use the TryAdd method on the concurrent dictionary

                distinctCells.TryAdd((c.X - 1, c.Y - 1), false);
                distinctCells.TryAdd((c.X, c.Y - 1), false);
                distinctCells.TryAdd((c.X + 1, c.Y - 1), false);
                distinctCells.TryAdd((c.X - 1, c.Y), false);
                distinctCells.TryAdd((c.X + 1, c.Y), false);
                distinctCells.TryAdd((c.X - 1, c.Y + 1), false);
                distinctCells.TryAdd((c.X, c.Y + 1), false);
                distinctCells.TryAdd((c.X + 1, c.Y + 1), false);

            });
            return distinctCells;
        }
    }
}
