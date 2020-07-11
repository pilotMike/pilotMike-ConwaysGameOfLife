using ConwaysGameOfLife.Views;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.GoL
{
    public sealed class ParallelForLoopGoL : IGameOfLife
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
                    RunImpl<IterCheck, CancellationTokenCheck, TView>(memory, view, maxIters.Value, token);
                else RunImpl<IterCheck, AlwaysTrueCancellationTokenCheck, TView>(memory, view, maxIters.Value, token);
            else
            {
                if (options?.CancellationToken != null)
                    RunImpl<AlwaysRun, CancellationTokenCheck, TView>(memory, view, cancellationToken: token);
                else RunImpl<AlwaysRun, AlwaysTrueCancellationTokenCheck, TView>(memory, view);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private void RunImpl<TIterCheck, TCancellationTokenCheck, TView>(
            bool[,] memory,
            TView view,
            int maxIters = 0, CancellationToken cancellationToken = default)
            where TIterCheck : struct, IIterCheck
            where TCancellationTokenCheck : struct, ICancellationTokenCheck
            where TView : IView
        {
            TIterCheck iterCheck = default;
            TCancellationTokenCheck cancellationTokenCheck = default;

            var original = memory;
            var output = new bool[memory.GetUpperBound(0), memory.GetUpperBound(1)];

            while (iterCheck.Run(maxIters) &&
                cancellationTokenCheck.Check(cancellationToken))
            {
                Loop(original, output, view);
                (original, output) = (output, original);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private void Loop<TView>(bool[,] original, bool[,] output, TView view) where TView : IView
        {
            Parallel.For(0, original.GetUpperBound(0), y =>
            {
                for (int x = 0; x < original.GetUpperBound(1); x++)
                {
                    var c = (x, y);
                    var isAlive = CellState.IsAlive(original, c);
                    output[y, x] = isAlive;
                    view.Set((c, isAlive));
                }
            });
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
