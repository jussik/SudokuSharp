using System;

namespace SudokuSharp
{
    internal static class HiddenSolver
    {
        public static bool Solve(Span<Cell> cells, Solver.Indexer indexer)
        {
            Span<int> possRows = stackalloc int[9];
            bool changed = false;
            for (int major = 0; major < 9; major++)
            {
                int any = 0; // bit is 1 if any cells have that possibility
                int ovr = 0; // bit is 1 once a second cell has it
                for (int minor = 0; minor < 9; minor++)
                {
                    int i = indexer(major, minor);
                    Cell cell = cells[i];
                    int pos = cell.Possible;
                    if (cell.Value != Cell.Unknown)
                    {
                        // value already exists, take it out of contention
                        possRows[minor] = 0;
                        ovr |= pos;
                    }
                    else
                    {
                        possRows[minor] = pos;
                        ovr |= any & pos;
                    }
                    any |= pos;
                }
                int uniqs = any ^ ovr;
                if (uniqs != 0)
                {
                    // there are bits in any that are not in ovr
                    // second pass, find cells with unique possibles
                    for (int minor = 0; minor < 9; minor++)
                    {
                        int p = possRows[minor] & uniqs;
                        if (p != 0)
                        {
                            // cell has unique possible
                            int i = indexer(major, minor);
                            cells[i].Possible = p;
                            changed |= cells[i].CheckPossible();
                        }
                    }
                }
            }
            return changed;
        }
    }
}