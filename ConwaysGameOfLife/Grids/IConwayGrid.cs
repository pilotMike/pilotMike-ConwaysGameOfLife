using System.Collections.Generic;

namespace ConwaysGameOfLife.Grids
{
    public interface IConwayGrid
    {
        ///<summary> Clears the buffer and assigns all active (dead and alive cells).
        ///Returns the provided buffer.
        /// </summary>
        TDictionary ActiveCells<TDictionary>(TDictionary buffer)
            where TDictionary : IDictionary<Coordinate, bool>;

        bool HasLiveCell(Coordinate c);
        void Set<TEnumerable>(TEnumerable state) where TEnumerable : IEnumerable<(Coordinate, bool)>;
        void SetDictionary<TDictionary>(TDictionary state) where TDictionary : IDictionary<Coordinate, bool>;
        bool HasLiveCells();
    }
}