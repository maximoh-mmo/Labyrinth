using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Tiles
{
    public class TileGridTest : MonoBehaviour
    {
        public int gridSize = 7;
        public TileType[] tileTypes;
        private float _offset;
        private InputAction _shuffle;
        void Start()
        {
            _offset = gridSize / 2f;
            InitGrid();
            _shuffle = new InputAction("Shuffle");
            _shuffle.AddBinding("<Keyboard>/Space");
            _shuffle.Enable();
        }

        private void Update()
        {
            if (_shuffle.WasPerformedThisFrame())
            {
                InitializeGrid();
            }
        }

        private void InitializeGrid()
        {
            DestroyGrid();
            InitGrid();
        }

        private void InitGrid()
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    TileType randomType = tileTypes[Random.Range(0, tileTypes.Length)];
                    GameObject obj = Instantiate(randomType.prefabVisualisation);
                    obj.transform.position = new Vector3(x - _offset, y - _offset, 0);
                    obj.name = randomType.name;

                    Tile tile = obj.GetComponent<Tile>();
                    tile.type = randomType;
                    tile.SetRotation(Random.Range(0, 4));
                }
            }
        }

        private void DestroyGrid()
        {
            var objects = FindObjectsByType<MeshFilter>(0);
            foreach (var obj in objects)
            {
                Destroy(obj.gameObject);
            }
        }
    }
}