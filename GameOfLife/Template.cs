using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GameOfLife
{
    public class Template : ITemplate
    {
        public Template(string name, int height, int width, Cell[][] cells)
        {
            if(string.IsNullOrWhiteSpace(name) || width < 1 || height < 1 || 
               cells == null || cells.Length != height || cells.Any(x => x.Length != width))
            {
                throw new ArgumentException();
            }

            Name = name;
            Height = height;
            Width = width;
            Cells = cells;
        }

        public string Name { get; }
        public int Height { get; }
        public int Width { get; }
        public Cell[][] Cells { get; }

        public override string ToString()
        {
            StringBuilder output = new StringBuilder();

            output.Append(Height);
            output.AppendLine();
            output.Append(Width);
            output.AppendLine();

            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    char cell;
                    switch(Cells[y][x])
                    {
                        case Cell.Alive:
                            cell = CellUtilities.InputAliveCell;
                            break;

                        case Cell.Dead:
                            cell = CellUtilities.InputDeadCell;
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                    output.Append(cell);
                }
                output.AppendLine();
            }

            return output.ToString();
        }

        public static Cell[][] GetCells(int height, int width, string[] lines)
        {
            if(height < 1 || width < 1 || 
               lines == null || lines.Length != height || lines.Any(x => x.Length != width))
            {
                throw new ArgumentException();
            }

            Cell[][] cells = new Cell[height][];
            for(int y = 0; y < height; y++)
            {
                cells[y] = new Cell[width];
                for(int x = 0; x < width; x++)
                {
                    cells[y][x] = CellUtilities.GetCell(lines[y][x]);
                }
            }

            return cells;
        }

        public static void SaveTemplate(Template template)
        {
            using(StreamWriter templateFile = File.CreateText($@"Templates\{template.Name}.txt"))
            {
                templateFile.Write(template);
            }
        }

        public static string[] GetTemplateNames()
        {
            return Directory.GetFiles(@"Templates", "*.txt").Select(Path.GetFileNameWithoutExtension).ToArray();
        }

        public static Template LoadTemplate(string name)
        {
            string filePath = $@"Templates\{name}.txt";

            if(!File.Exists(filePath))
            {
                return null;
            }

            string[] templateFile = File.ReadAllLines(filePath);

            int height = int.Parse(templateFile[0]);
            int width = int.Parse(templateFile[1]);
            string[] lines = templateFile.Skip(2).ToArray();

            Cell[][] cells = GetCells(height, width, lines);

            return new Template(name, height, width, cells);
        }
    }
}
