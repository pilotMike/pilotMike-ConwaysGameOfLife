using System;
using System.Collections.Generic;
using System.Text;

namespace ConwaysGameOfLife.Grids
{
    public sealed class ConsoleGrid : IConwayGrid
    {
        private readonly HashGrid _cache;
        private readonly int _maxDimensions;

        public HashSet<Coordinate> Buffer => _cache.Buffer;

        public TDictionary ActiveCells<TDictionary>(TDictionary buffer)
            where TDictionary : IDictionary<Coordinate, bool> =>
            _cache.ActiveCells(buffer);

        public ConsoleGrid(int maxDimensions)
        {
            _cache = new HashGrid(new HashSet<Coordinate>(), maxDimensions);
            _maxDimensions = maxDimensions;
        }

        public ConsoleGrid(int maxDimensions, IEnumerable<Coordinate> state)
        {
            _cache = new HashGrid(new HashSet<Coordinate>(state), maxDimensions);
            _maxDimensions = maxDimensions;
        }

        internal void Center(Coordinate[] initialState)
        {
            var length = Math.Min(_maxDimensions, Console.BufferWidth);
            var height = Math.Min(_maxDimensions, Console.BufferHeight);

            var xOffset = length / 2;
            var yOffset = height / 2;

            foreach (var c in initialState)
            {
                Coordinate p = (c.X + xOffset, c.Y + yOffset);
                _cache.Add(p);
                Console.SetCursorPosition(p.X, p.Y);
                Console.Write("o");
            }
        }

        public bool HasLiveCell(Coordinate c) => _cache.HasLiveCell(c);

        public bool HasLiveCells() => _cache.HasLiveCells();

        public void Set<TEnumerable>(TEnumerable state) where TEnumerable : IEnumerable<(Coordinate Key, bool Value)>
        {
            _cache.Set(state);
            foreach (var (Key, Value) in state)
            {
                if (Key.X <= _maxDimensions && Key.Y <= _maxDimensions
                    && Key.X >= 0 && Key.Y >= 0)
                {
                    Console.SetCursorPosition(Key.X, Key.Y);
                    Console.Write(Value ? "o" : " ");
                }
            }
        }

        public void SetDictionary<TDictionary>(TDictionary state) where TDictionary : IDictionary<Coordinate, bool>
        {
            _cache.SetDictionary(state);
            foreach (var (Key, Value) in state)
            {
                if (Key.X <= _maxDimensions && Key.Y <= _maxDimensions
                     && Key.X >= 0 && Key.Y >= 0)
                {
                    Console.SetCursorPosition(Key.X, Key.Y);
                    Console.Write(Value ? "o" : " ");
                }
            }
        }
    }
}
