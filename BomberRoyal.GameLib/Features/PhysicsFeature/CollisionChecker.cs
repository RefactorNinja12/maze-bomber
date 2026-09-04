
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.BombFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BomberRoyal.Core.Features.PhysicsFeature
{
   
    // Hanterar all kollision i spelet mellan spelare, fiender, bomber och väggar.
  
    
    public class CollisionChecker
    {
        private readonly List<Bomb> _bombs;
        private readonly List<EnemyBase> _enemies;
        private readonly MazeData _maze;
        public readonly Player _player;

        private readonly HashSet<Point> _playerGhostTiles = new();
        private readonly HashSet<Point> _ignoredBombTiles = new();
        private readonly Dictionary<Point, List<EnemyBase>> _enemyGrid = new();

        private const int TILE_SIZE = 42;

        public IReadOnlyList<EnemyBase> Enemies => _enemies;

        public CollisionChecker(MazeData maze, List<Bomb> bombs, List<EnemyBase> enemies, Player player)
        {
            _maze = maze;
            _player = player;
            _bombs = bombs;
            _enemies = enemies;
        }

       
        // Tillåter spelaren att tillfälligt gå igenom en tile.
       
        public void AllowPlayerGhostThroughTile(int tx, int ty)
        {
            _playerGhostTiles.Add(new Point(tx, ty));
        }

  
        public void RegisterNewBomb(int tx, int ty)
        {
            _ignoredBombTiles.Add(new Point(tx, ty));
        }

        // Uppdaterar vilka tiles spelaren inte längre passerar (när denne lämnar dem).
      
        public void Update(float delta)
        {
            RebuildEnemyGrid();

            if (_player == null) return;

            Rectangle playerRect = _player.Hitbox;

            RemoveTilesOutside(playerRect, _playerGhostTiles);
            RemoveTilesOutside(playerRect, _ignoredBombTiles);
        }

        // Grupperar levande fiender per tile så att CollidesWithEnemy bara behöver
        // kolla grannrutorna istället för alla fiender (O(n) istället för O(n^2)).
        private void RebuildEnemyGrid()
        {
            foreach (var bucket in _enemyGrid.Values)
                bucket.Clear();

            foreach (var enemy in _enemies)
            {
                if (!enemy.IsAlive) continue;

                var tile = new Point(
                    (int)(enemy.Position.X / TILE_SIZE),
                    (int)(enemy.Position.Y / TILE_SIZE));

                if (!_enemyGrid.TryGetValue(tile, out var bucket))
                {
                    bucket = new List<EnemyBase>();
                    _enemyGrid[tile] = bucket;
                }

                bucket.Add(enemy);
            }
        }

        // Tar bort tiles från en lista när spelaren inte längre står på dem.
        private void RemoveTilesOutside(Rectangle playerRect, HashSet<Point> tileSet)
        {
            var toRemove = new List<Point>();

            foreach (var t in tileSet)
            {
                var rect = new Rectangle(t.X * TILE_SIZE, t.Y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                if (!playerRect.Intersects(rect))
                    toRemove.Add(t);
            }

            foreach (var t in toRemove)
                tileSet.Remove(t);
        }
        // Väggkollision
     
        public bool CollidesWithWall(Rectangle hitBox)
        {
            int startX = Math.Max(0, hitBox.Left / TILE_SIZE);
            int endX = Math.Min(_maze.Width - 1, hitBox.Right / TILE_SIZE);
            int startY = Math.Max(0, hitBox.Top / TILE_SIZE);
            int endY = Math.Min(_maze.Height - 1, hitBox.Bottom / TILE_SIZE);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    var tile = _maze.GetTile(x, y);
                    if (!tile.isSolid) continue;

                    var rect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    if (hitBox.Intersects(rect))
                        return true;
                }
            }
            return false;
        }

        // Väggkollision för spelare 
     
        public bool CollidesWithWall(Rectangle hitBox, Player player)
        {
            int startX = Math.Max(0, hitBox.Left / TILE_SIZE);
            int endX = Math.Min(_maze.Width - 1, hitBox.Right / TILE_SIZE);
            int startY = Math.Max(0, hitBox.Top / TILE_SIZE);
            int endY = Math.Min(_maze.Height - 1, hitBox.Bottom / TILE_SIZE);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    var tile = _maze.GetTile(x, y);
                    if (!tile.isSolid) continue;

                    // Hoppa över tiles som spelaren temporärt får passera
                    if (_playerGhostTiles.Contains(new Point(x, y)))
                        continue;

                    var rect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    if (hitBox.Intersects(rect))
                        return true;
                }
            }
            return false;
        }
     
        public bool CollidesWithBomb(Rectangle hitbox)
        {
            foreach (var bomb in _bombs)
            {
                var rect = new Rectangle(
                    (int)(bomb.Position.X - TILE_SIZE / 2f),
                    (int)(bomb.Position.Y - TILE_SIZE / 2f),
                    TILE_SIZE, TILE_SIZE
                );

                if (hitbox.Intersects(rect))
                    return true;
            }
            return false;
        }

        // Bombkollision för spelaren

        public bool CollidesWithBomb(Rectangle hitbox, Player player)
        {
            foreach (var bomb in _bombs)
            {
                var bombTile = new Point(
                    (int)(bomb.Position.X / TILE_SIZE),
                    (int)(bomb.Position.Y / TILE_SIZE)
                );

                var rect = new Rectangle(
                    (int)(bomb.Position.X - TILE_SIZE / 2f),
                    (int)(bomb.Position.Y - TILE_SIZE / 2f),
                    TILE_SIZE, TILE_SIZE
                );

                // Spelaren får gå igenom nyplacerad bomb
                if (_ignoredBombTiles.Contains(bombTile))
                    continue;

                if (hitbox.Intersects(rect))
                {
                    _ignoredBombTiles.Add(bombTile);
                    return false; 
                }
            }
            return false;
        }

        public bool CollidesWithEnemy(Rectangle hitbox, EnemyBase self)
        {
            int centerTileX = (int)(self.Position.X / TILE_SIZE);
            int centerTileY = (int)(self.Position.Y / TILE_SIZE);

            for (int y = centerTileY - 1; y <= centerTileY + 1; y++)
            {
                for (int x = centerTileX - 1; x <= centerTileX + 1; x++)
                {
                    if (!_enemyGrid.TryGetValue(new Point(x, y), out var bucket))
                        continue;

                    foreach (var enemy in bucket)
                    {
                        if (ReferenceEquals(enemy, self)) continue;

                        if (hitbox.Intersects(enemy.Hitbox))
                        {
                            ResolveEnemyPush(self, enemy);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void ResolveEnemyPush(EnemyBase self, EnemyBase other)
        {
            Vector2 pushDir = self.LastMoveDirection != Vector2.Zero
                ? -Vector2.Normalize(self.LastMoveDirection)
                : Vector2.Normalize(self.Position - other.Position);

            float dist = Vector2.Distance(self.Position, other.Position);
            float pushStrength = Math.Clamp((36f - dist) / 36f, 0f, 1f);
            Vector2 pushVector = pushDir * pushStrength * 1.5f;

            Vector2 newPos = self.Position + pushVector;
            var newRect = new Rectangle(
                (int)(newPos.X - self.Width / 2),
                (int)(newPos.Y - self.Height / 2),
                self.Width, self.Height
            );

            if (!CollidesWithWall(newRect) && !CollidesWithBomb(newRect))
                self.Position = newPos;
        }

        public bool CollidesWithPlayer(Rectangle hitbox)
        {
            if (_player == null || !_player.IsAlive)
                return false;
            return hitbox.Intersects(_player.Hitbox);
        }

        public bool IsBlocked(Rectangle hitbox, Player player)
            => CollidesWithWall(hitbox, player) || CollidesWithBomb(hitbox, player);

        public bool IsBlocked(Rectangle hitbox, EnemyBase enemy)
            => CollidesWithWall(hitbox) || CollidesWithBomb(hitbox) || CollidesWithEnemy(hitbox, enemy);

        public bool IsBlocked(Rectangle hitbox)
            => CollidesWithWall(hitbox) || CollidesWithBomb(hitbox) || CollidesWithPlayer(hitbox);

        // Explosionsträffar

        public (bool playerHit, List<EnemyBase> enemiesHit) CheckExplosionHits(Rectangle area)
        {
            bool playerHit = _player.IsAlive && area.Intersects(_player.Hitbox);
            var enemiesHit = _enemies
                .Where(e => e.IsAlive && area.Intersects(e.Hitbox))
                .ToList();

            return (playerHit, enemiesHit);
        }
    }
}