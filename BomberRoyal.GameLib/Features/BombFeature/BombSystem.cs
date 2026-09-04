using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.PhysicsFeature;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BomberRoyal.Core.Features.BombFeature
{
    public class BombSystem : IBombSystem
    {
        public List<Bomb> _activeBombs;

        private readonly IEventBus _eventBus;
        private readonly CollisionChecker _collision;

        private float _cooldown = 1.5f;
        private float _currentCooldown = 0f;
        private int _defaultRadius = 1;
        private const int TILE_SIZE = 42;

        public float Cooldown => _cooldown;
        public int DefaultRadius => _defaultRadius;
        public float CurrentCooldown => _currentCooldown;
        public IReadOnlyList<Bomb> ActiveBombs => _activeBombs;

        public BombSystem(IEventBus eventBus, CollisionChecker collision, List<Bomb> sharedBombs)
        {
            _eventBus = eventBus;
            _collision = collision;
            _activeBombs = sharedBombs ?? new List<Bomb>();
            _eventBus.Subscribe<BombExplodedEvent>(OnBombExploded);
        }

        private void OnBombExploded(BombExplodedEvent e)
        {
            _activeBombs.Remove(e.Bomb);
        }
        public void TryPlaceBomb(Vector2 playerPosition)
        {
            if (_currentCooldown > 0)
                return;

            var maze = GameManager.Instance.Maze;
            int tx = (int)(playerPosition.X / TILE_SIZE);
            int ty = (int)(playerPosition.Y / TILE_SIZE);
            var tile = maze.GetTile(tx, ty);

            if (!tile.IsWalkable())
                return;


            Vector2 bombPos = new Vector2(
                tx * TILE_SIZE + TILE_SIZE / 2,
                ty * TILE_SIZE + TILE_SIZE / 2
            );

            var bomb = new Bomb(bombPos, _eventBus, _cooldown, _defaultRadius);
            _activeBombs.Add(bomb);
            _eventBus.Publish(new BombPlacedEvent(bomb));

            _collision.RegisterNewBomb(tx, ty);

            _currentCooldown = _cooldown;
        }

        public void Update(float delta)
        {
            if (_currentCooldown > 0)
                _currentCooldown -= delta;

            foreach (var bomb in _activeBombs.ToList())
            {
                bomb.Update(delta);
                if (bomb.Exploded)
                    _eventBus.Publish(new BombExplodedEvent(bomb));
            }
        }

        public void IncreaseRadius(int amount)
        {
            _defaultRadius += amount;
            foreach (var bomb in _activeBombs)
                bomb.Radius = _defaultRadius;
        }

        public void DecreaseCooldown(float factor)
        {
            _cooldown = MathF.Max(0.3f, _cooldown * factor);
        }
    }
}