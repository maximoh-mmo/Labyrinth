using Maze.Generator;
using Maze.Tiles;
using UnityEditor;
using UnityEngine;

namespace Maze
{
    public static class LabyrinthGeneratorAPI
    {
        public static TileVisualization[,] Generate(int gridSize, TileType[] tileTypes, System.Func<TileType, TileVisualization> spawnTile)
        {
            LabyrinthGenerator generator = new LabyrinthGenerator(tileTypes, gridSize);
            return generator.Generate(gridSize, tileTypes, spawnTile);
        }
    }
    public class LabyrinthGenerator
    {
        private readonly TileType[] _tileTypes;
        private readonly int _gridSize;
        private readonly Tile[,] _tiles; 

        public LabyrinthGenerator(TileType[] tileTypes, int gridSize)
        {
            _tileTypes = tileTypes;
            _gridSize = gridSize;
            
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets("t:ConstraintController");
            if (guids.Length <= 0)
            {
                Debug.Log("Constraint controller not found.");
                return;
            }
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            MazeGenerator generator = new MazeGenerator(gridSize, AssetDatabase.LoadAssetAtPath<ConstraintController>(path));
            _tiles = generator.Generate();
#endif
        }

        public TileVisualization[,] Generate(
            int gridSize, TileType[] tileTypes, System.Func<TileType, TileVisualization> spawnTile)
        {
            TileVisualization[,] grid = new TileVisualization[gridSize, gridSize];
            float offset = (gridSize - 1) / 2f;
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    (TileType type, int rotation)? type = PickValidTile(_tiles[x, y], tileTypes);
                    if (!type.HasValue)
                    {
                        Debug.LogFormat("No valid tile found at {0}, {1}", x, y);
                        continue;
                    }
                    TileVisualization tileVisualization = spawnTile(type.Value.type);
                    tileVisualization.type = type.Value.type;
                    tileVisualization.name = $"{type.Value.type.displayName}_{x}_{y}";
                    tileVisualization.SetRotation(type.Value.rotation);
                    tileVisualization.transform.position = new Vector3(x - offset, y - offset, 0);
                    grid[x, y] = tileVisualization;
                }
            }
            return grid;
        }

        private (TileType, int)? PickValidTile(Tile type, TileType[] tileTypes)
        {
            foreach (TileType tileType in tileTypes)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (type.Base().Equals(tileType.baseConnections.Rotate(i)))
                    {
                        return (tileType, type.Rotation);
                    }   
                }
            }
            return null;
        }
    }
}