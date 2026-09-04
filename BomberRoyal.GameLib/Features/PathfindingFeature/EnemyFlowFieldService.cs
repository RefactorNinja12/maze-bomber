
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.BombFeature;
using BomberRoyal.Core.Features.PlayerFeature;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BomberRoyal.Core.Features.PathfindingFeature
{
    // Delad "flow field"-pathfinding: en enda BFS från spelarens tile per intervall,
    // återanvänd av alla fiender istället för att varje fiende söker sin egen väg.
    public class EnemyFlowFieldService
    {
        private static readonly Point[] Directions =
        {
            new Point(1, 0), new Point(-1, 0),
            new Point(0, 1), new Point(0, -1)
        };

        private readonly MazeData _maze;
        private readonly List<Bomb> _bombs;
        private readonly Player _player;
        private readonly int _tileSize = 42;

        private readonly Dictionary<Point, Point> _cameFrom = new();
        private Point _goalTile;

        private float _rebuildTimer = 0f;
        private const float RebuildInterval = 1.0f;

        public EnemyFlowFieldService(MazeData maze, List<Bomb> bombs, Player player)
        {
            _maze = maze;
            _bombs = bombs;
            _player = player;
        }

        public void Update(float deltaTime)
        {
            _rebuildTimer -= deltaTime;
            if (_rebuildTimer > 0f)
                return;

            _rebuildTimer = RebuildInterval;

            if (_player == null)
                return;

            Point goal = new(
                (int)(_player.Position.X / _tileSize),
                (int)(_player.Position.Y / _tileSize));

            RebuildFromGoal(goal);
        }

        private void RebuildFromGoal(Point goal)
        {
            _goalTile = goal;
            _cameFrom.Clear();

            var bombTiles = new HashSet<Point>();
            foreach (var bomb in _bombs)
            {
                bombTiles.Add(new Point(
                    (int)(bomb.Position.X / _tileSize),
                    (int)(bomb.Position.Y / _tileSize)));
            }

            var queue = new Queue<Point>();
            queue.Enqueue(goal);
            _cameFrom[goal] = goal;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                foreach (var d in Directions)
                {
                    var neighbor = new Point(current.X + d.X, current.Y + d.Y);

                    if (neighbor.X < 0 || neighbor.Y < 0 || neighbor.X >= _maze.Width || neighbor.Y >= _maze.Height)
                        continue;

                    if (_cameFrom.ContainsKey(neighbor))
                        continue;

                    if (!_maze.GetTile(neighbor.X, neighbor.Y).IsWalkable())
                        continue;

                    if (bombTiles.Contains(neighbor))
                        continue;

                    _cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Följer den förberäknade vägen mot målet, från start till mål.
        public List<Point> GetPathFrom(Point start)
        {
            var path = new List<Point>();

            if (!_cameFrom.ContainsKey(start))
                return path;

            var current = start;
            path.Add(current);

            while (current != _goalTile)
            {
                current = _cameFrom[current];
                path.Add(current);
            }

            return path;
        }
    }
}
