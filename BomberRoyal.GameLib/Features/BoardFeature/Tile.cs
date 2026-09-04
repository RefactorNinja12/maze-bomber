using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.BoardFeature;


namespace BomberRoyal.Core.Features.Board
{
    public class Tile
    {
        private readonly IEventBus _eventBus;
        public int Size { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public TileType Type { get; private set; }
        public int Health { get; private set; }

        public Tile(int x, int y, int size, TileType type, IEventBus eventBus)
        {
            _eventBus = eventBus;
            X = x;
            Y = y;
            Size = size;
            Type = type;
            Health = 100;
            _eventBus = eventBus;
        }

        public void TakeDamage(int amount)
        {
            if (!IsDestructible() || Health <= 0) return;

            Health -= amount;

            if (Health <= 0)
            {
                GameManager.Instance.Maze.SetTile(X, Y, new Tile(X, Y, Size, TileType.Floor, _eventBus));
                _eventBus.Publish(new TileDestroyedEvent(this));
            }
        }
        public bool IsDestructible() => Type == TileType.BreakableWall;

        public bool IsWalkable() => Type == TileType.Floor;

        public bool isSolid => Type == TileType.SolidWall || Type == TileType.BreakableWall;
    }
}
