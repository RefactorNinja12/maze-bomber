
using BomberRoyal.Core.Features.BoardFeature;
using BomberRoyal.Core.Features.PhysicsFeature;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace BomberRoyal.Core.Features.PathfindingFeature
{
    // A* pathfinding algoritm https://www.redblobgames.com/pathfinding/a-star/introduction.html
    public class Pathfinder
    {
        private readonly MazeData _maze;
        private readonly CollisionChecker _collisionChecker;
        private readonly int _tileSize = 42; 

        public Pathfinder(MazeData maze, CollisionChecker collisionChecker)
        {
            _maze = maze;
            _collisionChecker = collisionChecker;
        }
        // Hittar den kortaste vägen mellan två tile-positioner med hjälp av A*.
        public List<Point> FindPath(Point start, Point goal)
        {
            var openSet = new PriorityQueue<Point, float>();
            var cameFrom = new Dictionary<Point, Point>();
            //gScore = faktisk kostnad från start till denna punkt
            var gScore = new Dictionary<Point, float>();
            // fScore = total kostnad (g + h) där h = heuristik (uppskattning av avstånd till mål)
            var fScore = new Dictionary<Point, float>();

            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);
            openSet.Enqueue(start, fScore[start]);
            //Håller koll på den närmaste punkten ifall målet inte kan nås
            Point closest = start;
            float bestDist = Heuristic(start, goal);
            //fortsätter tills alla möjliga vägar testats
            while (openSet.Count > 0)
            {
                var current = openSet.Dequeue();
                // Uppdatera närmaste punkt till målet
                float currentDist = Heuristic(current, goal);
                if (currentDist < bestDist)
                {
                    bestDist = currentDist;
                    closest = current;
                }
                // Om detta är en bättre väg än tidigare, uppdatera
                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                foreach (var neighbor in GetNeighbors(current))
                {
                    float tentativeG = gScore[current] + 1;
                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);
                        openSet.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            if (closest != start)
                return ReconstructPath(cameFrom, closest);

            return new List<Point>();
        }
        // Enkel heuristik (Manhattan-avstånd) eftersom vi rör oss i fyra riktningar.
        private float Heuristic(Point a, Point b)
            => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        // Returnerar alla gångbara grann-tile-positioner inga väggar eller bomber
        private IEnumerable<Point> GetNeighbors(Point p)
        {
            Point[] dirs = new[]
            {
                new Point(1, 0), new Point(-1, 0),
                new Point(0, 1), new Point(0, -1)
            };

            foreach (var d in dirs)
            {
                int nx = p.X + d.X;
                int ny = p.Y + d.Y;

                if (nx < 0 || ny < 0 || nx >= _maze.Width || ny >= _maze.Height)
                    continue;

                var tile = _maze.GetTile(nx, ny);

                
                if (!tile.IsWalkable()) continue;

                Rectangle neighborRect = new Rectangle(nx * _tileSize, ny * _tileSize, _tileSize, _tileSize);
                if (_collisionChecker.CollidesWithBomb(neighborRect))
                    continue; 

                yield return new Point(nx, ny);
            }
        }
        // Återskapar vägen bakåt från mål till start baserat på cameFrom-listan.
        private List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            var path = new List<Point> { current };
            while (cameFrom.TryGetValue(current, out var prev))
            {
                // Säkerhet – undvik oändliga loopar om datan är korrupt
                if (path.Contains(prev)) break;
                path.Insert(0, prev);
                current = prev;
            }
            return path;
        }
    }
}