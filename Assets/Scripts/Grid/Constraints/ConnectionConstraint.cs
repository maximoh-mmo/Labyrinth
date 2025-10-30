namespace Grid.Constraints
{
    using Tiles;
 
    public class ConnectionConstraint : ILabyrinthConstraint
    {
        public bool Validate(TileConnection[,] grid, int x, int y, TileType type, int rotation, TileConnection mask)
        {
            // Left neighbor
            if (x > 0 && grid[x - 1, y] != 0)
            {
                var left = grid[x - 1, y];
                if (HasConnection(left, TileConnection.Right) != HasConnection(mask, TileConnection.Left))
                    return false;
            }

            // Down neighbor
            if (y > 0 && grid[x, y - 1] != 0)
            {
                var down = grid[x, y - 1];
                if (HasConnection(down, TileConnection.Up) != HasConnection(mask, TileConnection.Down))
                    return false;
            }

            return true;
        }

        private static bool HasConnection(TileConnection mask, TileConnection dir)
            => (mask & dir) == dir;
    }
}