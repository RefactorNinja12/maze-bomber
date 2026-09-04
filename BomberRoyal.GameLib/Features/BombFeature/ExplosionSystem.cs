using BomberRoyal.Core.Event;
using BomberRoyal.Core.Features.Board;
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using Microsoft.Xna.Framework;
using System;


namespace BomberRoyal.Core.Features.BombFeature
{
    public class ExplosionSystem
    {
        private readonly IEventBus _eventBus;
        private readonly MazeData _maze;
        private readonly CollisionChecker _collision;

        public ExplosionSystem(MazeData maze, CollisionChecker collision, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _maze = maze;
            _collision = collision;
            _eventBus.Subscribe<BombExplodedEvent>(OnBombExploded);
            _eventBus = eventBus;
        }

        private void OnBombExploded(BombExplodedEvent e)
        {
            var bomb = e.Bomb;
            Explode(bomb);
        }
        private void Explode(Bomb bomb)
        {
            int tileSize = 42; 
            int bombX = (int)(bomb.Position.X / tileSize);
            int bombY = (int)(bomb.Position.Y / tileSize);
            ApplyDamage(bombX, bombY);
            SpreadExplosion(bombX, bombY, 1, 0, bomb.Radius);
            SpreadExplosion(bombX, bombY, -1, 0, bomb.Radius);
            SpreadExplosion(bombX, bombY, 0, 1, bomb.Radius);
            SpreadExplosion(bombX, bombY, 0, -1, bomb.Radius);
        }
        private void SpreadExplosion(int startX, int startY, int dx, int dy, int radius)
        {
            for(int i = 1; i <= radius; i++)
            {
                int x = startX + dx * i; 
                int y = startY + dy * i;

                if(x < 0 || y < 0 || x >= _maze.Width || y >= _maze.Height)
                {
                    break;
                }
                var tile = _maze.GetTile(x, y);
                if(tile.Type == TileType.SolidWall)
                {
                    break;
                }
                if(tile.Type == BoardFeature.TileType.BreakableWall)
                {
                    _maze.SetTile(x, y, new Tile(x, y, 42, TileType.Floor, _eventBus));
                    _eventBus.Publish(new TileDestroyedEvent(tile));
                    break;
                }


                ApplyDamage(x, y);
            }
        }
        private void ApplyDamage(int tileX, int tileY)
        {
            int tileSize = 42;
            Rectangle explosionRect = new(
                tileX * tileSize,
                tileY * tileSize,
                tileSize,
                tileSize
            );

           
            var (playerHit, enemiesHit) = _collision.CheckExplosionHits(explosionRect);

            if (playerHit)
            {
                
               
                _eventBus.Publish(new PlayerDiedEvent(_collision._player));
            }

            foreach (var enemy in enemiesHit)
            {
                
                enemy.Kill();
                _eventBus.Publish(new EnemyDiedEvent(enemy));
            }
        }
    }
}
