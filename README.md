# pilotMike-ConwaysGameOfLife
Playing around with different implementations of Conways Game of Life

# Benchmarks
#### 1,000 iterations, release mode, optimize checked, allow unsafe (two methods for stack allocating an arry to check for neighbors)
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.900 (1909/November2018Update/19H2)
AMD Ryzen 5 3600, 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=3.1.300
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


|                                    Method | Dimensions |        Mean |     Error |     StdDev | Ratio | RatioSD |      Gen 0 |     Gen 1 |    Gen 2 | Allocated |
|------------------------------------------ |----------- |------------:|----------:|-----------:|------:|--------:|-----------:|----------:|---------:|----------:|
|                          HashSetBenchmark |         50 |   443.41 ms |  0.497 ms |   0.441 ms |  1.00 |    0.00 | 11000.0000 |         - |        - |  90.52 MB |
|                                  Parallel |         50 |    59.31 ms |  1.868 ms |   5.479 ms |  0.13 |    0.01 | 15555.5556 | 3222.2222 | 111.1111 |  117.7 MB |
|  ParallelAggressiveOptimization |         50 |    61.58 ms |  1.965 ms |   5.762 ms |  0.14 |    0.02 | 16250.0000 | 3250.0000 |        - | 123.74 MB |
|   ParallelBuffered |         50 |   256.37 ms | 12.092 ms |  35.463 ms |  0.53 |    0.04 | 24000.0000 | 4000.0000 | 333.3333 | 171.95 MB |
| AggressiveParallelRemoveStackAllocForeach |         50 |    58.46 ms |  1.440 ms |   4.109 ms |  0.13 |    0.01 | 15888.8889 | 3333.3333 |        - | 120.77 MB |
|                                           |            |             |           |            |       |         |            |           |          |           |
|   HashSetBenchmark |        100 | 1,421.78 ms |  1.971 ms |   1.646 ms |  1.00 |    0.00 | 36000.0000 |         - |        - | 290.22 MB |
|                                  Parallel |        100 |   154.50 ms |  3.960 ms |  11.553 ms |  0.11 |    0.01 | 19250.0000 | 1000.0000 | 500.0000 | 148.45 MB |
|            ParallelAggressiveOptimization |        100 |   138.26 ms |  4.449 ms |  13.117 ms |  0.10 |    0.01 | 21000.0000 | 1000.0000 | 500.0000 | 160.62 MB |
|   ParallelBuffered |        100 |   762.67 ms | 44.779 ms | 132.032 ms |  0.41 |    0.04 | 47000.0000 | 1000.0000 |        - | 340.94 MB |
| AggressiveParallelRemoveStackAllocForeach |        100 |   140.55 ms |  4.215 ms |  12.428 ms |  0.10 |    0.01 | 20750.0000 | 1000.0000 | 500.0000 | 157.57 MB |
|         ForLoop |        100 |   755.6 ms |  1.52 ms |  1.42 ms |no  baseline used | |         - |     - |     - |    9.92 KB |0000 | 500.0000 | 157.57 MB |
| ParallelForLoop |        100 |   126.1 ms |  2.48 ms |  2.20 ms | no  baseline used | | 400.0000 |     - |     - | 3893.21 KB |
|                                           |            |             |           |            |       |         |
|            ParallelAggressiveOptimization |        300 |   769.0 ms | 18.03 ms | 52.59 ms | no  baseline used | | 69000.0000 | 10000.0000 | 3000.0000 | 537848.74 KB |
| AggressiveParallelRemoveStackAllocForeach |        300 |   761.7 ms | 16.04 ms | 46.78 ms | no  baseline used | | 71000.0000 |  8000.0000 | 4000.0000 | 558889.48 KB |
|  ForLoop |        300 | 6,771.0 ms |  6.44 ms |  6.02 ms | no  baseline used | |          - |          - |         - |     89.81 KB |
|   ParallelForLoop |        300 |   953.2 ms |  4.25 ms |  3.98 ms |  no baseline used | |        - |          - |         - |   4022.12 KB |

The parallel version uses some inlined code via generics with constraints for struct/interface that the HashSet version does not, though that shouldn't make much difference.

The original hash set benchmark tries to avoid allocations and reuses the same collections to hold the cells moreso than the parallel version, hence no Gen1 or Gen2 collections. The parallel version could probably be improved with some weirdness, such as passing in the concurrent hash set to be used for getting the ActiveCells rather than creating a new one inside of the method.

Parallel After Buffering: didn't actually help. The Clear() method doesn't reset, it re-allocates. It does, however, cut the Gen2 collections. https://github.com/dotnet/runtime/blob/9f4d39a74a57f8e7a18d0d9068e3ccbf728e8715/src/libraries/System.Collections.Concurrent/src/System/Collections/Concurrent/ConcurrentDictionary.cs#L639.

For AggressiveParallelRemoveStackAllocForeach, I changed the code to get the neighbors from making a `stackalloc Coordinate[]` (Coordinate is a struct) to individually adding each coordinate to the Concurrent dictionary. The benefit is that it did cut down on Gen0 garbage collection, though that is often considered free. Downside is the `foreach` and `stackalloc` seems to maybe have some slight optimization on it, making it a few ms faster on larger grid sizes.

### How I should have started
For the final run, I went with what I should have started with: the simplest solution, a for-loop. I had assumed that I should start with a hash set to only check on the cells that can actually change (alive cells are tracked and their neighbors computed). I also tried taking those total coordinates and `distinct`ing them, so they aren't checked more than once. 

I didn't include the hash set versions for the final 300x300 run because I wasn't patient enough to wait for it to finish. It was looking to be about 12 seconds per run. However, my assumption on using a hash set as the backing data store wasn't entirely incorrect. As the dimension size increases, the for-loop lags behind, even though it is king at lower dimension sizes (except for the serial hash set. That one is always last.). This makes sense.  I should probably also test how each approach does with a given density of alive cells in their starting states.

For now, the parallel hash set approach is fastest, which `distinct`s by adding neighbors into a `ConcurrentDictionary` that is re-allocated on each cycle, which was faster than re-using and clearing it. I was surpised by how much slower the serial hash set implementation was, regardless of grid size.

I have not tested a parallel for-loop (or serial) with distincting coordinates/cells because 1) didn't feel like it and 2) the for-loop is the purest method. It only contains an allocation for the memory array. The parallel version uses another array as the output and then swaps the two betweent generations; it also allocates during the `Parallel.For` call on each generation. If I were to `distinct` the alive cells, it would become more like the hash-set or parallel hash set implementation.

Oh, one other thing to be careful of is any assumptions on memory usage. The lowest usage by far is the for-loops, but other versions finish faster. Some have lots of Gen0 allocations, which is theoretically fine. The for-loop versions have none, but lose out on speed.
Which version is the best depends. How big is the grid? How full is it? Are you running multiple users concurrently, such as on a web server. I imagine that in a single user environment, pure speed might be best, but in a web server, sacrificing some speed for lower GC pressue may be better. But I haven't benchmarked that and any thoughts are pretty much meaningless until you do. Such is the case with my assumption of starting with a hash set instead of a 2D array. 

Another note: the 400 for Gen0 on the parallel for loop for a 100x100 grid seems flukish, since there was none on the larger grid.


If I had to recommend one to use assuming that grid sizes could get insane, I'd probably go for the Aggressive (optimization) parallel approach. It tracks active cells and distincts the neighbors in parallel before processing. It produces GC churn, but will likely perform the best of the methods for large grid sizes with arbitrary occupancy rates.

## Techniques used for performance
1) Install the Heap Allocation Viewer extension. It will give hints on how to avoid allocations
2) One of the main suggestions used is to, instead of using a method signature like
  ``` csharp
  public void DoSomething(IEnumerable<T> collection)
  {
    foreach (var item in collection) // heap allocation viewer gives a green squigly here
  } 
  ```
  Use
  ``` csharp
  public void DoSomething<T>(T collection) where T : IEnumerable<x>
  ```
  What this does is allow the compiler/jitter to make a new version of the method for the specific concrete type passed in, preventing the boxing of struct enumerators into an IEnumerable heap allocated reference. If you pass in a `List<T>` or `T[]`, the compiler and jitter can use the concrete types, rather than the interface, thereby avoiding the allocation of the struct enumerator.

3) How to get inlining.
I was able to verify that this this was working by checking the disassembly during runtime for the `while` checks (max iterations and cancellation token check).
  a) First, make your parent class a `sealed class`.
  b) Create structs with your logic and implement a common interface.
  c) Use the generic constraint in place of passing in an interface, like above.

  The method came out looking a bit strange:
  ``` csharp

  var maxIters = options?.MaxIterations; // max iters = 1000
  var token = options?.CancellationToken ?? CancellationToken.None; // not set for the benchmarks

  if (maxIters.HasValue)
    if (options?.CancellationToken != null)
        RunImpl<IterCheck, CancellationTokenCheck, TView>(memory, view, maxIters.Value, token);
    else RunImpl<IterCheck, AlwaysTrueCancellationTokenCheck, TView>(memory, view, maxIters.Value, token);// the options brought us here
  else
  {
      if (options?.CancellationToken != null)
          RunImpl<AlwaysRun, CancellationTokenCheck, TView>(memory, view, cancellationToken: token);
      else RunImpl<AlwaysRun, AlwaysTrueCancellationTokenCheck, TView>(memory, view); 
  }
```
``` csharp
private void RunImpl<TIterCheck, TCancellationTokenCheck, TView>(
    bool[,] memory,
    TView view,
    int maxIters = 0, CancellationToken cancellationToken = default)
    where TIterCheck : struct, IIterCheck
    where TCancellationTokenCheck : struct, ICancellationTokenCheck
    where TView : IView
{
    TIterCheck iterCheck = default; // don't pass this in. Just use the generic constraint so the compiler can try inlining or removing code.
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

private interface IIterCheck { bool Run(int max); }
private struct IterCheck : IIterCheck
{
    private int current;

    public bool Run(int max) => current++ < max;
}
private readonly struct AlwaysTrueCancellationTokenCheck : ICancellationTokenCheck
{
    public bool Check(CancellationToken token) => true;
}
```

  What this does is it lets the compiler know exactly what struct we are using. Structs have special rules and can be inlined. Because we are in a sealed class, we pass in the type of `ICancellationTokenCheck` to use and initialize it inside the method (with `TCancellationTokenCheck cancellationTokenCheck = default`) and generic constrains, the `AlwaysTrueCancellationTokenCheck` logic was removed from the assembly and never executed because the jitter/compiler can see that it always returns true. I'm not terribly good at reading assembly, but I did see a chunk of instructions disappear from the disassembly on implementing this.

4) Other suggestions for performance.
Even though I did do it in a few places, don't use `foreach` on non-array types in hot paths. There is special behavior for removing bounds checks on arrays, but not lists an other types of collections. Use a `for` loop if possible.

If you need a temporary array, consider passing it in as a buffer or using `ArrayPool<T>.Rent`.
