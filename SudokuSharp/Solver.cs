using System;

namespace SudokuSharp
{
    internal interface IIndexer
    {
        int Get(int major, int minor);
    }
    internal struct RowsIndexer : IIndexer
    {
        public int Get(int major, int minor) => major * 9 + minor;
    }
    internal struct ColsIndexer : IIndexer
    {
        public int Get(int major, int minor) => minor * 9 + major;
    }
    internal struct BoxRowsIndexer : IIndexer
    {
        public int Get(int major, int minor) => (major % 3) * 3
            + (major / 3) * 27
            + (minor % 3)
            + (minor / 3) * 9;
    }
    internal struct BoxColsIndexer : IIndexer
    {
        public int Get(int major, int minor) => (major % 3) * 27
            + (major / 3) * 3
            + (minor % 3) * 9
            + (minor / 3);
    }

    internal static class Solver
    {
        private const int MaxGuessLevels = 10;
        public enum Result
        {
            Success, Incomplete, Invalid, OutOfGuesses
        }

        public static RowsIndexer RowsIndex;
        public static ColsIndexer ColsIndex;
        public static BoxRowsIndexer BoxRowsIndex;
        public static BoxColsIndexer BoxColsIndex;

        public static Result Solve(Span<Cell> cells, int guessLevel = 0)
        {
            while (true)
            {
                bool changed = false;
                changed |= SimpleSolver.Solve(cells, ref RowsIndex);
                changed |= SimpleSolver.Solve(cells, ref ColsIndex);
                changed |= SimpleSolver.Solve(cells, ref BoxRowsIndex);
                changed |= HiddenSolver.Solve(cells, ref RowsIndex);
                changed |= HiddenSolver.Solve(cells, ref ColsIndex);
                changed |= HiddenSolver.Solve(cells, ref BoxRowsIndex);
                changed |= LockedSolver.Solve(cells, ref BoxRowsIndex, ref RowsIndex);
                changed |= LockedSolver.Solve(cells, ref BoxColsIndex, ref ColsIndex);
                changed |= LockedSolver.Solve(cells, ref RowsIndex, ref BoxRowsIndex);
                changed |= LockedSolver.Solve(cells, ref ColsIndex, ref BoxColsIndex);
                changed |= NakedSolver.Solve(cells, ref RowsIndex);
                changed |= NakedSolver.Solve(cells, ref ColsIndex);
                changed |= NakedSolver.Solve(cells, ref BoxRowsIndex);

                if (Verify(cells, ref RowsIndex, out Result result)
                    && Verify(cells, ref ColsIndex, out result)
                    && Verify(cells, ref BoxRowsIndex, out result)
                    || result == Result.Invalid)
                    return result;
                if (!changed)
                    return guessLevel > MaxGuessLevels
                        ? Result.OutOfGuesses
                        : Guess(cells, guessLevel);
            }
        }

        /// <summary>
        /// Verify a single axis in the puzzle.
        /// True means continue verification, false means further verification not required.
        /// </summary>
        private static bool Verify<T>(Span<Cell> cells, ref T indexer, out Result result)
            where T : struct, IIndexer
        {
            result = Result.Success;
            for (int major = 0; major < 9; major++)
            {
                int valMask = 0;
                for (int minor = 0; minor < 9; minor++)
                {
                    Cell cell = cells[indexer.Get(major, minor)];
                    if (cell.Value == Cell.Unknown)
                    {
                        if (cell.Possible == 0)
                        {
                            // no remaining possibilities
                            result = Result.Invalid;
                            return false;
                        }
                        result = Result.Incomplete;
                    }
                    else
                    {
                        // check if all 9 values are present
                        int valBit = 1 << cell.Value;
                        // duplicate value if valBit already added
                        if ((valMask & valBit) != 0)
                        {
                            result = Result.Invalid;
                            return false;
                        }
                        valMask |= valBit;
                    }
                }
            }

            return result == Result.Success;
        }

        private static Result Guess(Span<Cell> cells, int guessLevel)
        {
            Span<Cell> newCells = stackalloc Cell[81];
            for (int poss = 2; poss < 10; poss++)
            {
                // find cells with least possibles first
                for (int c = 0; c < 81; c++)
                {
                    Cell cell = cells[c];
                    if (cell.Value == Cell.Unknown)
                    {
                        int p = cell.Possible;
                        int v = 0;
                        if (Utils.CountOnes(p) == poss)
                        {
                            while (p != 0)
                            {
                                // check next LSB of possibles
                                if ((p & 1) != 0)
                                {
                                    cells.CopyTo(newCells);
                                    // just make a guess from one of the remaining possible values
                                    newCells[c].Value = v;
                                    newCells[c].Possible = 1 << v;
                                    if (Solve(newCells, guessLevel + 1) == Result.Success)
                                    {
                                        newCells.CopyTo(cells);
                                        return Result.Success;
                                    }
                                }

                                // remove LSB from possibles
                                p >>= 1;
                                v += 1;
                            }

                            // No remaining possible values for cell
                            return Result.Invalid;
                        }
                    }
                }
            }

            // No unknown values, but still not solved
            return Result.Invalid;
        }
    }
}