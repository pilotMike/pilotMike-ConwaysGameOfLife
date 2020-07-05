using System.Collections.Generic;

namespace ConwaysGameOfLife
{
    public interface IStepResult
    {
        void Set(Coordinate cell, bool cellState);
        IDictionary<Coordinate, bool> State { get; }
        void Clear();
    }
}