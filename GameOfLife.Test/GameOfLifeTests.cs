using System;
using Xunit;
// ReSharper disable RedundantExplicitArraySize
// ReSharper disable ObjectCreationAsStatement

namespace GameOfLife.Test
{
    public class GameOfLifeTests
    {
        private Cell[][] CreateCells(int height, int width, Cell cell = Cell.Dead)
        {
            Cell[][] cells = new Cell[height][];
            for(int i = 0; i < height; i++)
            {
                cells[i] = new Cell[width];
                for(int j = 0; j < width; j++)
                {
                    cells[i][j] = cell;
                }
            }

            return cells;
        }

        [Fact]
        public void CreateTemplate()
        {
            const string name = "Test";
            const int height = 7;
            const int width = 5;
            Cell[][] cells = CreateCells(height, width);

            ITemplate template = new Template(name, height, width, cells);

            Assert.True(template.Name == name && template.Height == height && template.Width == width);
            Assert.Equal(cells, template.Cells);

            // ToString() for Template.
            Assert.Equal(
@"
7
5
XXXXX
XXXXX
XXXXX
XXXXX
XXXXX
XXXXX
XXXXX
".TrimStart(),
            template.ToString());

            cells = CreateCells(height, width, Cell.Alive);
            template = new Template(name, height, width, cells);

            Assert.True(template.Name == name && template.Height == height && template.Width == width);
            Assert.Equal(cells, template.Cells);

            // ToString() for Template.
            Assert.Equal(
@"
7
5
OOOOO
OOOOO
OOOOO
OOOOO
OOOOO
OOOOO
OOOOO
".TrimStart(),
            template.ToString());
        }

        [Fact]
        public void CreateInvalidTemplate()
        {
            const string name = "Test";
            const int height = 7;
            const int width = 5;

            // Swapped around height and width.
            Cell[][] cells = CreateCells(width, height);

            Assert.ThrowsAny<Exception>(() =>
            {
                new Template(name, height, width, cells);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new Template(name, height, width, null);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new Template(name, 0, width, cells);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new Template(name, -1, width, cells);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new Template(name, height, 0, cells);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new Template(name, height, -1, cells);
            });
        }

        [Fact]
        public void CreateGameOfLifeBlinker()
        {
            const string name = "Blinker";
            const int height = 5;
            const int width = 5;
            Cell[][] cells = 
            new Cell[height][] 
            {
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Alive, Cell.Alive, Cell.Alive, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead }
            };
            Cell[][] otherCells =
            new Cell[height][]
            {
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Alive, Cell.Dead, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Alive, Cell.Dead, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Alive, Cell.Dead, Cell.Dead },
                new Cell[width] { Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead, Cell.Dead }
            };

            ITemplate template = new Template(name, height, width, cells);
            IGameOfLife gameOfLife = new GameOfLife(height, width);

            Assert.True(gameOfLife.Height == height && gameOfLife.Width == width);
            gameOfLife.InsertTemplate(template, 0, 0);

            Assert.Equal(cells, gameOfLife.Cells);
            gameOfLife.TakeTurn();
            Assert.Equal(otherCells, gameOfLife.Cells);
            gameOfLife.TakeTurn();
            Assert.Equal(cells, gameOfLife.Cells);
            gameOfLife.TakeTurn();
            Assert.Equal(otherCells, gameOfLife.Cells);

            // ToString() for GameOfLife.
            Assert.Equal(
@"
     
  █  
  █  
  █  
     
".Substring("\r\n".Length),
            gameOfLife.ToString());

            gameOfLife.TakeTurn();
            Assert.Equal(cells, gameOfLife.Cells);

            // ToString() for GameOfLife.
            Assert.Equal(
@"
     
     
 ███ 
     
     
".Substring("\r\n".Length),
            gameOfLife.ToString());
        }

        [Fact]
        public void CreateGameOfLifeGlider()
        {
            const string name = "Glider";
            const int templateHeight = 3;
            const int templateWidth = 3;
            Cell[][] cells =
            new Cell[templateHeight][]
            {
                new Cell[templateWidth] { Cell.Alive, Cell.Dead, Cell.Alive },
                new Cell[templateWidth] { Cell.Dead, Cell.Alive, Cell.Alive },
                new Cell[templateWidth] { Cell.Dead, Cell.Alive, Cell.Dead }
            };
            const int gameHeight = 10;
            const int gameWidth = 10;

            ITemplate template = new Template(name, templateHeight, templateWidth, cells);
            IGameOfLife gameOfLife = new GameOfLife(gameHeight, gameWidth);

            Assert.True(gameOfLife.Height == gameHeight && gameOfLife.Width == gameWidth);
            gameOfLife.InsertTemplate(template, 2, 1);
            Assert.Equal(
@"
          
  █ █     
   ██     
   █      
          
          
          
          
          
          
".Substring("\r\n".Length),
            gameOfLife.ToString());
            gameOfLife.TakeTurn();
            Assert.Equal(
@"
          
    █     
  █ █     
   ██     
          
          
          
          
          
          
".Substring("\r\n".Length),
            gameOfLife.ToString());
            gameOfLife.TakeTurn();
            Assert.Equal(
@"
          
   █      
    ██    
   ██     
          
          
          
          
          
          
".Substring("\r\n".Length),
            gameOfLife.ToString());
            gameOfLife.TakeTurn();
            Assert.Equal(
@"
          
    █     
     █    
   ███    
          
          
          
          
          
          
".Substring("\r\n".Length),
            gameOfLife.ToString());
            gameOfLife.TakeTurn();
            Assert.Equal(
@"
          
          
   █ █    
    ██    
    █     
          
          
          
          
          
".Substring("\r\n".Length),
            gameOfLife.ToString());
        }

        [Fact]
        public void CreateInvalidGameOfLife()
        {
            const string name = "Test";
            const int height = 7;
            const int width = 5;
            Cell[][] cells = CreateCells(height, width);

            ITemplate template = new Template(name, height, width, cells);

            Assert.ThrowsAny<Exception>(() =>
            {
                new GameOfLife(0, width);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new GameOfLife(-1, width);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new GameOfLife(height, 0);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                new GameOfLife(height, -1);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                IGameOfLife gameOfLife = new GameOfLife(height, width);
                gameOfLife.InsertTemplate(template, 1, 0);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                IGameOfLife gameOfLife = new GameOfLife(height, width);
                gameOfLife.InsertTemplate(template, 0, 1);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                IGameOfLife gameOfLife = new GameOfLife(height, width);
                gameOfLife.InsertTemplate(template, 1, 1);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                IGameOfLife gameOfLife = new GameOfLife(height + 10, width);
                gameOfLife.InsertTemplate(template, 1 + 10, 1);
            });
            Assert.ThrowsAny<Exception>(() =>
            {
                IGameOfLife gameOfLife = new GameOfLife(height, width + 10);
                gameOfLife.InsertTemplate(template, 1, 1 + 10);
            });
        }
    }
}
