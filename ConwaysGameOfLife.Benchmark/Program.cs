using BenchmarkDotNet.Running;
using System;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<BenchmarkTest>();

            //Console.WriteLine("starting");
            //var b = new BenchmarkTest() { Dimensions = 100 };
            //b.Setup();
            //return b.HashSetBenchmarkAsync();
        }
    }
}
