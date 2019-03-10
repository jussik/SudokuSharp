using System;

namespace SudokuSharp
{
    internal static class SimpleSolver
    {
        public static bool Solve(Span<Cell> cells, Solver.Indexer indexer)
        {
            bool changed = false;
            for (int major = 0; major < 9; major++)
            {
                for (int minor = 0; minor < 8; minor++)
                {
                    int i = indexer(major, minor);
                    Cell ci = cells[i];
                    for (int minorAdj = minor + 1; minorAdj < 9; minorAdj++)
                    {
                        int j = indexer(major, minorAdj);
                        Cell cj = cells[j];
                        if (ci.Value != Cell.Unknown)
                        {
                            if (cj.Value == Cell.Unknown && cj.IsPossible(ci.Value))
                            {
                                changed |= cells[j].TryRemovePossible(ci.Value);
                            }
                        }
                        else if (cj.Value != Cell.Unknown && ci.IsPossible(cj.Value))
                        {
                            changed |= cells[i].TryRemovePossible(cj.Value);
                        }
                    }
                }
            }
            return changed;
        }
    }
}