using System;

namespace SudokuSharp
{
    internal static class LockedSolver
    {
        public static bool Solve(Span<Cell> cells, Solver.Indexer outer, Solver.Indexer inner)
        {
            Span<int> poss = stackalloc int[3];
            bool changed = false;
            for (int outerMaj = 0; outerMaj < 3; outerMaj++)
            {
                for (int outerMin = 0; outerMin < 3; outerMin++)
                {
                    int outerRoot = outer(outerMaj * 3 + outerMin, 0);
                    // find any values that are only in one row/column of this box
                    poss.Clear();
                    int values = 0;
                    for (int major = 0; major < 3; major++)
                    {
                        // loop through inner segments, each with 3 cells
                        for (int minor = 0; minor < 3; minor++)
                        {
                            int i = outerRoot + inner(major, minor);
                            Cell cell = cells[i];
                            if (cell.Value == Cell.Unknown)
                                poss[major] |= cell.Possible;
                            else
                                values |= 1 << cell.Value;
                        }
                    }
                    for (int major = 0; major < 3; major++)
                    {
                        int uniq = poss[major]
                            & ~values
                            & ~(poss[(major + 1) % 3] | poss[(major + 2) % 3]);
                        if (uniq != 0)
                        {
                            // some unique possibles in this inner segment
                            for (int iter = 1; iter < 3; iter++)
                            {
                                int outerOffset = outer(
                                    outerMaj * 3 + (outerMin + iter) % 3,
                                    0);
                                for (int minor = 0; minor < 3; minor++)
                                {
                                    int i = outerOffset + inner(major, minor);
                                    ref Cell cell = ref cells[i];
                                    if (cell.Value == Cell.Unknown && (cell.Possible & uniq) != 0)
                                    {
                                        cell.Possible &= ~uniq;
                                        cell.CheckPossible();
                                        changed = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return changed;
        }
    }
}