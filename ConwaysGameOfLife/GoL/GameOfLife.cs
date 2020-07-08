using ConwaysGameOfLife.Grids;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.GoL
{
    public class GameOfLife : IGameOfLife
    {
        public void Step<TStepResult, TGrid>(TStepResult results, TGrid grid, Dictionary<Coordinate, bool> buffer)
            where TStepResult : IStepResult
            where TGrid : IConwayGrid
        {
            // todo: come back to see about avoiding an allocation
            foreach (var kvp in grid.ActiveCells(buffer))
            {
                SetNeighbors(results, grid, kvp);
            }
        }

        private void SetNeighbors<TStepResult, TGrid>(TStepResult results, TGrid grid, KeyValuePair<Coordinate, bool> kvp)
            where TStepResult : IStepResult
            where TGrid : IConwayGrid
        {

            var (cell, cellState) = CellState<TGrid>.Get(grid, kvp);

            results.Set(cell, cellState);
        }

        

        public void Run<TGrid>(TGrid grid, Dictionary<Coordinate, bool> buffer,
            int? iterations = null)
            where TGrid : IConwayGrid
        {
            var result = new StepResult();
            var executedIterations = 0;
            while (grid.HasLiveCells())
            {
                Step(result, grid, buffer);
                grid.SetDictionary(result.State);

                result.Clear();

                executedIterations++;
                if (iterations != null && executedIterations >= iterations)
                    break;
            }
        }

        public async Task RunAsync<TGrid>(int delayMillis, TGrid grid, Dictionary<Coordinate, bool> buffer,
            int? iterations = null)
            where TGrid : IConwayGrid
        {
            var result = new StepResult();
            var executedIterations = 0;
            while (grid.HasLiveCells())
            {
                Step(result, grid, buffer);
                grid.SetDictionary(result.State);

                result.Clear();

                if (delayMillis > 0)
                    await Task.Delay(delayMillis).ConfigureAwait(false);

                executedIterations++;
                if (iterations != null && executedIterations >= iterations)
                    break;
            }
        }

        public class StepResult : IStepResult
        {
            private readonly Dictionary<Coordinate, bool> _buffer;
            public IDictionary<Coordinate, bool> State => _buffer;

            public StepResult(int bufferSize = 4) => _buffer = new Dictionary<Coordinate, bool>(bufferSize);
            public void Set(Coordinate cell, bool cellState) => _buffer[cell] = cellState;

            public void Clear() => _buffer.Clear();
        }

        public class ConcurrentStepResult : IStepResult
        {
            private readonly ConcurrentDictionary<Coordinate, bool> _state = new ConcurrentDictionary<Coordinate, bool>();
            public IDictionary<Coordinate, bool> State => _state;

            public void Set(Coordinate cell, bool cellState) => _state[cell] = cellState;
            public void Clear() => _state.Clear();
        }
    }
}
