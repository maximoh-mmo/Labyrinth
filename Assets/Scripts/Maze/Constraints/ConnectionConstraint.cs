
using Maze.Generator;
using Maze.Tiles;

namespace Maze.Constraints
{
    public class ConnectionConstraint : Constraint
    {
        private static bool HasConnection(Direction mask, Direction dir)
            => (mask & dir) == dir;

        public override bool Validate(Cell[,] grid, Tile a, Tile b, int ax, int ay, int bx, int by)
        {
            Direction dir = BFromA(ax, ay, bx, by);
            return a.HasConnection(dir) && b.HasConnection(dir.Opposite());
        }

        private static Direction BFromA(int ax, int ay, int bx, int by)
        {
            var x = bx - ax;
            var y = by - ay;
            return x == 1 ? Direction.Right :
                x == -1 ? Direction.Left :
                y == 1 ? Direction.Up :
                y == -1 ? Direction.Down : Direction.None;
        }
    }
    }