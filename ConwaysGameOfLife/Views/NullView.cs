using ConwaysGameOfLife.Grids;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConwaysGameOfLife.Views
{
    public struct NullView : IView
    {
        public int Dimensions { get; set; }

        public void Set((Coordinate c, bool alive) cell)
        {
            
        }

        public void Set<TConwayGrid>(TConwayGrid grid) where TConwayGrid : IConwayGrid
        {
            
        }
    }
}
