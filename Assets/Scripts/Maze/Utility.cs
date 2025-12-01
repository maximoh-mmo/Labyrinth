using System;
using System.Collections.Generic;
using System.Linq;
using Maze.Generator;
using Maze.Tiles;

namespace Maze
{
    public static class Utility
    {
        public static List<Type> GetAllDerivedTypes<T>()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && typeof(T).IsAssignableFrom(type))
                .ToList();
        }
    }
}