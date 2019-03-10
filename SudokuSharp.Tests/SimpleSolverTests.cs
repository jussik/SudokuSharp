using System.Linq;
using NUnit.Framework;

namespace SudokuSharp.Tests
{
    internal class SimpleSolverTests
    {
        private static readonly TestCaseData[] Indexers = {
            new TestCaseData((Solver.Indexer)Solver.RowsIndex).SetArgDisplayNames("Rows"),
            new TestCaseData((Solver.Indexer)Solver.ColsIndex).SetArgDisplayNames("Cols"),
            new TestCaseData((Solver.Indexer)Solver.BoxRowsIndex).SetArgDisplayNames("BoxRows"),
        };

        [TestCaseSource(nameof(Indexers))]
        public void TestSolveSingle(Solver.Indexer indexer)
        {
            Cell[] cells = Enumerable.Repeat(new Cell(-1), 81).ToArray();
            // set segment values to 1-8, with final cell left blank
            for (int i = 0; i < 8; i++)
            {
                cells[indexer(0, i)] = new Cell(i);
            }

            int tgtIndex = indexer(0, 8);
            Assert.That(cells[tgtIndex].Value, Is.EqualTo(Cell.Unknown));
            SimpleSolver.Solve(cells, indexer);
            SimpleSolver.Solve(cells, indexer);
            // final value should be 9
            Assert.That(cells[tgtIndex].Value, Is.EqualTo(8));
        }

        [TestCaseSource(nameof(Indexers))]
        public void TestRemovesPossibles(Solver.Indexer indexer)
        {
            Cell[] cells = Enumerable.Repeat(new Cell(-1), 81).ToArray();
            // set 3rd element to 3
            cells[indexer(0, 2)] = new Cell(2);

            int tgtIndex = indexer(0, 7);
            Assert.That(cells[tgtIndex].IsPossible(2), Is.True);
            SimpleSolver.Solve(cells, indexer);
            SimpleSolver.Solve(cells, indexer);
            // 8th element in same segment should not have 3 be possible
            Assert.That(cells[tgtIndex].IsPossible(2), Is.False);
        }
    }
}