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
            rotationSteps = (steps % 4 + 4) % 4;
            transform.rotation = Quaternion.Euler(0, 0, rotationSteps * -90);
        }
        public bool HasConnection(TileConnection direction)
        {
            // Rotate the input direction backwards by our rotation amount
            var rotatedDir = RotateConnection(direction, -rotationSteps);
            return type.baseConnections.HasFlag(rotatedDir);
        }
        private TileConnection RotateConnection(TileConnection dir, int quarterTurns)
        {
            if (dir == TileConnection.None) return TileConnection.None;

            quarterTurns = ((quarterTurns % 4) + 4) % 4; // normalize

            for (int i = 0; i < quarterTurns; i++)
            {
                dir = dir switch
                {
                    TileConnection.Up => TileConnection.Right,
                    TileConnection.Right => TileConnection.Down,
                    TileConnection.Down => TileConnection.Left,
                    TileConnection.Left => TileConnection.Up,
                    _ => dir
                };
            }

            return dir;
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (type == null) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.9f);

            float arrowLength = 0.3f;

            // Iterate through all possible directions
            foreach (TileConnection dir in System.Enum.GetValues(typeof(TileConnection)))
            {
                if (dir == TileConnection.None) continue;
                if (!HasConnection(dir)) continue;

                // Convert logical direction into a vector
                Vector3 localDir = dir switch
                {
                    TileConnection.Up => Vector3.up,
                    TileConnection.Right => Vector3.right,
                    TileConnection.Down => Vector3.down,
                    TileConnection.Left => Vector3.left,
                    _ => Vector3.zero
                };

                // Apply the tile's current rotation
                Vector3 worldDir = transform.rotation * localDir;

                Gizmos.DrawLine(transform.position, transform.position + worldDir * arrowLength);
            }
        }
#endif
    }
}