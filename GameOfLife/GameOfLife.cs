using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GameOfLife
{
    public class GameOfLife : IGameOfLife
    {
        public GameOfLife(int height, int width)
        {
            if(height < 1 || width < 1)
            {
                throw new ArgumentException();
            }

            Height = height;
            Width = width;
        }

        [JsonConstructor]
        // ReSharper disable once UnusedMember.Local
        private GameOfLife(int height, int width, Cell[][] cells) : this(height, width)
        {
            Cells = cells;
        }

        public int Height { get; }
        public int Width { get; }
        public Cell[][] Cells { get; private set; }

        public void InsertTemplate(ITemplate template, int templateX, int templateY)
        {
            if(templateX < 0 || templateY < 0 ||
               templateX + template.Width > Width || templateY + template.Height > Height)
            {
                throw new ArgumentException();
            }

            InitialiseCells();
            for(int y = 0; y < template.Height; y++)
            {
                for(int x = 0; x < template.Width; x++)
                {
                    Cells[y + templateY][x + templateX] = template.Cells[y][x];
                }
            }
        }

        private void InitialiseCells()
        {
            Cells = new Cell[Height][];
            for(int y = 0; y < Height; y++)
            {
                Cells[y] = new Cell[Width];
                for(int x = 0; x < Width; x++)
                {
                    Cells[y][x] = Cell.Dead;
                }
            }
        }

        public void PlayGame(ITemplate template, int templateX, int templateY)
        {
            InsertTemplate(template, templateX, templateY);
            PlayGame();
        }

        public void ResumeGame()
        {
            PlayGame();
        }

        private void PlayGame()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            Task playGameTask = PlayGameAsync(cancellationTokenSource.Token);

            // Wait for user to stop the game.
            Console.ReadKey(true);

            cancellationTokenSource.Cancel();
            try
            {
                playGameTask.Wait(cancellationTokenSource.Token);
            }
            catch(OperationCanceledException)
            { }

            SaveGameOfLife(this);
            Console.WriteLine("Game stopped - the state has been saved.");
        }

        private async Task PlayGameAsync(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                Console.Clear();
                Console.WriteLine(this);
                Console.WriteLine("Press any key to stop game...");

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    TakeTurn();
                }
                catch(TaskCanceledException)
                { }
            }
        }

        public void TakeTurn()
        {
            // Rules:
            //  1. Any live cell with fewer than two live neighbours dies, as if caused by under-population.
            //  2. Any live cell with two or three live neighbours lives on to the next generation.
            //  3. Any live cell with more than three live neighbours dies, as if by over-population.
            //  4. Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
            Cell[][] newCells = new Cell[Height][];
            for(int y = 0; y < Height; y++)
            {
                newCells[y] = new Cell[Width];
                for(int x = 0; x < Width; x++)
                {
                    Cell newCell;
                    int aliveNeighbours = AliveNeighbours(x, y);
                    switch(Cells[y][x])
                    {
                        case Cell.Alive:
                            if(aliveNeighbours < 2)
                            {
                                newCell = Cell.Dead;
                            }
                            else if(aliveNeighbours == 2 || aliveNeighbours == 3)
                            {
                                newCell = Cell.Alive;                                
                            }
                            else // More than 3.
                            {
                                newCell = Cell.Dead;
                            }
                            break;

                        case Cell.Dead:
                            newCell = aliveNeighbours == 3 ? Cell.Alive : Cell.Dead;
                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    newCells[y][x] = newCell;
                }
            }

            Cells = newCells;
        }

        private int AliveNeighbours(int xCoordinate, int yCoordinate)
        {
            int aliveNeighbours = 0;
            for(int y = yCoordinate - 1; y <= yCoordinate + 1; y++)
            {
                if(y < 0 || y >= Height)
                {
                    continue;
                }

                for(int x = xCoordinate - 1; x <= xCoordinate + 1; x++)
                {
                    if(x < 0 || x >= Width || (y == yCoordinate && x == xCoordinate))
                    {
                        continue;
                    }
                    if(Cells[y][x] == Cell.Alive)
                    {
                        aliveNeighbours++;
                    }
                }
            }

            return aliveNeighbours;
        }

        public override string ToString()
        {
            return CellUtilities.ToString(Cells);
        }

        private const string GameOfLifeFilePath = @"GameOfLife\GameOfLife.json";

        public static void SaveGameOfLife(GameOfLife gameOfLife)
        {
            File.WriteAllText(GameOfLifeFilePath, JsonConvert.SerializeObject(gameOfLife));
        }

        public static GameOfLife LoadGameOfLife()
        {
            return File.Exists(GameOfLifeFilePath) ? JsonConvert.DeserializeObject<GameOfLife>(File.ReadAllText(GameOfLifeFilePath)) : null;
        }
    }
}
