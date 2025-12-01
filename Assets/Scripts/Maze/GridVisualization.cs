using System;
using System.Collections;
using System.Collections.Generic;
using Maze.Generator;
using Maze.Tiles;
using UnityEngine;

namespace Maze
{
    [Serializable]
   

    public class GridVisualization : MonoBehaviour
    {
        [Header("Grid Setup")] 
        [SerializeField] private int gridSize = 7;
        [SerializeField] private TileType[] tileTypes;
        private TileVisualization[,] _tiles;
        private TileVisualization _spareTileVisualization;
        private float _offset;
        private Coroutine _animationCoroutine;
        public int GridSize => gridSize;

        // Track last insertion to prevent direct reversal
        private Position _lastInsertPos = Position.Invalid;
        private bool _hasPreviousMove;


        private void Start()
        {
            _offset = (gridSize - 1) / 2f;
            InitializeGrid();
        }
        
        [ContextMenu("Rebuild Grid")]

        private void InitializeGrid()
        {
            if (_tiles != null)
            {
                foreach (TileVisualization tile in _tiles)
                {
                    Destroy(tile.gameObject);
                    Destroy(_spareTileVisualization.gameObject);
                }
            }

            _tiles = LabyrinthGeneratorAPI.Generate(
                gridSize,
                tileTypes,
                type =>
                {
                    TileVisualization tileVisualization = Instantiate(type.prefabVisualisation).GetComponent<TileVisualization>();
                    return tileVisualization;
                });
            _spareTileVisualization = RandomTile(gridSize + 1, 0);
        }

        private TileVisualization RandomTile(int x, int y)
        {
            TileType randomType = tileTypes[UnityEngine.Random.Range(0, tileTypes.Length)];
            GameObject obj = Instantiate(randomType.prefabVisualisation);
            obj.transform.position = new Vector3(x - _offset, y - _offset, 0);
            obj.name = randomType.name;

            TileVisualization tileVisualization = obj.GetComponent<TileVisualization>();
            tileVisualization.type = randomType;
            tileVisualization.SetRotation(UnityEngine.Random.Range(0, 4));
            return tileVisualization;
        }


        /// <summary>
        /// Attempts to insert the spare tile into the given row/column, pushing from the edge of the grid.
        /// </summary>
        public void InsertTile(Position entryPoint, Direction direction)
        {
            if (_animationCoroutine != null)
            {
                Debug.Log("Animation in progress. Insertion blocked.");
                return;
            }

            if (!IsWithinBounds(entryPoint))
            {
                Debug.LogWarning($"Entry point {entryPoint} is out of bounds for gridSize {gridSize}.");
                return;
            }
            
            // Prevent reverse move
            if (_hasPreviousMove && entryPoint == GetOppositePosition(_lastInsertPos))
            {
                Debug.LogWarning("Cannot reverse previous insertion directly!");
                return;
            }

            TileVisualization ejectedTileVisualization = direction switch
            {
                Direction.Up or Direction.Down => Shift(entryPoint, direction),
                Direction.Left or Direction.Right => Shift(entryPoint, direction),
                _ => null
            };
            
            _spareTileVisualization = ejectedTileVisualization;
            _lastInsertPos = entryPoint;
            _hasPreviousMove = true;
            UpdateTileTransforms();
        }
        private bool IsWithinBounds(Position pos) =>
            pos.X >= 0 && pos.X < gridSize && pos.Y >= 0 && pos.Y < gridSize;

        private TileVisualization ShiftRowLeft(int row)
        {
            var ejected = _tiles[0, row];
            for (var x = 0; x < gridSize - 1; x++)
                _tiles[x, row] = _tiles[x + 1, row];
            _tiles[gridSize - 1, row] = _spareTileVisualization;
            return ejected;
        }

        private TileVisualization ShiftRowRight(int row)
        {
            var ejected = _tiles[gridSize - 1, row];
            for (var x = gridSize - 1; x > 0; x--)
                _tiles[x, row] = _tiles[x - 1, row];
            _tiles[0, row] = _spareTileVisualization;
            return ejected;
        }

        private TileVisualization ShiftColumnUp(int col)
        {
            var ejected = _tiles[col, 0];
            for (var y = 0; y < gridSize - 1; y++)
                _tiles[col, y] = _tiles[col, y + 1];
            _tiles[col, gridSize - 1] = _spareTileVisualization;
            return ejected;
        }

        private TileVisualization ShiftColumnDown(int col)
        {
            var ejected = _tiles[col, gridSize - 1];
            for (var y = gridSize - 1; y > 0; y--)
                _tiles[col, y] = _tiles[col, y - 1];
            _tiles[col, 0] = _spareTileVisualization;
            return ejected;
        }
        private TileVisualization Shift(Position entryPoint, Direction direction)
        {
            return direction switch
            {
                Direction.Up => ShiftColumnUp(entryPoint.X),
                Direction.Down => ShiftColumnDown(entryPoint.X),
                Direction.Left => ShiftRowLeft(entryPoint.Y),
                Direction.Right => ShiftRowRight(entryPoint.Y),
                _ => null
            };
        }

        private TileVisualization GetTile(int fixedIndex, int movingIndex, bool isColumn)
        {
            return isColumn ? _tiles[fixedIndex, movingIndex] : _tiles[movingIndex, fixedIndex];
        }

        private void SetTile(int fixedIndex, int movingIndex, bool isColumn, TileVisualization tileVisualization)
        {
            if (isColumn)
                _tiles[fixedIndex, movingIndex] = tileVisualization;
            else
                _tiles[movingIndex, fixedIndex] = tileVisualization;
        }

        private Position GetOppositePosition(Position pos)
        {
            int oppositeX = pos.X == 0 ? gridSize - 1 : pos.X == gridSize - 1 ? 0 : pos.X;
            int oppositeY = pos.Y == 0 ? gridSize - 1 : pos.Y == gridSize - 1 ? 0 : pos.Y;
            return new Position(oppositeX, oppositeY);
        }
        
        private void UpdateTileTransforms(float duration = 0.5f)
        {
            // Stop any running group animation to avoid overlaps
            if (_animationCoroutine != null)
            {
                StopCoroutine(_animationCoroutine);
                _animationCoroutine = null;
            }

            // Start one coroutine that animates all grid tiles and the spare tile
            _animationCoroutine = StartCoroutine(AnimateAllTiles(duration));
        }
        
        private IEnumerator AnimateAllTiles(float duration = 0.5f)
        {
            Dictionary<TileVisualization, Vector3> startPositions = new();
            Dictionary<TileVisualization, Vector3> targetPositions = new();

            // Collect start/target positions for all grid tiles
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    TileVisualization tileVisualization = _tiles[x, y];
                    if (!tileVisualization) continue;
                    startPositions[tileVisualization] = tileVisualization.transform.position;
                    // fixed axis order and z coordinate (previous version mixed axes)
                    targetPositions[tileVisualization] = new Vector3(x - _offset, y - _offset, 0);
                }
            }

            // Include spare tile in the animation (if present)
            if (_spareTileVisualization)
            {
                startPositions[_spareTileVisualization] = _spareTileVisualization.transform.position;
                targetPositions[_spareTileVisualization] = new Vector3(gridSize + 1 - _offset, 0 - _offset, 0);
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                foreach (KeyValuePair<TileVisualization, Vector3> kvp in startPositions)
                {
                    TileVisualization tileVisualization = kvp.Key;
                    if (!tileVisualization) continue;
                    Vector3 start = kvp.Value;
                    Vector3 target = targetPositions[tileVisualization];
                    tileVisualization.transform.position = Vector3.Lerp(start, target, t);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final positions are exact
            foreach (KeyValuePair<TileVisualization, Vector3> kvp in targetPositions)
            {
                TileVisualization tileVisualization = kvp.Key;
                if (!tileVisualization) continue;
                tileVisualization.transform.position = kvp.Value;
            }

            _animationCoroutine = null;
        }

    }
    
}