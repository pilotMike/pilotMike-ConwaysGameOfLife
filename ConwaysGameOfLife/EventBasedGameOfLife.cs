using ConwaysGameOfLife.Grids;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace ConwaysGameOfLife
{
    public sealed class EventBasedGameOfLife
    {
        // idea to have cells broadcast to each other instead of iterating and checking

        public void Run(ICollection<Coordinate> initialState, int dimensions)
        {
            var grid = new EventGrid(dimensions, (Coordinate c, Cell cell, bool isAlive) => {
                Console.SetCursorPosition(c.X, c.Y);
                Console.Write(isAlive ? "o" : " ");
                Thread.Sleep(100);
                Debug.WriteLine("Writing {0},{1} with {2} and {3} neighbors", c.X, c.Y, isAlive, cell.Neighbors);
            }, new HashSet<Coordinate>(initialState));
        }

        private sealed class EventGrid
        {
            private readonly Cell[,] _grid;

            public EventGrid(int dimensions, Action<Coordinate, Cell, bool> onStatusChanged, HashSet<Coordinate>  initialState)
            {
                _grid = new Cell[dimensions, dimensions];

                Coordinate[] cs = new Coordinate[8];
                // assign event handlers
                for (int i = 0; i <= _grid.GetUpperBound(0); i++)
                {
                    for (int j = 0; j <= _grid.GetUpperBound(1); j++)
                    {
                        var cell = new Cell();
                        Coordinate coord = (j, i);
                        _grid[i,j] = cell;
                        cell.StatusChanged += (_, isAlive) =>
                                    onStatusChanged(coord, cell, isAlive);
                    }
                }

                for (int i = 0; i < _grid.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < _grid.GetUpperBound(1); j++)
                    {
                        Coordinate c = (j, i);
                        var cell = _grid[i, j];
                        cs[0] = (c.X - 1, c.Y - 1);
                        cs[1] = (c.X, c.Y - 1);
                        cs[2] = (c.X + 1, c.Y - 1);
                        cs[3] = (c.X - 1, c.Y);
                        cs[4] = (c.X + 1, c.Y);
                        cs[5] = (c.X - 1, c.Y + 1);
                        cs[6] = (c.X, c.Y + 1);
                        cs[7] = (c.X + 1, c.Y + 1);

                        foreach (var cx in cs)
                            if (cx.X > -1 && cx.Y > -1 && cx.X <= _grid.GetUpperBound(1) && cx.Y <= _grid.GetUpperBound(0))
                            {
                                var target = _grid[cx.Y, cx.X];
                                target.StatusChanged += cell.Listen;
                            }
                    }
                }

                foreach (var c in initialState)
                {
                    cs[0] = (c.X - 1, c.Y - 1);
                    cs[1] = (c.X, c.Y - 1);
                    cs[2] = (c.X + 1, c.Y - 1);
                    cs[3] = (c.X - 1, c.Y);
                    cs[4] = (c.X + 1, c.Y);
                    cs[5] = (c.X - 1, c.Y + 1);
                    cs[6] = (c.X, c.Y + 1);
                    cs[7] = (c.X + 1, c.Y + 1);

                    foreach (var cx in cs)
                        if (cx.X > -1 && cx.Y > -1 && cx.X <= _grid.GetUpperBound(1) && cx.Y <= _grid.GetUpperBound(0))
                            _grid[cx.Y, cx.X].Neighbors++;
                }


            }

        }

        private class Cell
        {
            private byte neighbors;
            private bool _isAlive;
            public bool IsAlive 
            {
                get => _isAlive;
                set
                {
                    if (value == _isAlive) return;
                    _isAlive = value;
                    StatusChanged?.Invoke(this, _isAlive);
                } 
            }

            public byte Neighbors
            {
                get => neighbors; 
                set
                {
                    neighbors = value;
                    IsAlive = neighbors switch
                    {
                        2 when _isAlive => true,
                        3 => true,
                        _ => false
                    };
                }
            }

            public event EventHandler<bool> StatusChanged;

            public void Listen(object sender, bool e)
            {
                if (e) Neighbors++;
                else Neighbors--;
            }

            internal void Notify() => StatusChanged?.Invoke(this, IsAlive);
        }
    }
}
