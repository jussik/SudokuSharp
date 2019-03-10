using System.Linq;
using System.Text;

namespace SudokuSharp
{
    internal static class Puzzle
    {
        public static Cell[] ParsePuzzle(string puzzleString)
        {
            return puzzleString
                .Where(ch => ch >= '0' && ch <= '9')
                .Select(ch => new Cell(ch - '0' - 1)) // map [0-9] to -1,[0-8]
                .ToArray();
        }

        public static string FormatResults(string name, Solver.Result result, Cell[] before, Cell[] after)
        {
            var sb = new StringBuilder();
            void AppendSeparator()
            {
                sb.Append('-', 13);
                sb.Append("   ");
                sb.Append('-', 13);
                sb.AppendLine();
            }
            sb.Append(name);
            sb.Append(": ");
            sb.Append(result);
            sb.AppendLine();
            for (int major = 0; major < 9; major++)
            {
                void AppendRow(Cell[] cells)
                {
                    for (int minor = 0; minor < 9; minor++)
                    {
                        if (minor % 3 == 0)
                            sb.Append("|");
                        int value = cells[Solver.RowsIndex(major, minor)].Value;
                        sb.Append(value == Cell.Unknown ? " " : (value + 1).ToString());
                    }
                }

                if (major % 3 == 0)
                    AppendSeparator();
                AppendRow(before);
                sb.Append(major == 4 ? "| > " : "|   ");
                AppendRow(after);
                sb.Append('|');
                sb.AppendLine();
            }
            AppendSeparator();
            return sb.ToString();
        }

        public static string FormatCells(Cell[] puzzle)
        {
            var sb = new StringBuilder();
            void AppendSeparator()
            {
                sb.Append('-', 13);
                sb.AppendLine();
            }
            for (int major = 0; major < 9; major++)
            {
                void AppendRow(Cell[] cells)
                {
                    for (int minor = 0; minor < 9; minor++)
                    {
                        if (minor % 3 == 0)
                            sb.Append("|");
                        int value = cells[Solver.RowsIndex(major, minor)].Value;
                        sb.Append(value == Cell.Unknown ? " " : (value + 1).ToString());
                    }
                }

                if (major % 3 == 0)
                    AppendSeparator();
                AppendRow(puzzle);
                sb.Append('|');
                sb.AppendLine();
            }
            AppendSeparator();
            return sb.ToString();
        }
    }
}