//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace ConwaysGameOfLife.Grids
//{
//    public sealed class ArrayGrid : IConwayGrid
//    {
//        private readonly bool[,] _grid;

//        public ArrayGrid(int size)
//        {
//            _grid = new bool[size, size];
//            //_size = size;
//        }

//        public TDictionary ActiveCells<TDictionary>(TDictionary buffer) where TDictionary : IDictionary<Coordinate, bool>
//        {
//            buffer.Clear();

//            for (int i = 0; i < _grid.GetLength(0); i++)
//            {
//                for (int j = 0; j < _grid.GetLength(1); j++)
//                {
//                    if (_grid[i, j])
//                    {
                        
//                        buffer[(j, i)] = true;

//                        Grid.AddNeighbors(buffer, _grid, (j,i));
//                    }
//                }
//            }
//        }

//        public bool HasLiveCell(Coordinate c)
//        {
//            throw new NotImplementedException();
//        }

//        public bool HasLiveCells()
//        {
//            throw new NotImplementedException();
//        }

//        public void Set<TEnumerable>(TEnumerable state) where TEnumerable : IEnumerable<(Coordinate, bool)>
//        {
//            throw new NotImplementedException();
//        }

//        public void SetDictionary<TDictionary>(TDictionary state) where TDictionary : IDictionary<Coordinate, bool>
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
