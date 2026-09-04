using BomberRoyal.Core.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;


namespace BomberRoyal.Core.Features.BoardFeature
{
    // Recursive Division Algoritm för maze generation https://weblog.jamisbuck.org/2011/1/12/maze-generation-recursive-division-algorithm
    public class MazeGenerator
    {
        private readonly RandomGenerator rnd = new();
        public string[,] GameBoard { get; set; }

        public string[,] BorderWalls(int height, int width)
        {
            GameBoard = new string[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (y == 0 || y == height - 1 || x == 0 || x == width - 1)
                        GameBoard[y, x] = "@";
                    else
                        GameBoard[y, x] = " ";
                }
            }
            return GameBoard;
        }
        // Delar området i två delar med en vägg och gör en öppning.

        public void Generate(int x, int y, int width, int height)
        {
            if (width <= 1 || height <= 1)
                return;

            bool horizontal = rnd.GenerateMazeRandom(0, 2) == 0;

            if (horizontal)
            {
                int wallY = rnd.GenerateMazeRandom(y + 1, y + height - 2);
                int doorX = FindDoorX(x, width, wallY);
                // Rita horisontell vägg, men lämna ett hål
                for (int i = x; i < x + width && i < GameBoard.GetLength(1); i++)
                {
                    if (i != doorX && wallY < GameBoard.GetLength(0))
                        GameBoard[wallY, i] = "#";
                }

                int upperHeight = wallY - y;
                int lowerHeight = (y + height) - (wallY + 1);

                Generate(x, y, width, upperHeight);
                Generate(x, wallY + 1, width, lowerHeight);
            }
            else
            {
                int wallX = rnd.GenerateMazeRandom(x + 1, x + width - 2);
                int doorY = FindDoorY(y, height, wallX);
                // Rita vertikal vägg med öppning

                for (int i = y; i < y + height && i < GameBoard.GetLength(0); i++)
                {
                    if (i != doorY && wallX < GameBoard.GetLength(1))
                        GameBoard[i, wallX] = "#";
                }
                // Rekursivt dela vänster och höger sida
                int leftWidth = wallX - x;
                int rightWidth = (x + width) - (wallX + 1);

                Generate(x, y, leftWidth, height);
                Generate(wallX + 1, y, rightWidth, height);
            }
        }
        // Letar efter en lämplig X-position för dörren i en horisontell vägg.
        private int FindDoorX(int x, int width, int y)
        {
            var candidates = new List<int>();

            for (int i = x; i < x + width && i < GameBoard.GetLength(1); i++)
            {
                if (GameBoard[y, i] != "#" &&
                    (y - 1 < 0 || GameBoard[y - 1, i] != "#") &&
                    (y + 1 >= GameBoard.GetLength(0) || GameBoard[y + 1, i] != "#"))
                {
                    candidates.Add(i);
                }
            }

            if (candidates.Count == 0)
                return rnd.GenerateMazeRandom(x, x + width - 1);

            return candidates[rnd.GenerateMazeRandom(0, candidates.Count)];
        }
        // Letar efter en Y-position för dörren i en vertikal vägg
        private int FindDoorY(int y, int height, int x)
        {
            var candidates = new List<int>();

            for (int i = y; i < y + height && i < GameBoard.GetLength(0); i++)
            {
                if (GameBoard[i, x] != "#" &&
                    (x - 1 < 0 || GameBoard[i, x - 1] != "#") &&
                    (x + 1 >= GameBoard.GetLength(1) || GameBoard[i, x + 1] != "#"))
                {
                    candidates.Add(i);
                }
            }

            if (candidates.Count == 0)
                return rnd.GenerateMazeRandom(y, y + height - 1);

            return candidates[rnd.GenerateMazeRandom(0, candidates.Count)];
        }
        // Snyggar till labyrinten genom att slumpmässigt ta bort vissa väggar
        public void SmoothMaze(string[,] board)
        {
            int h = board.GetLength(0);
            int w = board.GetLength(1);

            var rnd = new Random();
            for (int y = 1; y < h - 1; y++)
            {
                for (int x = 1; x < w - 1; x++)
                {
                    if (board[y, x] == "#")
                    {
                        bool verticalOpen = board[y - 1, x] == " " && board[y + 1, x] == " ";
                        bool horizontalOpen = board[y, x - 1] == " " && board[y, x + 1] == " ";
                        // ~25% chans att ta bort en vägg som ligger mellan öppna ytor
                        if ((verticalOpen || horizontalOpen) && rnd.NextDouble() < 0.25)
                            board[y, x] = " ";
                    }
                }
            }
            Print();

        }
        public void Print()
        {
            for (int y = 0; y < GameBoard.GetLength(0); y++)
            {
                for (int x = 0; x < GameBoard.GetLength(1); x++)
                    Console.Write(GameBoard[y, x]);
                Console.WriteLine();
            }
        }
    }
}
