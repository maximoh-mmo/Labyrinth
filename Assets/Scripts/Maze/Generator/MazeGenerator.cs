using System.Linq;
using Maze.Tiles;

namespace Maze.Generator
{
    using System.Collections.Generic;
    using Constraints;
    
    public class MazeGenerator
    {
        private List<Constraint> _constraintList;
        private int _gridSize;
        private Tile[,] _grid;
        private ConstraintController _constraintController;

        public Tile[,] Grid => _grid;

        public MazeGenerator(int gridSize, ConstraintController constraintController)
        {
            _gridSize = gridSize;
            _grid = new Tile[gridSize, gridSize];
            _constraintController = constraintController;
        }

        public Tile[,] Generate()
        {
            return WFCGenerator.Generate(_gridSize, _constraintController.GetActiveConstraintsOfType<AdjacencyConstraint>().ToList());
        }
    }

}