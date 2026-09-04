using BomberRoyal.Core.Enums;
using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.Board;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using System;

namespace BomberRoyal.Core.Features.AbilityFeature
{
    public class PlaceTileAbility
    {
        private readonly Player _player;
        private readonly MazeData _maze;
        private readonly IEventBus _eventBus;
        private readonly CollisionChecker _collision;
        private readonly int _tileSize;
        private readonly float _cooldown;
        private float _currentCooldown;

        public bool IsReady => _currentCooldown <= 0;

        public PlaceTileAbility(Player player, MazeData maze, IEventBus eventBus,
            CollisionChecker collision, int tileSize = 42, float cooldown = 5f)
        {
            _player = player;
            _maze = maze;
            _eventBus = eventBus;
            _collision = collision;
            _tileSize = tileSize;
            _cooldown = cooldown;
        }

        public void Update(float delta)
        {
            if (_currentCooldown > 0)
                _currentCooldown -= delta;
        }

        // Logik för att försöka placera en tile framför spelaren
        public void TryPlaceTile()
        {
            if (!IsReady) return;

            Vector2 dir = GetFacingVector(_player.Direction);
            int tx = (int)((_player.Position.X + dir.X * _tileSize) / _tileSize);
            int ty = (int)((_player.Position.Y + dir.Y * _tileSize) / _tileSize);

            if (tx < 0 || ty < 0 || tx >= _maze.Width || ty >= _maze.Height) return;

            var target = _maze.GetTile(tx, ty);
            if (!target.IsWalkable()) return;

            var newTile = new Tile(tx, ty, _tileSize, TileType.BreakableWall, _eventBus);
            _maze.SetTile(tx, ty, newTile);
            _eventBus.Publish(new TilePlacedEvent(newTile));

            _collision.AllowPlayerGhostThroughTile(tx, ty);

            _currentCooldown = _cooldown;
        }

        private static Vector2 GetFacingVector(Direction dir) => dir switch
        {
            Direction.Up => new Vector2(0, -1),
            Direction.Down => new Vector2(0, 1),
            Direction.Left => new Vector2(-1, 0),
            Direction.Right => new Vector2(1, 0),
            _ => Vector2.UnitY
        };
    }
}