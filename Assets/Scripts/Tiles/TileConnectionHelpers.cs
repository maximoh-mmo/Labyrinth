namespace Tiles
{
    public static class TileConnectionHelpers
    {
        public static TileConnection Rotate(this TileConnection c)
        {
            TileConnection rotated = TileConnection.None;

            if (c.HasFlag(TileConnection.Up)) rotated |= TileConnection.Right;
            if (c.HasFlag(TileConnection.Right)) rotated |= TileConnection.Down;
            if (c.HasFlag(TileConnection.Down)) rotated |= TileConnection.Left;
            if (c.HasFlag(TileConnection.Left)) rotated |= TileConnection.Up;
            return rotated;
        }
    }
}