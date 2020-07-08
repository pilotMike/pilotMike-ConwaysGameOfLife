using ConwaysGameOfLife.GoL;
using ConwaysGameOfLife.Grids;
using ConwaysGameOfLife.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ConwaysGameOfLife
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //    var consoleGrid = new ConsoleGrid(40);
            //    var initialState = InitialStates.Glider;
            //    consoleGrid.Center(initialState);


            //    await GameOfLife.RunAsync(200, consoleGrid, new Dictionary<Coordinate, bool>());

            const int dims = 40;
            var state = GetInitialState(dims, 10);
            const int iters = 1_000;

            //var cg = new ConsoleGrid(dims, state);
            ////var hg = new HashGrid(new HashSet<Coordinate>(state), dims);

            //var gol = new GameOfLife();
            //await gol.RunAsync(200, cg, new Dictionary<Coordinate, bool>(), iters);

            var pgol = new ParallelGameOfLife();
            pgol.Run<ParallelHashGrid, ConsoleView>(new ParallelHashGrid(state),
                new GameOfLifeOptions { /*DelayMillis = 200,*/ MaxIterations = iters, Dimensions = dims });


            //Console.WriteLine("starting parallel");
            //RunTest(state, dims, iters);
            //Console.WriteLine("done");



            //RunEventBased();
        }

        public static void RunEventBased()
        {
            // there's no stopping this
            var state = InitialStates.Glider.Select(c => (Coordinate)(c.X + 10, c.Y + 10)).ToList(); //GetInitialState(70);
            var gol = new EventBasedGameOfLife();
            gol.Run(state, 70);
        }

        private static List<Coordinate> GetInitialState(int dimensions, int fraction)
        {
            var state = new List<Coordinate>(dimensions * dimensions / fraction);
            var r = new Random();
            for (int i = 0; i < dimensions; i++)
                for (int j = 0; j < dimensions; j++)
                {
                    var add = r.Next(0, 4) == 1;
                    if (add) state.Add((i, j));
                }

            return state;
        }
    }
}
