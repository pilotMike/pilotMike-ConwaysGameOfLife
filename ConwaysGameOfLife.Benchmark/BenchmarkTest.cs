using BenchmarkDotNet.Attributes;
using ConwaysGameOfLife.Grids;
using Microsoft.Diagnostics.Tracing.Parsers.MicrosoftWindowsTCPIP;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.Benchmark
{
    [MemoryDiagnoser]
    public class BenchmarkTest
    {
        [Params(10, 100)]
        public int Dimensions { get; set; }
        public int Iterations { get; set; } = 1000;

        private List<Coordinate> _state;

        [GlobalSetup]
        public void Setup()
        {
            _state = new List<Coordinate>(Dimensions * Dimensions / 3);
            var r = new Random();
            for (int i = 0; i < Dimensions; i++)
            for (int j = 0; j < Dimensions; j++)
            {
                var add = r.Next(0, 4) == 1;
                if (add) _state.Add((i, j));
            }
        }

        [Benchmark(Baseline = true)]
        public void HashSetBenchmark()
        {
            var hs = new HashGrid(new HashSet<Coordinate>(_state), Dimensions);
            var buffer = new Dictionary<Coordinate, bool>(Dimensions);
            GameOfLife.Run(hs, buffer, Iterations);
        }

        // slooooow
        //[Benchmark]
        //public void HashSetParallelBenchmark()
        //{
        //    var hg = new HashGrid(new HashSet<Coordinate>(_state), Dimensions);
        //    GameOfLife.RunParallel(hg, Iterations);
        //}

    }
}
