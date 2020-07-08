# pilotMike-ConwaysGameOfLife
Playing around with different implementations of Conways Game of Life

# Benchmarks
#### 1,000 iterations, relase mode, optimize checked, allow unsafe (two methods for stack allocating an arry to check for neighbors)
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.900 (1909/November2018Update/19H2)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.300
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


|           Method | Dimensions |        Mean |    Error |    StdDev | Ratio |      Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|----------------- |----------- |------------:|---------:|----------:|------:|-----------:|----------:|---------:|----------:|
| HashSetBenchmark |         50 |   380.52 ms | 0.456 ms |  0.381 ms |  1.00 |  7000.0000 |         - |        - |  62.29 MB |
|         Parallel |         50 |    57.59 ms | 1.515 ms |  4.466 ms |  0.15 | 15800.0000 | 3100.0000 |        - | 119.35 MB |
|         Parallel After Buffering |         50 |   110.2 ms |  5.35 ms | 15.68 ms |  0.33 |    0.03 | 16800.0000 |  800.0000 |        - | 127.17 MB |
| Parallel Aggressive Optimization (separate run, new random input)  |         50 |    57.60 ms | 1.582 ms |  4.638 ms |  0.16 |    0.02 | 15700.0000 |  100.0000 |        - | 119.07 MB |
|                  |            |             |          |           |       |            |           |          |           |
| HashSetBenchmark |        100 | 1,318.67 ms | 1.331 ms |  1.180 ms |  1.00 | 27000.0000 |         - |        - | 224.02 MB |
|         Parallel |        100 |   141.02 ms | 4.640 ms | 13.681 ms |  0.10 | 19750.0000 | 1000.0000 | 500.0000 | 150.54 MB |
|         Parallel After Buffering |        100 |   313.3 ms | 13.23 ms | 39.00 ms |  0.23 |    0.03 | 27000.0000 | 1333.3333 | 666.6667 | 198.08 MB |
|         Parallel Aggressive Optimization (separate run, new random input) |        100 |   139.12 ms | 4.414 ms | 12.877 ms |  0.11 |    0.01 | 23250.0000 | 1000.0000 | 500.0000 | 177.01 MB |


The parallel version uses some inlined code via generics with constraints for struct/interface that the HashSet version does not, though that shouldn't make much difference.

The original hash set benchmark tries to avoid allocations and reuses the same collections to hold the cells moreso than the parallel version, hence no Gen1 or Gen2 collections. The parallel version could probably be improved with some weirdness, such as passing in the concurrent hash set to be used for getting the ActiveCells rather than creating a new one inside of the method.

Parallel After Buffering: didn't actually help. The Clear() method doesn't reset, it re-allocates. https://github.com/dotnet/runtime/blob/9f4d39a74a57f8e7a18d0d9068e3ccbf728e8715/src/libraries/System.Collections.Concurrent/src/System/Collections/Concurrent/ConcurrentDictionary.cs#L639.
