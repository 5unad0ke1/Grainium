using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorExtensions.Hierarchy
{
    internal static class ActiveToggle
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(GameObject gameObj, Rect selectionRect)
        {
            if (gameObj == null)
                return;
            OnToggle(selectionRect, gameObj);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnToggle(in Rect rect, in GameObject gameObj)
        {
            bool active = gameObj.activeSelf;
            bool value = GUI.Toggle(rect, active, string.Empty);
            if (active != value)
            {
                Undo.RecordObject(gameObj, "Toggle Active");
                gameObj.SetActive(value);
                EditorUtility.SetDirty(gameObj);
            }
        }
    }
}