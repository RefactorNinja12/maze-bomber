



using Microsoft.Xna.Framework;

namespace BomberRoyal.Core.Features.BoardFeature
{
    public static class SpawnHelper
    {
        public static Vector2 FindValidSpawn(MazeData maze, int tileSize)
        {
             var candidates = new (int x, int y)[]
        {
            (1, 1),
            (maze.Width - 2, 1),
            (1, maze.Height - 2),
            (maze.Width - 2, maze.Height - 2),
        };

        foreach (var candidate in candidates)
        {
            if (maze.GetTile(candidate.x, candidate.y).IsWalkable())
            {
                return new Vector2(
                    candidate.x * tileSize + tileSize / 2,
                    candidate.y * tileSize + tileSize / 2
                );
            }
        }
        for (int y = 1; y < maze.Height - 1; y++)
        {
            for (int x = 1; x < maze.Width - 1; x++)
            {
                if (maze.GetTile(x, y).IsWalkable())
                {
                    return new Vector2(
                        x * tileSize + tileSize / 2,
                        y * tileSize + tileSize / 2
                    );
                }
            }
        }
        return new Vector2(tileSize + tileSize / 2, tileSize + tileSize / 2);
        }
    }
}
