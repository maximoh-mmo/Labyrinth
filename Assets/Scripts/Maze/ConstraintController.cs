using Maze.Constraints;
using Maze.Generator;

namespace Maze
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;


    [CreateAssetMenu(menuName = "Maze/Constraint Controller")]
    public class ConstraintController : ScriptableObject
    {
        public enum ValidationStatus
        {
            Unknown,
            Valid,
            Invalid
        }

        [Serializable]
        public class ConstraintEntry
        {
            public string name;
            public bool enabled;
            [HideInInspector] public Constraint Instance;
            [HideInInspector] public ValidationStatus status = ValidationStatus.Unknown;
            [HideInInspector] public string message = "";
            [HideInInspector] public List<string> details = new();
        }

        [SerializeField] private List<ConstraintEntry> constraints = new();

        public IReadOnlyList<ConstraintEntry> Constraints => constraints;

        private void OnValidate() => RefreshConstraints();
        private void OnEnable() => RefreshConstraints();

        private void RefreshConstraints()
        {
            // Find all non-abstract subclasses of Constraint
            List<Type> derivedTypes = Utility.GetAllDerivedTypes<Constraint>();
            
            // Add new constraint types if missing
            foreach (Type type in derivedTypes.Where(type => constraints.All(c => c.name != type.Name)))
            {
                try
                {
                    Constraint instance = (Constraint)Activator.CreateInstance(type);
                    constraints.Add(new ConstraintEntry
                    {
                        name = type.Name,
                        Instance = instance,
                        enabled = false
                    });
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Could not instantiate constraint '{type.Name}': {e.Message}");
                }
            }

            // Remove old ones that no longer exist
            constraints.RemoveAll(c => derivedTypes.All(t => t.Name != c.name));
        }

        /// <summary>
        /// Returns only enabled constraints.
        /// </summary>
        public IEnumerable<T> GetActiveConstraintsOfType<T>() where T : Constraint
        {
            return constraints
                .Where(c => c.enabled && c.Instance is T)
                .Select(c => c.Instance as T)
                .Where(c => c != null);
        }

        /// <summary>
        /// Runs a fake validation/compatibility check on all enabled constraints.
        /// Replace this with your real logic.
        /// </summary>
        public void RunValidation()
        {
            foreach (var entry in constraints)
            {
                entry.details.Clear();

                if (!entry.enabled)
                {
                    entry.status = ValidationStatus.Unknown;
                    entry.message = "Disabled";
                    continue;
                }

                try
                {
                    // Replace with your actual validation routine.
                    bool ok = UnityEngine.Random.value > 0.3f;
                    
                    if (ok)
                    {
                        entry.status = ValidationStatus.Valid;
                        entry.message = "Constraint passed successfully.";
                    }
                    else
                    {
                        entry.status = ValidationStatus.Invalid;
                        entry.message = "Constraint failed validation.";
                        entry.details.Add("Tile (2,3) conflicts with rotation 90°.");
                        entry.details.Add("Expected open edge to North, but got South.");
                    }
                }
                catch (Exception e)
                {
                    entry.status = ValidationStatus.Invalid;
                    entry.message = $"Error while validating: {e.Message}";
                }
            }
        }
    }
}