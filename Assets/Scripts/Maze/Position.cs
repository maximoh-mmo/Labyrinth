using System;
using JetBrains.Annotations;

namespace Maze
{
    public struct Position : IEquatable<Position>
    {
        private readonly int _x;
        private readonly int _y;
        public int X => _x;
        public int Y => _y;

        public static readonly Position Invalid = new Position(-1, -1);

        public Position(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public static Position operator +(Position a, Position b) =>
            new Position(a._x + b._x, a._y + b._y);

        public static bool operator ==(Position a, Position b) =>
            a._x == b._x && a._y == b._y;

        public static bool operator !=(Position a, Position b) => !(a == b);

        public bool Equals(Position other) => _x == other._x && _y == other._y;

        public override bool Equals(object obj) => obj is Position other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(_x, _y);

        public override string ToString() => $"({X}, {Y})";
    }
    
}