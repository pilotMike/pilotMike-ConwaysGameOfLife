using ConwaysGameOfLife.Grids;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConwaysGameOfLife.Views
{
    public interface IView
    {
        int Dimensions { get; set; }

        void Set<TConwayGrid>(TConwayGrid grid) where TConwayGrid : IConwayGrid;
        void Set((Coordinate c, bool alive) cell);
    }
}
