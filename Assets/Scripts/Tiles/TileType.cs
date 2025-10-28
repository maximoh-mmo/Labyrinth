namespace Tiles
{
    using UnityEngine;
    [CreateAssetMenu(menuName = "Labyrinth/TileType")]
    public class TileType : ScriptableObject
    {
        public string displayName;
        public TileConnection baseConnections;
        public GameObject prefabVisualisation;
    }
}