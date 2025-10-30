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
        
        public TileConnection GetRotatedConnections()
        {
            int rotated = ((int)type.baseConnections << rotationSteps) & 0b1111;
            rotated |= ((int)type.baseConnections >> (4 - rotationSteps)) & 0b1111;
            return (TileConnection)rotated;
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (type == null) return;

            Gizmos.color = Color.green;
            float size = 0.4f;
            var pos = transform.position;

            var conns = GetRotatedConnections(); // Use rotated directions

            if (conns.HasFlag(TileConnection.Up))
                Gizmos.DrawLine(pos, pos + Vector3.up * size);
            if (conns.HasFlag(TileConnection.Right))
                Gizmos.DrawLine(pos, pos + Vector3.right * size);
            if (conns.HasFlag(TileConnection.Down))
                Gizmos.DrawLine(pos, pos + Vector3.down * size);
            if (conns.HasFlag(TileConnection.Left))
                Gizmos.DrawLine(pos, pos + Vector3.left * size);
        }
#endif
    }
}