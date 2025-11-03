using System;
using Grid;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class Controller : MonoBehaviour
    {
        private InputAction _shiftUp, _shiftDown, _shiftLeft, _shiftRight;
        private LabyrinthGrid _grid;

        private void Start()
        {
            _grid = GetComponent<LabyrinthGrid>();
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
            if (!_grid) return;
            {
                if (_shiftUp != null && _shiftUp.WasPerformedThisFrame())
                    _grid.InsertTile(new GridPosition(3, _grid.GridSize - 1), ShiftDirection.Up);

                if (_shiftDown != null && _shiftDown.WasPerformedThisFrame())
                    _grid.InsertTile(new GridPosition(3, 0), ShiftDirection.Down);

                if (_shiftLeft != null && _shiftLeft.WasPerformedThisFrame())
                    _grid.InsertTile(new GridPosition(0, 3), ShiftDirection.Left);

                if (_shiftRight != null && _shiftRight.WasPerformedThisFrame())
                    _grid.InsertTile(new GridPosition(_grid.GridSize - 1, 3), ShiftDirection.Right);
            }
            
        }
    }
}