namespace Grid
{
    using Tiles;
    public interface ILabyrinthConstraint
    {
        bool Validate(TileConnection[,] grid, int x, int y, TileType type, int rotation, TileConnection mask);
    }
}