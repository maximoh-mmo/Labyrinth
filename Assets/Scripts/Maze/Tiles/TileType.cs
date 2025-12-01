using UnityEngine;

namespace Maze.Tiles
{
    [CreateAssetMenu(menuName = "Labyrinth/TileType")]
    public class TileType : ScriptableObject
    {
        public string displayName;
        public Direction baseConnections;
        public GameObject prefabVisualisation;
    }
}