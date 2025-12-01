using System;
using System.Collections.Generic;
using Maze.Constraints;
using Maze.Tiles;

namespace Maze.Generator
{
    public static class WFCGenerator
    {
        private static readonly Tile[] _patterns = new[]
        {
            new Tile(Direction.Up, 0),
            new Tile(Direction.Up, 1),
            new Tile(Direction.Up, 2),
            new Tile(Direction.Up, 3),
            new Tile(Direction.Right | Direction.Up, 0),
            new Tile(Direction.Right | Direction.Up, 1),
            new Tile(Direction.Right | Direction.Up, 2),
            new Tile(Direction.Right | Direction.Up, 3),
            new Tile(Direction.Right | Direction.Left, 0),
            new Tile(Direction.Right | Direction.Left, 1),
            new Tile(Direction.Right | Direction.Left, 2),
            new Tile(Direction.Right | Direction.Left, 3),
            new Tile(Direction.Right | Direction.Up | Direction.Left, 0),
            new Tile(Direction.Right | Direction.Up | Direction.Left, 1),
            new Tile(Direction.Right | Direction.Up | Direction.Left, 2),
            new Tile(Direction.Right | Direction.Up | Direction.Left, 3),
            new Tile(Direction.Right | Direction.Up | Direction.Left | Direction.Down, 0),
            new Tile(Direction.Right | Direction.Up | Direction.Left | Direction.Down, 1),
            new Tile(Direction.Right | Direction.Up | Direction.Left | Direction.Down, 2),
            new Tile(Direction.Right | Direction.Up | Direction.Left | Direction.Down, 3)
        };
        private static readonly System.Random Rand = new();

        public static Tile[,] Generate(int size, List<AdjacencyConstraint> constraints)
        {
            var grid = new Cell[size, size];
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                grid[x, y] = new Cell(_patterns);

            while (true)
            {
                // Find lowest entropy cell (smallest domain > 1)
                (int x, int y)? choice = null;
                int minEntropy = int.MaxValue;

                for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    int count = grid[x, y].Candidates.Count;
                    if (count > 1 && count < minEntropy)
                    {
                        minEntropy = count;
                        choice = (x, y);
                    }
                }

                if (choice == null) break; // Done, all collapsed

                var (cx, cy) = choice.Value;
                var cell = grid[cx, cy];
                var value = cell.Candidates[Rand.Next(cell.Candidates.Count)];
                cell.Candidates = new List<Tile> { value };
                
                ArcCollapse3.EnforceArcConsistency(grid, constraints);

            }

            // Convert to final map
            var result = new Tile[size, size];
            for (int x = 0; x < size; x++)
            for (int y = 0; y < size; y++)
                if (grid[x, y].Value == null)
                {
                    Console.WriteLine("null value returned from wfc");
                }
                else result[x, y] = grid[x, y].Value;

            return result;
        }
    }

}