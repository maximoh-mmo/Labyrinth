using System.Collections.Generic;

namespace Grid
{
    using System;
    using System.Collections;
    using Tiles;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public enum ShiftDirection
    {
        Up,
        Down,
        Left,
        Right,
        None
    }

    [Serializable]
    public struct GridPosition : IEquatable<GridPosition>
    {
        private readonly int _x;
        private readonly int _y;
        public int X => _x;
        public int Y => _y;

        public static readonly GridPosition Invalid = new GridPosition(-1, -1);

        public GridPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public static GridPosition operator +(GridPosition a, GridPosition b) =>
            new GridPosition(a._x + b._x, a._y + b._y);

        public static bool operator ==(GridPosition a, GridPosition b) =>
            a._x == b._x && a._y == b._y;

        public static bool operator !=(GridPosition a, GridPosition b) => !(a == b);

        public bool Equals(GridPosition other) => _x == other._x && _y == other._y;

        public override bool Equals(object obj) => obj is GridPosition other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_x, _y);

        public override string ToString() => $"({X}, {Y})";
    }

    public class LabyrinthGrid : MonoBehaviour
    {
        [Header("Grid Setup")] 
        [SerializeField] private int gridSize = 7;
        [SerializeField] private TileType[] tileTypes;
        private Tile[,] _tiles;
        private Tile _spareTile;
        private float _offset;
        private InputAction _shiftUp, _shiftDown, _shiftLeft, _shiftRight;
        private Coroutine _animationCoroutine;


        // Track last insertion to prevent direct reversal
        private GridPosition _lastInsertPos = GridPosition.Invalid;
        private bool _hasPreviousMove;


        private void Start()
        {
            _offset = (gridSize - 1) / 2f;
            InitializeGrid();
            _shiftUp = new InputAction("Shift Up");
            _shiftUp.AddBinding("<Keyboard>/W");
            _shiftUp.Enable();
            _shiftDown = new InputAction("Shift Down");
            _shiftDown.AddBinding("<Keyboard>/S");
            _shiftDown.Enable();
            _shiftLeft = new InputAction("Shift Left");
            _shiftLeft.AddBinding("<Keyboard>/A");
            _shiftLeft.Enable();
            _shiftRight = new InputAction("Shift Right");
            _shiftRight.AddBinding("<Keyboard>/D");
            _shiftRight.Enable();
        }

        private void OnDisable()
        {
            DisposeInputAction(_shiftUp);
            DisposeInputAction(_shiftDown);
            DisposeInputAction(_shiftLeft);
            DisposeInputAction(_shiftRight);
        }

        private void OnDestroy() => OnDisable();

        private void DisposeInputAction(InputAction action)
        {
            if (action == null) return;
            if (action.enabled) action.Disable();
            action.Dispose();
        }
        [ContextMenu("Rebuild Grid")]

        private void InitializeGrid()
        {
            if (_tiles != null)
            {
                foreach (var tile in _tiles)
                {
                    Destroy(tile.gameObject);
                    Destroy(_spareTile.gameObject);
                }
            }

            _tiles = new Tile[gridSize, gridSize];
            
            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    _tiles[x, y] = RandomTile(x, y);
                }
            }
            _spareTile = RandomTile(gridSize + 1, 0);
        }

        private Tile RandomTile(int x, int y)
        {
            var randomType = tileTypes[UnityEngine.Random.Range(0, tileTypes.Length)];
            var obj = Instantiate(randomType.prefabVisualisation);
            obj.transform.position = new Vector3(x - _offset, y - _offset, 0);
            obj.name = randomType.name;

            var tile = obj.GetComponent<Tile>();
            tile.type = randomType;
            tile.SetRotation(UnityEngine.Random.Range(0, 4));
            return tile;
        }

        private void Update()
        {
            {
                if (_shiftUp != null && _shiftUp.WasPerformedThisFrame())
                    InsertTile(new GridPosition(3, 0));

                if (_shiftDown != null && _shiftDown.WasPerformedThisFrame())
                    InsertTile(new GridPosition(3, gridSize - 1));

                if (_shiftLeft != null && _shiftLeft.WasPerformedThisFrame())
                    InsertTile(new GridPosition(0, 3));

                if (_shiftRight != null && _shiftRight.WasPerformedThisFrame())
                    InsertTile(new GridPosition(gridSize - 1, 3));
            }
        }

        /// <summary>
        /// Attempts to insert the spare tile into the given row/column, pushing from the edge of the grid.
        /// </summary>
        private void InsertTile(GridPosition entryPoint)
        {
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

            var ejectedTile = CalculateDirection(entryPoint) switch
            {
                ShiftDirection.Up => ShiftColumnUp(entryPoint.X),
                ShiftDirection.Down => ShiftColumnDown(entryPoint.X),
                ShiftDirection.Left => ShiftRowLeft(entryPoint.Y),
                ShiftDirection.Right => ShiftRowRight(entryPoint.Y),
                _ => null
            };
            _spareTile = ejectedTile;
            _lastInsertPos = entryPoint;
            _hasPreviousMove = true;
            UpdateTileTransforms();
        }
        private bool IsWithinBounds(GridPosition pos) =>
            pos.X >= 0 && pos.X < gridSize && pos.Y >= 0 && pos.Y < gridSize;

        private ShiftDirection CalculateDirection(GridPosition entryPoint)
        {
            return entryPoint.X == 0 ? ShiftDirection.Left :
                entryPoint.X == gridSize - 1 ? ShiftDirection.Right :
                entryPoint.Y == 0 ? ShiftDirection.Up :
                entryPoint.Y == gridSize - 1 ? ShiftDirection.Down : ShiftDirection.None;
        }

        private Tile ShiftRowLeft(int row)
        {
            var ejected = _tiles[0, row];
            for (var x = 0; x < gridSize - 1; x++)
                _tiles[x, row] = _tiles[x + 1, row];
            _tiles[gridSize - 1, row] = _spareTile;
            return ejected;
        }

        private Tile ShiftRowRight(int row)
        {
            var ejected = _tiles[gridSize - 1, row];
            for (var x = gridSize - 1; x > 0; x--)
                _tiles[x, row] = _tiles[x - 1, row];
            _tiles[0, row] = _spareTile;
            return ejected;
        }

        private Tile ShiftColumnUp(int col)
        {
            var ejected = _tiles[col, 0];
            for (var y = 0; y < gridSize - 1; y++)
                _tiles[col, y] = _tiles[col, y + 1];
            _tiles[col, gridSize - 1] = _spareTile;
            return ejected;
        }

        private Tile ShiftColumnDown(int col)
        {
            var ejected = _tiles[col, gridSize - 1];
            for (var y = gridSize - 1; y > 0; y--)
                _tiles[col, y] = _tiles[col, y - 1];
            _tiles[col, 0] = _spareTile;
            return ejected;
        }

        private GridPosition GetOppositePosition(GridPosition pos)
        {
            var oppositeX = pos.X == 0 ? gridSize - 1 : pos.X == gridSize - 1 ? 0 : pos.X;
            var oppositeY = pos.Y == 0 ? gridSize - 1 : pos.Y == gridSize - 1 ? 0 : pos.Y;
            return new GridPosition(oppositeX, oppositeY);
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
            var startPositions = new Dictionary<Tile, Vector3>();
            var targetPositions = new Dictionary<Tile, Vector3>();

            // Collect start/target positions for all grid tiles
            for (var x = 0; x < gridSize; x++)
            {
                for (var y = 0; y < gridSize; y++)
                {
                    var tile = _tiles[x, y];
                    if (!tile) continue;
                    startPositions[tile] = tile.transform.position;
                    // fixed axis order and z coordinate (previous version mixed axes)
                    targetPositions[tile] = new Vector3(x - _offset, y - _offset, 0);
                }
            }

            // Include spare tile in the animation (if present)
            if (_spareTile)
            {
                startPositions[_spareTile] = _spareTile.transform.position;
                targetPositions[_spareTile] = new Vector3(gridSize + 1 - _offset, 0 - _offset, 0);
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                foreach (var kvp in startPositions)
                {
                    var tile = kvp.Key;
                    if (!tile) continue;
                    var start = kvp.Value;
                    var target = targetPositions[tile];
                    tile.transform.position = Vector3.Lerp(start, target, t);
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Ensure final positions are exact
            foreach (var kvp in targetPositions)
            {
                var tile = kvp.Key;
                if (!tile) continue;
                tile.transform.position = kvp.Value;
            }

            _animationCoroutine = null;
        }

    }
    
}