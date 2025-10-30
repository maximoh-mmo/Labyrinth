namespace Grid
{
    using System.Collections.Generic;
    using Tiles;
    using UnityEngine;

    public class LabyrinthGeneratorAPI
    {
        public static Tile[,] Generate(int gridSize, TileType[] tileTypes, System.Func<TileType, Tile> spawnTile, params ILabyrinthConstraint[] constraints)
        {
            var generator = new LabyrinthGenerator(tileTypes, gridSize);
            foreach (var c in constraints) generator.AddConstraint(c);
            generator.AddConstraint(new Constraints.ConnectionConstraint());
            return generator.Generate(gridSize, tileTypes, spawnTile);
        }
    }
    public class LabyrinthGenerator
    {
        private List<ILabyrinthConstraint> _constraints = new();
        private readonly TileType[] _tileTypes;
        private readonly int _gridSize;

        public LabyrinthGenerator(TileType[] tileTypes, int gridSize)
        {
            _tileTypes = tileTypes;
            _gridSize = gridSize;
        }

        public void AddConstraint(ILabyrinthConstraint constraint) => _constraints.Add(constraint);
        public void RemoveConstraint(ILabyrinthConstraint constraint) => _constraints.Remove(constraint);
        public void ClearConstraints() => _constraints.Clear();
        public Tile[,] Generate(
            int gridSize, TileType[] tileTypes, System.Func<TileType, Tile> spawnTile)
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

        private (TileType, int, TileConnection) PickValidTile(
            int x, int y, TileConnection[,] logicalGrid, TileType[] types)
        {
            var valid = new List<(TileType, int, TileConnection)>();

            foreach (var type in types)
            {
                for (int rot = 0; rot < 4; rot++)
                {
                    var rotatedMask = RotateConnections(type.baseConnections, rot);
                    if (ValidateLogicalTile(logicalGrid, x, y, type, rot, rotatedMask))
                        valid.Add((type, rot, rotatedMask));
                }
            }

            return valid[Random.Range(0, valid.Count)];
        }
        
        private bool ValidateLogicalTile(TileConnection[,] grid, int x, int y, TileType type, int rotation, TileConnection mask)
        {
            foreach (var constraint in _constraints)
                if (!constraint.Validate(grid, x, y, type, rotation, mask))
                    return false;

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