using System.Linq;
using Maze.Constraints;
using Maze.Tiles;

namespace Maze.Generator
{
  using System.Collections.Generic;

    public static class ArcCollapse3
    {
        private static readonly (int dx, int dy, Direction dir, Direction opp)[] Neighbors =
        {
            (1, 0, Direction.Right, Direction.Left),
            (-1, 0, Direction.Left, Direction.Right),
            (0, 1, Direction.Up, Direction.Down),
            (0, -1, Direction.Down, Direction.Up)
        };

        public static bool EnforceArcConsistency(Cell[,] grid, List<AdjacencyConstraint> constraints)
        {
            int size = grid.GetLength(0);
            Queue<(int x, int y, int nx, int ny)> queue = new Queue<(int x, int y, int nx, int ny)>();

            // Initialize all arcs
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    foreach ((int dx, int dy, Direction dir, Direction opp) in Neighbors)
                    {
                        int nx = x + dx, ny = y + dy;
                        if (nx >= 0 && ny >= 0 && nx < size && ny < size)
                            queue.Enqueue((x, y, nx, ny));
                    }
                }
            }

            while (queue.Count > 0)
            {
                (int x, int y, int nx, int ny) = queue.Dequeue();
                if (!Revise(grid, x, y, nx, ny, constraints))
                {
                    continue;
                }
                if (grid[x, y].Candidates.Count == 0)
                {
                    return false;
                }

                foreach ((int dx, int dy, Direction dir, Direction opp) in Neighbors)
                {
                    int px = x + dx, py = y + dy;
                    if (px >= 0 && py >= 0 && px < size && py < size)
                    {
                        queue.Enqueue((px, py, x, y));
                    }
                }
            }

            return true;
        }

        private static bool Revise(Cell[,] grid, int x, int y, int nx, int ny, List<AdjacencyConstraint> constraints)
        {
            bool revised = false;

            Cell cell = grid[x, y];
            Cell neighbor = grid[nx, ny];

            for (int i = cell.Candidates.Count - 1; i >= 0; i--)
            {
                Tile a = cell.Candidates[i];
                bool supported =
                    neighbor.Candidates.Any(b => constraints.All(c => c.Validate(grid, a, b, x, y, nx, ny)));
                if (supported)
                {
                    continue;
                }
                cell.Candidates.RemoveAt(i);
                revised = true;
            }

            return revised;
        }
    }
}