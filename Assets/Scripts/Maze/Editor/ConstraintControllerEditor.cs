using UnityEditor;
using UnityEngine;

namespace Maze.Editor
{
    [CustomEditor(typeof(ConstraintController))]
    public class ConstraintControllerEditor : UnityEditor.Editor
    {
        private ConstraintController controller;
        private readonly System.Collections.Generic.Dictionary<string, bool> foldouts = new();

        private void OnEnable()
        {
            controller = (ConstraintController)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Constraint Manager", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            foreach (var entry in controller.Constraints)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();

                // Checkbox
                bool newState = EditorGUILayout.Toggle(entry.enabled, GUILayout.Width(20));

                // Status icon + name
                DrawStatusIcon(entry.status);
                EditorGUILayout.LabelField(entry.name, EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();

                // Tooltip message
                var messageColor = GetMessageColor(entry.status);
                GUIStyle messageStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = messageColor } };
                EditorGUILayout.LabelField(entry.message, messageStyle);

                // Foldout for detailed info
                if (!foldouts.ContainsKey(entry.name))
                    foldouts[entry.name] = false;

                if (entry.details.Count > 0)
                {
                    foldouts[entry.name] = EditorGUILayout.Foldout(foldouts[entry.name], "Details", true);
                    if (foldouts[entry.name])
                    {
                        EditorGUI.indentLevel++;
                        foreach (var detail in entry.details)
                            EditorGUILayout.LabelField($"• {detail}", EditorStyles.wordWrappedMiniLabel);
                        EditorGUI.indentLevel--;
                    }
                }

                // Handle toggle change
                if (newState != entry.enabled)
                {
                    entry.enabled = newState;
                    controller.RunValidation();
                    EditorUtility.SetDirty(controller);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(4);
            }

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Validate All Constraints"))
            {
                controller.RunValidation();
                EditorUtility.SetDirty(controller);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStatusIcon(ConstraintController.ValidationStatus status)
        {
            Color color = GUI.color;
            string symbol = "";

            switch (status)
            {
                case ConstraintController.ValidationStatus.Valid:
                    GUI.color = Color.green;
                    symbol = "✔";
                    break;
                case ConstraintController.ValidationStatus.Invalid:
                    GUI.color = Color.red;
                    symbol = "✖";
                    break;
                default:
                    GUI.color = Color.gray;
                    symbol = "?";
                    break;
            }

            GUILayout.Label(symbol, EditorStyles.boldLabel, GUILayout.Width(20));
            GUI.color = color;
        }

        private Color GetMessageColor(ConstraintController.ValidationStatus status)
        {
            return status switch
            {
                ConstraintController.ValidationStatus.Valid => Color.green,
                ConstraintController.ValidationStatus.Invalid => Color.red,
                _ => Color.gray
            };
        }
    }
}
