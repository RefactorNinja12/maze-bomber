using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.Board;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameLibrary.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BomberRoyal.Core.Features.BoardFeature
{
    // Ansvarar för att rendera MazeData på skärmen genom att använda ett Tileset och Tilemap.
    public class MazeRenderer
    {
      
        private readonly IEventBus _eventBus;
        private MazeData _maze;
        private readonly Texture2D _pixel;
        private int _cellSize;
        private SpriteBatch _spriteBatch;
        private readonly Tileset _tileset;
        private Tilemap _tilemap;
        private Random _random;

        public MazeRenderer(Tileset tileset, Texture2D pixel, int cellSize, SpriteBatch spriteBatch, IEventBus eventBus)
        {
            _tileset = tileset;
            _pixel = pixel;
            _cellSize = cellSize;
            _spriteBatch = spriteBatch;
            _eventBus = eventBus;
            _eventBus.Subscribe<TileDestroyedEvent>(OnTileDestroyed);
            _eventBus.Subscribe<TilePlacedEvent>(OnTilePlaced);

        }
        private void OnTilePlaced(TilePlacedEvent e)
        {
            int x = e.Tile.X;
            int y = e.Tile.Y;

            var tileTypeBelow = y + 1 < _maze.Height ? _maze.GetTile(x, y + 1).Type : TileType.Floor;

            if (tileTypeBelow == TileType.BreakableWall || tileTypeBelow == TileType.SolidWall)
                VerticalWallTile(x, y);
            else
                VerticalWallBase(x, y);
        }
        private void OnTileDestroyed(TileDestroyedEvent e)
        {
            int x = e.Tile.X;
            int y = e.Tile.Y;

            FloorTile(x, y);
        }

        public void SetMaze(MazeData maze)
        {
            _maze = maze;
            _tilemap = new Tilemap(_tileset, _maze.Width, _maze.Height);
            _random = new Random(Guid.NewGuid().GetHashCode());

            _tilemap.Scale = new Vector2(2.625f, 2.625f);

            for (int y = 0; y < _maze.Height; y++)
            {
                for (int x = 0; x < _maze.Width; x++)
                {
                    Tile tile = _maze.GetTile(x, y);
                    var tileTypeBelow = TileType.Floor;
                    var tileTypeAbove = TileType.Floor;
                    switch (tile.Type)
                    {
                        case TileType.Floor:
                            FloorTile(x, y);
                            break;

                        case TileType.SolidWall:
                            if (y == _maze.Height - 1)
                                HorizontalWallTile(x, y);
                            else if (y == 0)
                            {
                                tileTypeBelow = _maze.GetTile(x, y+1).Type;
                                if (tileTypeBelow == TileType.BreakableWall || tileTypeBelow == TileType.SolidWall)
                                    VerticalWallTile(x, y);
                                else
                                    HorizontalWallTile(x, y);
                            }
                            else
                                VerticalWallTile(x, y);   
                            break;

                        case TileType.BreakableWall:
                            
                            tileTypeBelow = _maze.GetTile(x, y + 1).Type;
                            tileTypeAbove = _maze.GetTile(x, y - 1).Type;
                            if(tileTypeBelow == TileType.BreakableWall || tileTypeBelow  == TileType.SolidWall)
                                VerticalWallTile(x, y);
                            else
                                VerticalWallBase (x, y);
                            break;
                    }
                }
            }
        }

        public void Draw()
        {
            if (_maze == null) return;
            _tilemap.Draw(_spriteBatch);
        }
        private void FloorTile(int x, int y)
        {
            var variation = _random.Next(1, 10);
            switch (variation)
            {
                case 1:
                    _tilemap.SetTile(x, y, 6);
                    break;
                case 2:
                    _tilemap.SetTile(x, y, 7);
                    break;
                case 3:
                    _tilemap.SetTile(x, y, 8);
                    break;
                case 4:
                    _tilemap.SetTile(x, y, 11);
                    break;
                case 5:
                    _tilemap.SetTile(x, y, 12);
                    break;
                case 6:
                    _tilemap.SetTile(x, y, 13);
                    break;
                case 7:
                    _tilemap.SetTile(x, y, 16);
                    break;
                case 8:
                    _tilemap.SetTile(x, y, 17);
                    break;
                case 9:
                    _tilemap.SetTile(x, y, 18);
                    break;
                default:
                    break;
            }
        }
        private void VerticalWallTile(int x, int y)
        {
            var variation = _random.Next(1, 7);
            switch (variation)
            {
                case 1:
                    _tilemap.SetTile(x, y, 5);
                    break;
                case 2:
                    _tilemap.SetTile(x, y, 10);
                    break;
                case 3:
                    _tilemap.SetTile(x, y, 15);
                    break;
                case 4:
                    _tilemap.SetTile(x, y, 9);
                    break;
                case 5:
                    _tilemap.SetTile(x, y, 14);
                    break;
                case 6:
                    _tilemap.SetTile(x, y, 19);
                    break;
            }
        }
        private void VerticalWallBase(int x, int y)
        {
            var variation = _random.Next(1, 3);
            switch (variation)
            {
                case 1:
                    _tilemap.SetTile(x, y, 20);
                    break;
                case 2:
                    _tilemap.SetTile(x, y, 24);
                    break;
            }
        }
        private void HorizontalWallTile(int x, int y)
        {
            var variation = _random.Next(1, 7);
            switch (variation)
            {
                case 1:
                    _tilemap.SetTile(x, y, 1);
                    break;
                case 2:
                    _tilemap.SetTile(x, y, 2);
                    break;
                case 3:
                    _tilemap.SetTile(x, y, 3);
                    break;
                case 4:
                    _tilemap.SetTile(x, y, 21);
                    break;
                case 5:
                    _tilemap.SetTile(x, y, 22);
                    break;
                case 6:
                    _tilemap.SetTile(x, y, 23);
                    break;
            }
        }
    }
}
