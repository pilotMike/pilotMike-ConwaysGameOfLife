using ConwaysGameOfLife.Grids;
using ConwaysGameOfLife.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConwaysGameOfLife.GoL
{

    public class GameOfLifeOptions
    {
        public int? DelayMillis { get; set; }
        public int? MaxIterations { get; set; }
        public CancellationToken? CancellationToken { get; set; }
        public int? Dimensions { get; set; }
    }
}
