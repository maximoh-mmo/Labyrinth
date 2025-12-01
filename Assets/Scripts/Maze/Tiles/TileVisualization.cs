using UnityEngine;

namespace Maze.Tiles
{
    public class TileVisualization : MonoBehaviour
    {
        public TileType type;
        private Tile _tile;

        public TileVisualization(TileType type, int rotationSteps)
        {
            this.type = type;
            _tile = new Tile(type.baseConnections, rotationSteps);
        }

        public void SetRotation(int steps)
        {
            _tile = new Tile(type.baseConnections, steps);
            transform.rotation = Quaternion.Euler(0, 0, _tile.Rotation * -90);
        }
        
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (type == null) return;

            Gizmos.color = Color.green;
            float size = 0.4f;
            Vector3 pos = transform.position;

            Direction conns = _tile.Connections(); // Use rotated directions

            if (conns.HasFlag(Direction.Up))
                Gizmos.DrawLine(pos, pos + Vector3.up * size);
            if (conns.HasFlag(Direction.Right))
                Gizmos.DrawLine(pos, pos + Vector3.right * size);
            if (conns.HasFlag(Direction.Down))
                Gizmos.DrawLine(pos, pos + Vector3.down * size);
            if (conns.HasFlag(Direction.Left))
                Gizmos.DrawLine(pos, pos + Vector3.left * size);
        }
#endif
    }
}