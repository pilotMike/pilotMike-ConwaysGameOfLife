using ConwaysGameOfLife.Grids;
using ConwaysGameOfLife.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ConwaysGameOfLife.GoL
{
    public sealed class ForLoopGoL : IGameOfLife
    {
        public void Run<TView, TState>(
            TState state,
            int dimensions,
            GameOfLifeOptions options = null)
            where TView : struct, IView
            where TState : IEnumerable<Coordinate>
        {
            // setup
            var maxIters = options?.MaxIterations;
            var token = options?.CancellationToken ?? CancellationToken.None;
            TView view = default;

            var memory = new bool[dimensions, dimensions];
            foreach (var liveCell in state)
                memory[liveCell.Y, liveCell.X] = true;

            // run
            // wanna see some weird shit to allow the compiler/jitter to inline?
            // let's hope it actually works lol
            if (maxIters.HasValue)
                if (options?.CancellationToken != null)
                    Loop<IterCheck, CancellationTokenCheck, TView>(memory, view, maxIters.Value, token);
                else Loop<IterCheck, AlwaysTrueCancellationTokenCheck, TView>(memory, view, maxIters.Value, token);
            else
            {
                if (options?.CancellationToken != null)
                    Loop<AlwaysRun, CancellationTokenCheck, TView>(memory, view, cancellationToken: token);
                else Loop<AlwaysRun, AlwaysTrueCancellationTokenCheck, TView>(memory, view);
            }
        }

        private void Loop<TIterCheck, TCancellationTokenCheck, TView>(
            bool[,] memory,
            TView view,
            int maxIters = 0, CancellationToken cancellationToken = default)
            where TIterCheck : struct, IIterCheck
            where TCancellationTokenCheck : struct, ICancellationTokenCheck
            where TView : IView
        {
            TIterCheck iterCheck = default;
            TCancellationTokenCheck cancellationTokenCheck = default;

            while (iterCheck.Run(maxIters) &&
                cancellationTokenCheck.Check(cancellationToken))
            {
                for (int y = 0; y < memory.GetUpperBound(0); y++)
                {
                    for (int x = 0; x < memory.GetUpperBound(1); x++)
                    {
                        var c = (x, y);
                        var isAlive = CellState.IsAlive(memory, c);
                        memory[y, x] = isAlive;
                        view.Set((c, isAlive));
                    }
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
