using System.Collections.Generic;

namespace ConwaysGameOfLife.Grids
{
    public sealed class HashGrid : IConwayGrid
    {
        private readonly HashSet<Coordinate> _hashSet;
        private readonly int _maxDimensions;

        public HashSet<Coordinate> Buffer { get; } = new HashSet<Coordinate>();

        public HashGrid(HashSet<Coordinate> hashSet, int maxDimensions) =>
            (_hashSet, _maxDimensions) = (hashSet, maxDimensions);

        public bool HasLiveCell(Coordinate c) => _hashSet.Contains(c);

        public TDictionary ActiveCells<TDictionary>(TDictionary buffer)
            where TDictionary : IDictionary<Coordinate, bool>
        {
            buffer.Clear();
            foreach (var c in _hashSet) // hash set's UnionWith would off no perf improvements, since it does the same thing
            {
                buffer[c] = true;
                Grid.AddNeighbors(buffer, _hashSet, c);
            }

            return buffer;
        }

        // no way to get rid of null checks

        public void Add(Coordinate c) => _hashSet.Add(c);

        public bool HasLiveCells() => _hashSet.Count > 0;

        public void Set<TEnumerable>(TEnumerable state) where TEnumerable : IEnumerable<(Coordinate Key, bool Value)>
        {
            foreach (var (Key, Value) in state)
                if (Value &&
                    Key.X <= _maxDimensions &&
                    Key.Y <= _maxDimensions)
                {
                    _hashSet.Add(Key);
                }
                else _hashSet.Remove(Key);
        }

        public void SetDictionary<TDictionary>(TDictionary state) where TDictionary : IDictionary<Coordinate, bool>
        {
            foreach (var (Key, Value) in state)
                if (Value &&
                    Key.X <= _maxDimensions &&
                    Key.Y <= _maxDimensions)
                {
                    _hashSet.Add(Key);
                }
                else _hashSet.Remove(Key);
        }
    }
}
