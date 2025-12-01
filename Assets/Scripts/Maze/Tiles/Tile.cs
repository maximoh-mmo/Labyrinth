using System;

namespace Maze.Tiles
{
    public class Tile : IEquatable<Tile>
    {
        private readonly Direction _connections;
        public int Rotation;

        public Tile(Direction connections, int rotation)
        {
            _connections = connections;
            Rotation = rotation;
        }
        
        public Direction Connections()
        {
            int rotated = ((int)_connections << Rotation) & 0b1111;
            rotated |= ((int)_connections >> (4 - Rotation)) & 0b1111;
            return (Direction)rotated;
        }

        public Direction Base()
        {
            return _connections;
        }

        public bool Equals(Tile other)
        {
            if (other is null) return false;
            return _connections == other._connections && Rotation == other.Rotation;
        }

        public override bool Equals(object obj) => 
            obj is Tile other && Equals(other);

        public override int GetHashCode()
        {
            return HashCode.Combine((int)_connections, Rotation);
        }

        public override string ToString()
        {
            return Connections() + " Rotation: " + Rotation;
        }
    }

    public static class TileExtensions
    {
        public static bool HasConnection(this Tile tile, Direction dir)
        {
            return tile.Connections().HasFlag(dir);
        }
    }
}