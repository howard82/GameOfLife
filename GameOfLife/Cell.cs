using System;
using System.Text;

namespace GameOfLife
{
    public enum Cell
    {
        Alive,
        Dead
    }

    public static class CellUtilities
    {
        public const char InputAliveCell = 'O';
        public const char InputDeadCell = 'X';

        public static bool IsCell(char cell)
        {
            cell = char.ToUpper(cell);

            return cell == InputAliveCell || cell == InputDeadCell;
        }

        public static Cell GetCell(char cell)
        {
            switch(char.ToUpper(cell))
            {
                case InputAliveCell:
                    return Cell.Alive;
                
                case InputDeadCell:
                    return Cell.Dead;

                default:
                    throw new InvalidOperationException();
            }
        }

        public static char ToChar(Cell cell)
        {
            const char outputAliveCell = '\u2588';
            const char outputDeadCell = ' ';

            switch(cell)
            {
                case Cell.Alive:
                    return outputAliveCell;

                case Cell.Dead:
                    return outputDeadCell;

                default:
                    throw new InvalidOperationException();
            }
        }

        public static string ToString(Cell[][] cells)
        {
            StringBuilder output = new StringBuilder();

            foreach(Cell[] row in cells)
            {
                foreach(Cell cell in row)
                {
                    output.Append(ToChar(cell));
                }
                output.AppendLine();
            }

            return output.ToString();
        }
    }
}
