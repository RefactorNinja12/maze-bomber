using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.Board
{
    public class MazeData
    {
        public Tile[,] Grid { get; private set; }
        public int Width => Grid.GetLength(1);
        public int Height => Grid.GetLength(0);

        public MazeData(int width, int height)
        {
            Grid = new Tile[height, width];
        }

        public void SetTile(int x, int y, Tile tile)
        {
            Grid[y, x] = tile;
        }

        public Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                throw new IndexOutOfRangeException();
            return Grid[y, x];
        }

        public void Print()
        {
            for (int y = 0; y < Grid.GetLength(0); y++)
            {
                for (int x = 0; x < Grid.GetLength(1); x++)
                    Console.Write(Grid[y, x]);
                Console.WriteLine();
            }
        }
    }
}
