Game of Life

**Rules**

The universe of the Game of Life is an infinite two-dimensional orthogonal grid of square cells, each of which is in one of two possible states, alive or dead. Every cell interacts with its eight neighbours, which are the cells that are horizontally, vertically, or diagonally adjacent.

At each step in time, the following transitions occur:

Any live cell with fewer than two live neighbours dies, as if caused by under-population.
Any live cell with two or three live neighbours lives on to the next generation.
Any live cell with more than three live neighbours dies, as if by over-population.
Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

The initial pattern constitutes the seed of the system. The first generation is created by applying the above rules simultaneously to every cell in the seed—births and deaths occur simultaneously, and the discrete moment at which this happens is sometimes called a tick (in other words, each generation is a pure function of the preceding one). The rules continue to be applied repeatedly to create further generations.

**Summary**

Players can make templates of cell layouts using O's and X's to define alive and dead cells respectively. These templates are used placed on a game board (size defined by user) and for each second that passes the cells to continue to live, die or are reborn based on rules relating to their proximity to other live and dead cells. The game continues until the user presses a key.

**Tests**

In-built test classes can be run to test the capacity of the program to apply the rules of the game, which conform to those defined in Conway's Game of Life.

* [Learn Markdown](https://bitbucket.org/tutorials/markdowndemo)
