#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sunadokei.EditorEx
{
    [InitializeOnLoad]
    internal static class InspectorButtons
    {
        static InspectorButtons()
        {
            Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        }
        private static void OnPostHeaderGUI(Editor editor)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            PingButton(editor);
            PropertiesWindowButton(editor);

            GUILayout.EndHorizontal();
        }
        private static void PingButton(Editor editor)
        {
            if (GUILayout.Button("Ping", GUILayout.Width(120)))
            {
                EditorGUIUtility.PingObject(editor.target);
            }
        }
        private static void PropertiesWindowButton(Editor editor)
        {
            if (GUILayout.Button("Properties", GUILayout.Width(120)))
            {
                EditorUtility.OpenPropertyEditor(editor.target);
            }
        }
    }
}
#endif