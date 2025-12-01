using System.Collections.Generic;
using Maze.Tiles;

namespace Maze.Generator
{
    public class Cell
    {
        public List<Tile> Candidates; // Possible tiles
        public bool IsCollapsed => Candidates.Count == 1;

        public Tile Value => IsCollapsed ? Candidates[0] : null;

        public Cell(IEnumerable<Tile> possible)
        {
            Candidates = new List<Tile>(possible);
        } 
    }
}