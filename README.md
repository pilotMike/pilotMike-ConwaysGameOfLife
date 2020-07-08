# pilotMike-ConwaysGameOfLife
Playing around with different implementations of Conways Game of Life

# Benchmarks
#### 1,000 iterations, relase mode, optimize checked, allow unsafe (two methods for stack allocating an arry to check for neighbors)
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.900 (1909/November2018Update/19H2)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.300
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


|                         Method | Dimensions |        Mean |     Error |     StdDev | Ratio | RatioSD |      Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|------------------------------- |----------- |------------:|----------:|-----------:|------:|--------:|-----------:|----------:|---------:|----------:|
|               HashSetBenchmark |         50 |   774.19 ms |  0.590 ms |   0.552 ms |  1.00 |    0.00 | 18000.0000 |         - |        - | 144.54 MB |
|                       Parallel |         50 |    56.72 ms |  1.704 ms |   4.971 ms |  0.07 |    0.01 | 15375.0000 | 3250.0000 |        - | 116.59 MB |
| ParallelAggressiveOptimization |         50 |    59.52 ms |  2.010 ms |   5.925 ms |  0.08 |    0.01 | 15500.0000 | 1750.0000 |        - |  117.6 MB |
|               ParallelBuffered |         50 |   288.47 ms | 18.837 ms |  55.540 ms |  0.30 |    0.04 | 24666.6667 | 4000.0000 | 333.3333 | 176.51 MB |
|                                |            |             |           |            |       |         |            |           |          |           |
|               HashSetBenchmark |        100 | 1,341.09 ms |  1.506 ms |   1.335 ms |  1.00 |    0.00 | 34000.0000 |         - |        - | 274.07 MB |
|                       Parallel |        100 |   142.61 ms |  3.622 ms |  10.622 ms |  0.10 |    0.01 | 21800.0000 | 1800.0000 | 800.0000 | 165.66 MB |
| ParallelAggressiveOptimization |        100 |   140.93 ms |  5.288 ms |  15.593 ms |  0.11 |    0.01 | 19000.0000 | 1333.3333 | 666.6667 | 147.11 MB |
|               ParallelBuffered |        100 |   731.46 ms | 53.146 ms | 156.702 ms |  0.43 |    0.05 | 43000.0000 | 1000.0000 |        - | 313.93 MB |


The parallel version uses some inlined code via generics with constraints for struct/interface that the HashSet version does not, though that shouldn't make much difference.

The original hash set benchmark tries to avoid allocations and reuses the same collections to hold the cells moreso than the parallel version, hence no Gen1 or Gen2 collections. The parallel version could probably be improved with some weirdness, such as passing in the concurrent hash set to be used for getting the ActiveCells rather than creating a new one inside of the method.

Parallel After Buffering: didn't actually help. The Clear() method doesn't reset, it re-allocates. It does, however, cut the Gen2 collections. https://github.com/dotnet/runtime/blob/9f4d39a74a57f8e7a18d0d9068e3ccbf728e8715/src/libraries/System.Collections.Concurrent/src/System/Collections/Concurrent/ConcurrentDictionary.cs#L639.
