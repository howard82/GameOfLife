using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife
{
    public class Program
    {
        private static void Main(string[] args)
        {
            switch(args.Length)
            {
                case 0:
                    MainMenu();
                    break;

                case 5:
                    CommandLineArguments(args);
                    break;

                default:
                    Console.WriteLine("Incorrect number of command line arguments.");
                    break;
            }
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void CommandLineArguments(string[] args)
        {
            // <game-height> <game-width> <template-name> <template-x> <template-y>
            int argsIndex = 0;

            int height;
            if(!int.TryParse(args[argsIndex++], out height) || height < 1)
            {
                Console.WriteLine("Game height invalid.");
                return;
            }

            int width;
            if(!int.TryParse(args[argsIndex++], out width) || width < 1)
            {
                Console.WriteLine("Game width invalid.");
            }

            Template template = Template.LoadTemplate(args[argsIndex++]);
            if(template == null)
            {
                Console.WriteLine("Template not found.");
                return;
            }

            int templateX;
            if(!int.TryParse(args[argsIndex++], out templateX) || templateX < 0 || templateX > width - template.Width)
            {
                Console.WriteLine("Template x coordinate invalid.");
                return;
            }

            int templateY;
            if(!int.TryParse(args[argsIndex], out templateY) || templateY < 0 || templateY > height - template.Height)
            {
                Console.WriteLine("Template y coordinate invalid.");
                return;
            }

            GameOfLife gameOfLife = new GameOfLife(height, width);
            gameOfLife.PlayGame(template, templateX, templateY);
        }

        private static void MainMenu()
        {
            bool run = true;
            while(run)
            {
                Console.WriteLine();
                Console.WriteLine("--- Game of Life ---");
                Console.WriteLine("1. Create Template");
                Console.WriteLine("2. Play Game");
                Console.WriteLine("3. Exit");
                Console.WriteLine();

                Console.Write("Enter an option: ");
                string input = Console.ReadLine();
                Console.WriteLine();

                int option;
                if(!int.TryParse(input, out option) || option < 1 || option > 3)
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                switch(option)
                {
                    case 1:
                        CreateTemplate();
                        break;

                    case 2:
                        PlayGame();
                        break;

                    case 3:
                        run = false;
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            Console.WriteLine("Goodbye.");
        }

        private static void CreateTemplate()
        {
            Console.WriteLine("--- Create Template ---");
            Console.WriteLine();

            Console.Write("Enter template name: ");
            // ReSharper disable once PossibleNullReferenceException
            string name = Console.ReadLine().Trim();
            Console.WriteLine();

            if(string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Console.Write("Enter height: ");
            string input = Console.ReadLine();
            Console.WriteLine();

            int height;
            if(!int.TryParse(input, out height) || height < 1)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Console.Write("Enter width: ");
            input = Console.ReadLine();
            Console.WriteLine();

            int width;
            if(!int.TryParse(input, out width) || width < 1)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Console.WriteLine($"Enter cells ('{CellUtilities.InputAliveCell}' is Alive, '{CellUtilities.InputDeadCell}' is Dead):");
            Cell[][] cells = GetCells(height, width);
            Console.WriteLine();
            if(cells == null)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Template template = new Template(name, height, width, cells);
            Template.SaveTemplate(template);

            Console.WriteLine("Template created.");
        }

        private static Cell[][] GetCells(int height, int width)
        {
            string[] lines = new string[height];

            StringBuilder line = new StringBuilder(width, width);
            for(int i = 0; i < height; i++)
            {
                line.Clear();
                for(int j = 0; j < width; j++)
                {
                    char input = Console.ReadKey().KeyChar;

                    if(!CellUtilities.IsCell(input))
                    {
                        Console.WriteLine();
                        return null;
                    }
                    line.Append(input);
                }
                Console.WriteLine();

                lines[i] = line.ToString();
            }

            return Template.GetCells(height, width, lines);
        }

        private static void PlayGame()
        {
            string[] templates = Template.GetTemplateNames();

            if(!templates.Any())
            {
                Console.WriteLine("No templates found - create at least one template before playing a game.");
                return;
            }

            Console.WriteLine("--- Play Game ---");
            Console.WriteLine("1. New Game");
            Console.WriteLine("2. Resume Game");
            Console.WriteLine();

            Console.Write("Enter an option: ");
            string input = Console.ReadLine();
            Console.WriteLine();

            int option;
            if(!int.TryParse(input, out option) || option < 1 || option > 2)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            switch(option)
            {
                case 1:
                    NewGame(templates);
                    break;

                case 2:
                    ResumeGame();
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        private static void NewGame(IReadOnlyList<string> templates)
        {
            Console.WriteLine("--- New Game ---");
            Console.WriteLine("Templates:");
            for(int i = 0; i < templates.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {templates[i]}");
            }
            Console.WriteLine();

            Console.Write("Select a template: ");
            string input = Console.ReadLine();
            Console.WriteLine();

            int option;
            if(!int.TryParse(input, out option) || option < 1 || option > templates.Count)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            string name = templates[option - 1];
            Template template = Template.LoadTemplate(name);

            Console.WriteLine("Template");
            Console.WriteLine($"Name  : {template.Name}");
            Console.WriteLine($"Height: {template.Height}");
            Console.WriteLine($"Width : {template.Width}");
            Console.WriteLine(CellUtilities.ToString(template.Cells));

            NewGame(template);
        }

        private static void NewGame(ITemplate template)
        {
            Console.Write($"Enter game height (must be at least {template.Height}): ");
            string input = Console.ReadLine();
            Console.WriteLine();

            int height;
            if(!int.TryParse(input, out height) || height < 1 || height < template.Height)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Console.Write($"Enter game width (must be at least {template.Width}): ");
            input = Console.ReadLine();
            Console.WriteLine();

            int width;
            if(!int.TryParse(input, out width) || width < 1 || width < template.Width)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Console.Write($"Enter template x coordinate (cannot be more than {width - template.Width}): ");
            input = Console.ReadLine();
            Console.WriteLine();

            int templateX;
            if(!int.TryParse(input, out templateX) || templateX < 0 || templateX > width - template.Width)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            Console.Write($"Enter template y coordinate (cannot be more than {height - template.Height}): ");
            input = Console.ReadLine();
            Console.WriteLine();

            int templateY;
            if(!int.TryParse(input, out templateY) || templateY < 0 || templateY > height - template.Height)
            {
                Console.WriteLine("Invalid input.");
                return;
            }

            GameOfLife gameOfLife = new GameOfLife(height, width);
            gameOfLife.PlayGame(template, templateX, templateY);        
        }

        private static void ResumeGame()
        {
            Console.WriteLine("--- Resume Game ---");
            Console.WriteLine();

            GameOfLife gameOfLife = GameOfLife.LoadGameOfLife();
            if(gameOfLife == null)
            {
                Console.WriteLine("No previous game data present.");
                return;
            }

            gameOfLife.ResumeGame();
        }
    }
}
