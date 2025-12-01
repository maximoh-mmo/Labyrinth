using Maze.Generator;
using Maze.Tiles;

namespace Maze.Constraints
{
    public abstract class Constraint
    {
        public abstract bool Validate(Cell[,] grid, Tile a, Tile b, int ax, int ay, int bx, int by);
    }

    public abstract class AdjacencyConstraint : Constraint
    {
        public abstract override bool Validate(Cell[,] grid, Tile a, Tile b, int ax, int ay, int bx, int by);
    }
    
    public abstract class BoundaryConstraint : Constraint
    {
        public abstract override bool Validate(Cell[,] grid, Tile a, Tile b, int ax, int ay, int bx, int by);
    }
    
    public abstract class GlobalConstraint : Constraint
    {
        public abstract override bool Validate(Cell[,] grid, Tile a, Tile b, int ax, int ay, int bx, int by);
    }
}