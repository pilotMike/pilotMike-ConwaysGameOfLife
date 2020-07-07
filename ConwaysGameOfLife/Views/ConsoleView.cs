using ConwaysGameOfLife.Grids;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConwaysGameOfLife.Views
{
    public struct ConsoleView : IView
    {
        public int Dimensions { get; set; }

        public void Set<TConwayGrid>(TConwayGrid grid) where TConwayGrid : IConwayGrid
        {
            
        }

        public void Set((Coordinate c, bool alive) cell)
        {
            var (c, alive) = cell;
            if (c.X <= Dimensions && c.Y <= Dimensions
                && c.X >= 0 && c.Y >= 0)
            {
                Console.SetCursorPosition(c.X, c.Y);
                Console.Write(alive ? "o" : " ");
            }
        }
    }
}
