
using BomberRoyal.Core.Features.Board;
using MonoGameLibrary.Graphics;
using System;


namespace BomberRoyal.Core.Features.BoardFeature
{
    // Representerar en spelplan (maze) som ett 2D-nät av Tile-objekt.
    // Ansvarar för att lagra, hämta och kontrollera information om varje ruta i labyrinten.
    public class MazeData
    {
        public Tile[,] Grid { get; private set; }
        public int Width => Grid.GetLength(1);
        public int Height => Grid.GetLength(0);

        public MazeData(int width, int height)
        {
            Grid = new Tile[height, width];
        }

        // Returnerar true om positionen innehåller en tile som kan är golv
        public bool IsWalkableTile(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;
            return GetTile(x, y).Type == TileType.Floor;
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
