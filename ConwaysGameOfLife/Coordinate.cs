using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ConwaysGameOfLife
{
    [DebuggerDisplay("({X}, {Y})")]
    public readonly struct Coordinate
    {
        public Coordinate(int x, int y) => (X, Y) = (x, y);

        public readonly int X;
        public readonly int Y;

        public static implicit operator Coordinate((int x, int y) t) => new Coordinate(t.x, t.y);
        public static bool operator ==(Coordinate a, Coordinate b) => a.Equals(b);
        public static bool operator !=(Coordinate a, Coordinate b) => !a.Equals(b);

        public override bool Equals(object obj) =>
            obj is Coordinate coordinate &&
                   X == coordinate.X &&
                   Y == coordinate.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }

    public static class Ext
    {
        public static IEnumerable<Coordinate> Neighbors(this Coordinate c) => new Coordinate[] {
            (c.X - 1, c.Y - 1),
            (c.X, c.Y - 1),
            (c.X + 1, c.Y - 1),
            (c.X - 1, c.Y),
            (c.X + 1, c.Y),
            (c.X - 1, c.Y + 1),
            (c.X, c.Y + 1),
            (c.X + 1, c.Y + 1)
        };
    }
}