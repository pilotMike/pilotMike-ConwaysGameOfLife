using ConwaysGameOfLife;
using ConwaysGameOfLife.Grids;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using static ConwaysGameOfLife.GameOfLife;

namespace ConwayGameOfLife.Tests
{
    [TestClass]
    public class RuleTests
    {
        [DataTestMethod]
        [DataRow(0, false)]
        [DataRow(1, false)]
        [DataRow(2, true)]
        [DataRow(3, true)]
        [DataRow(4, false)]
        [DataRow(5, false)]
        [DataRow(6, false)]
        [DataRow(7, false)]
        [DataRow(8, false)]
        public void LifeAndDeathRules(int neighbors, bool alive)
        {
            //var grid = new TestGrid(neighbors);
            //var game = new GameOfLife<TestGrid>(grid);
            //var result = new GameOfLife<TestGrid>.StepResult();
            //game.Step(result);

            //var aliveCount = result.State.Values.Count(a => a);
            //Assert.AreEqual(Convert.ToInt32(alive), aliveCount);
        }

        [TestMethod]
        public void Step2_OfGlider_IsCorrect()
        {
            var state = InitialStates.Glider;
            var grid = new HashGrid(new HashSet<Coordinate>(state), 80);

            var res = new StepResult();
            GameOfLife.Step(res, grid, new Dictionary<Coordinate, bool>());

            var alive = res.State.Values.Count(v => v);

            Coordinate[] expected =
            {
                (0,1),      (2,1),
                      (1,2), (2,2),
                      (1,3)
            };

            Assert.AreEqual(5, alive);
            Assert.IsTrue((
                from r in res.State where r.Value
                join e in expected on r.Key equals e into gj
                from x in gj.DefaultIfEmpty((0,0))
                select (x != (0,0))
                ).All(b => b));
        }

        //private class TestGrid : IConwayGrid
        //{
        //    private readonly int neighbors;
        //    private readonly HashGrid _grid;
        //    private int iters;

        //    public TestGrid(int neighbors) => (this.neighbors, iters, _grid) = (neighbors, 0, new HashGrid(new HashSet<Coordinate>()));

        //    public IEnumerable<Coordinate> ActiveCells => new Coordinate[] { (0, 0) };

        //    public bool HasLiveCell(Coordinate c) => iters++ < neighbors;

        //}
    }
}
