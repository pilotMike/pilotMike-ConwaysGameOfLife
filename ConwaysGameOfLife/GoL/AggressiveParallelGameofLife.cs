using ConwaysGameOfLife.Grids;
using ConwaysGameOfLife.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace ConwaysGameOfLife.GoL
{
    // copy pasta of the ParallelGameOfLife, but with the agressive optimization flag
    public class AggressiveParallelGameOfLife : IGameOfLife
    {
        public void Run<TConwayGrid, TView>(
            TConwayGrid grid, // todo: make this a regular interface. this way will do no good.
            GameOfLifeOptions options = null)
            where TConwayGrid : ParallelHashGrid
            where TView : struct, IView
        {
            var maxIters = options?.MaxIterations;
            var token = options?.CancellationToken ?? CancellationToken.None;
            TView view = default;
            if (options.Dimensions.HasValue)
                view.Dimensions = options.Dimensions.Value;

            // wanna see some weird shit to allow the compiler/jitter to inline?
            // let's hope it actually works lol
            if (maxIters.HasValue)
                if (options?.CancellationToken != null)
                    Loop<IterCheck, CancellationTokenCheck, TConwayGrid, TView>(grid, view, maxIters.Value, token);
                else Loop<IterCheck, AlwaysTrueCancellationTokenCheck, TConwayGrid, TView>(grid, view, maxIters.Value, token);
            else
            {
                if (options?.CancellationToken != null)
                    Loop<AlwaysRun, CancellationTokenCheck, TConwayGrid, TView>(grid, view, cancellationToken: token);
                else Loop<AlwaysRun, AlwaysTrueCancellationTokenCheck, TConwayGrid, TView>(grid, view);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private void Loop<TIterCheck, TCancellationTokenCheck, TConwayGrid, TView>(
            TConwayGrid grid,
            TView view,
            int maxIters = 0, CancellationToken cancellationToken = default)
            where TIterCheck : struct, IIterCheck
            where TCancellationTokenCheck : struct, ICancellationTokenCheck
            where TConwayGrid : ParallelHashGrid
            where TView : IView
        {
            TIterCheck iterCheck = default;
            TCancellationTokenCheck cancellationTokenCheck = default;

            while (iterCheck.Run(maxIters) &&
                cancellationTokenCheck.Check(cancellationToken))
            {
                var activeCells = grid.ActiveCellsParallel();
                var output = activeCells.AsParallel()
                    .Select(cell => CellState<TConwayGrid>.Get(grid, cell));
                foreach (var c in output)
                {
                    grid.Set(c);
                    view.Set(c);
                }
            }
        }



        private interface IIterCheck { bool Run(int max); }
        private struct IterCheck : IIterCheck
        {
            private int current;

            public bool Run(int max) => current++ < max;
        }
        private readonly struct AlwaysRun : IIterCheck
        {
            public bool Run(int max) => true;
        }

        private interface ICancellationTokenCheck
        {
            bool Check(CancellationToken token);
        }
        private readonly struct CancellationTokenCheck : ICancellationTokenCheck
        {
            public bool Check(CancellationToken token) => !token.IsCancellationRequested;
        }
        private readonly struct AlwaysTrueCancellationTokenCheck : ICancellationTokenCheck
        {
            public bool Check(CancellationToken token) => true;
        }
    }
}
