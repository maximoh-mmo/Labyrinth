namespace Tiles
{
    using UnityEngine;
    public class Tile : MonoBehaviour
    {
        public TileType type;
        public int rotationSteps; // 0–3, each step = 90° CW

        public TileConnection Connections
        {
            get
            {
                TileConnection c = type.baseConnections;
                for (int i = 0; i < rotationSteps; i++)
                    c = c.Rotate();
                return c;
            }
        }

        public void SetRotation(int steps)
        {
            rotationSteps = steps % 4;
            transform.rotation = Quaternion.Euler(0, 0, rotationSteps * 90);
        }
    }
}