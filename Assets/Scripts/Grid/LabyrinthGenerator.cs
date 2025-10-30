using System.Collections.Generic;

namespace Grid
{
    using Tiles;
    using UnityEngine;

    public class LabyrinthGenerator
    {
        public static Tile[,] Generate(
            int gridSize, TileType[] tileTypes, Transform parent, System.Func<TileType, Tile> spawnTile)
        {
            var grid = new Tile[gridSize, gridSize];
            var logicalGrid = new TileConnection[gridSize, gridSize]; // just store logical connections
            var offset = (gridSize - 1) / 2f;
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    var (type, rotation, connectionMask) = PickValidTile(x, y, logicalGrid, tileTypes);

                    logicalGrid[x, y] = connectionMask;

                    var tile = spawnTile(type);
                    tile.type = type;
                    tile.name = $"{type.name}_{x}_{y}";
                    tile.SetRotation(rotation);
                    tile.transform.position = new Vector3(x - offset, y - offset, 0);
                    grid[x, y] = tile;
                }
            }

            return grid;
        }

        private static (TileType, int, TileConnection) PickValidTile(
            int x, int y, TileConnection[,] logicalGrid, TileType[] types)
        {
            var valid = new List<(TileType, int, TileConnection)>();

            foreach (var type in types)
            {
                for (int rot = 0; rot < 4; rot++)
                {
                    var rotatedMask = RotateConnections(type.baseConnections, rot);
                    if (IsValid(x, y, logicalGrid, rotatedMask))
                        valid.Add((type, rot, rotatedMask));
                }
            }

            return valid[Random.Range(0, valid.Count)];
        }

        private static bool IsValid(int x, int y, TileConnection[,] grid, TileConnection candidate)
        {
            if (x > 0 && grid[x - 1, y] != 0)
            {
                var left = grid[x - 1, y];
                if (HasConnection(left, TileConnection.Right) != HasConnection(candidate, TileConnection.Left))
                    return false;
            }

            if (y > 0 && grid[x, y - 1] != 0)
            {
                var down = grid[x, y - 1];
                if (HasConnection(down, TileConnection.Up) != HasConnection(candidate, TileConnection.Down))
                    return false;
            }

            return true;
        }

        private static TileConnection RotateConnections(TileConnection mask, int steps)
        {
            // rotate bitflags
            return (TileConnection)(((int)mask << steps | (int)mask >> (4 - steps)) & 0b1111);
        }

        private static bool HasConnection(TileConnection mask, TileConnection dir)
            => (mask & dir) == dir;
    }
}