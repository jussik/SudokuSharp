using System;
using System.Diagnostics;
using NUnit.Framework;

namespace SudokuSharp.Tests
{
    internal class ExamplePuzzleTests
    {
        private static readonly TestCaseData[] Puzzles = {
            new TestCaseData( @"
                003|020|600
                900|305|001
                001|806|400
                008|102|900
                700|000|008
                006|708|200
                002|609|500
                800|203|009
                005|010|300").SetArgDisplayNames("Easy"),
            new TestCaseData( @"
                043|080|250
                600|000|000
                000|001|094
                900|004|070
                000|608|000
                010|200|003
                820|500|000
                000|000|005
                034|090|710").SetArgDisplayNames("Medium"),
            new TestCaseData( @"
                000|700|000
                100|000|000
                000|430|200
                000|000|006
                000|509|000
                000|000|418
                000|081|000
                002|000|050
                040|000|300").SetArgDisplayNames("Hard"),
            new TestCaseData( @"
                800|000|000
                003|600|000
                070|090|200
                050|007|000
                000|045|700
                000|100|030
                001|000|068
                008|500|010
                090|000|400").SetArgDisplayNames("Hardest")
        };

        [TestCaseSource(nameof(Puzzles))]
        public void TestPuzzle(string puzzle)
        {
            Cell[] cells = Puzzle.ParsePuzzle(puzzle);
            Solver.Result res = Solver.Solve(cells);
            //Stopwatch sw = Stopwatch.StartNew();
            //for (int i = 0; i < 1000; i++)
            //{
            //    Solver.Solve(cells);
            //}
            //Console.WriteLine(sw.Elapsed);
            Assert.That(res, Is.EqualTo(Solver.Result.Success));
        }
    }
}
