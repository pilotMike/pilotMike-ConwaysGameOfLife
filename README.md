# pilotMike-ConwaysGameOfLife
Playing around with different implementations of Conways Game of Life

# Benchmarks
#### 1,000 iterations, relase mode, optimize checked, allow unsafe (two methods for stack allocating an arry to check for neighbors)
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.900 (1909/November2018Update/19H2)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.300
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


|                                    Method | Dimensions |        Mean |     Error |     StdDev | Ratio | RatioSD |      Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|------------------------------------------ |----------- |------------:|----------:|-----------:|------:|--------:|-----------:|----------:|---------:|----------:|
|                          HashSetBenchmark |         50 |   443.41 ms |  0.497 ms |   0.441 ms |  1.00 |    0.00 | 11000.0000 |         - |        - |  90.52 MB |
|                                  Parallel |         50 |    59.31 ms |  1.868 ms |   5.479 ms |  0.13 |    0.01 | 15555.5556 | 3222.2222 | 111.1111 |  117.7 MB |
|            ParallelAggressiveOptimization |         50 |    61.58 ms |  1.965 ms |   5.762 ms |  0.14 |    0.02 | 16250.0000 | 3250.0000 |        - | 123.74 MB |
|                          ParallelBuffered |         50 |   256.37 ms | 12.092 ms |  35.463 ms |  0.53 |    0.04 | 24000.0000 | 4000.0000 | 333.3333 | 171.95 MB |
| AggressiveParallelRemoveStackAllocForeach |         50 |    58.46 ms |  1.440 ms |   4.109 ms |  0.13 |    0.01 | 15888.8889 | 3333.3333 |        - | 120.77 MB |
|                                           |            |             |           |            |       |         |            |           |          |           |
|                          HashSetBenchmark |        100 | 1,421.78 ms |  1.971 ms |   1.646 ms |  1.00 |    0.00 | 36000.0000 |         - |        - | 290.22 MB |
|                                  Parallel |        100 |   154.50 ms |  3.960 ms |  11.553 ms |  0.11 |    0.01 | 19250.0000 | 1000.0000 | 500.0000 | 148.45 MB |
|            ParallelAggressiveOptimization |        100 |   138.26 ms |  4.449 ms |  13.117 ms |  0.10 |    0.01 | 21000.0000 | 1000.0000 | 500.0000 | 160.62 MB |
|                          ParallelBuffered |        100 |   762.67 ms | 44.779 ms | 132.032 ms |  0.41 |    0.04 | 47000.0000 | 1000.0000 |        - | 340.94 MB |
| AggressiveParallelRemoveStackAllocForeach |        100 |   140.55 ms |  4.215 ms |  12.428 ms |  0.10 |    0.01 | 20750.0000 | 1000.0000 | 500.0000 | 157.57 MB |


The parallel version uses some inlined code via generics with constraints for struct/interface that the HashSet version does not, though that shouldn't make much difference.

The original hash set benchmark tries to avoid allocations and reuses the same collections to hold the cells moreso than the parallel version, hence no Gen1 or Gen2 collections. The parallel version could probably be improved with some weirdness, such as passing in the concurrent hash set to be used for getting the ActiveCells rather than creating a new one inside of the method.

Parallel After Buffering: didn't actually help. The Clear() method doesn't reset, it re-allocates. It does, however, cut the Gen2 collections. https://github.com/dotnet/runtime/blob/9f4d39a74a57f8e7a18d0d9068e3ccbf728e8715/src/libraries/System.Collections.Concurrent/src/System/Collections/Concurrent/ConcurrentDictionary.cs#L639.

For AggressiveParallelRemoveStackAllocForeach, I changed the code to get the neighbors from making a `stackalloc Coordinate[]` (Coordinate is a struct) to individually adding each coordinate to the Concurrent dictionary. The benefit is that it did cut down on Gen0 garbage collection, though that is often considered free. Downside is the `foreach` and `stackalloc` seems to maybe have some slight optimization on it, making it a few ms faster on larger grid sizes.