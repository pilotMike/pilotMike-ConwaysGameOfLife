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
|                  |            |             |          |           |       |            |           |          |           |
| HashSetBenchmark |        100 | 1,318.67 ms | 1.331 ms |  1.180 ms |  1.00 | 27000.0000 |         - |        - | 224.02 MB |
|         Parallel |        100 |   141.02 ms | 4.640 ms | 13.681 ms |  0.10 | 19750.0000 | 1000.0000 | 500.0000 | 150.54 MB |


I suspect the higher memory usage on the hash set version is because it is not 'distincting' the neighbor cells, whereas the parallel version puts them into a concurrent dictionary before returning the active cells (active meaning any cell that is or is near a live cell and can therefore change).

Also, the parallel version uses some inlined code via generics with constraints for struct/interface, though that shouldn't make any difference.

The original hash set benchmark tries to avoid allocations and reuses the same collections to hold the cells moreso than the parallel version, hence no Gen1 or Gen2 collections. The parallel version could probably be improved with some weirdness, such as passing in the concurrent hash set to be used for getting the ActiveCells rather than creating a new one inside of the method.