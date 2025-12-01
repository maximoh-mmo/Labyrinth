using Maze;

namespace Game
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class Controller : MonoBehaviour
    {
        private InputAction _shiftUp, _shiftDown, _shiftLeft, _shiftRight;
        private GridVisualization _gridVisualization;

        private void Start()
        {
            _gridVisualization = GetComponent<GridVisualization>();
        }

        private void OnEnable()
        {
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
        
        private void DisposeInputAction(InputAction action)
        {
            if (action == null) return;
            if (action.enabled) action.Disable();
            action.Dispose();
        }
        
        private void OnDisable()
        {
            DisposeInputAction(_shiftUp);
            DisposeInputAction(_shiftDown);
            DisposeInputAction(_shiftLeft);
            DisposeInputAction(_shiftRight);
        }
        private void OnDestroy() => OnDisable();
        
        private void Update()
        {
            if (!_gridVisualization) return;
            {
                if (_shiftUp != null && _shiftUp.WasPerformedThisFrame())
                    _gridVisualization.InsertTile(new Position(3, _gridVisualization.GridSize - 1), Direction.Up);

                if (_shiftDown != null && _shiftDown.WasPerformedThisFrame())
                    _gridVisualization.InsertTile(new Position(3, 0), Direction.Down);

                if (_shiftLeft != null && _shiftLeft.WasPerformedThisFrame())
                    _gridVisualization.InsertTile(new Position(0, 3), Direction.Left);

                if (_shiftRight != null && _shiftRight.WasPerformedThisFrame())
                    _gridVisualization.InsertTile(new Position(_gridVisualization.GridSize - 1, 3), Direction.Right);
            }
            
        }
    }
}