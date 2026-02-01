using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorExtensions.Hierarchy
{
    internal static class ChildCountDrawer
    {
        static ChildCountDrawer()
        {
            _style = new(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleRight,
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Draw(in GameObject gameObj, in Rect selectionRect)
        {
            if (gameObj == null)
                return;
            OnCounter(gameObj, selectionRect);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSize(in GameObject gameObj)
        {
            if (gameObj == null)
                return 0;
            int count = gameObj.transform.childCount;
            return Mathf.CeilToInt(_style.CalcSize(new GUIContent(count.ToString())).x);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnCounter(in GameObject gameObj, in Rect rect)
        {
            int count = gameObj.transform.childCount;
            GUI.Box(rect, count.ToString(), _style);
        }

        private static readonly GUIStyle _style;
    }
}