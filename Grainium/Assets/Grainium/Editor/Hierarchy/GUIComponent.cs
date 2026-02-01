#if UNITY_EDITOR
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorExtensions.Hierarchy
{
    [InitializeOnLoad]
    internal static class GUIComponent
    {
        public static void OnGUI(in GameObject gameObj, in Rect selectionRect, in bool isMouseContains)
        {
            if (gameObj == null)
            {
                return;
            }

            var components = gameObj.GetComponents<Component>();
            if (components.Length == 0)
            {
                return;
            }
            OnComponentIcons(selectionRect, components, isMouseContains);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OnComponentIcons(Rect selectionRect, in Component[] components, in bool isMouseContains)
        {
            Rect boxRect = selectionRect;
            boxRect.width = ICON_SIZE;
            boxRect.height = ICON_SIZE;

            int max = (int)((selectionRect.xMax - selectionRect.xMin) / ICON_SIZE);
            int length = isMouseContains ? components.Length : Mathf.Min(components.Length, max + 1);

            int count = length - 1;
            boxRect.x = selectionRect.xMax - ICON_SIZE * count;

            bool isOverflow = length < components.Length;
            for (int i = 0; i < length; i++)
            {
                if (components[i] is Transform)
                {
                    selectionRect.x += ICON_SIZE;
                    continue;
                }
                if (isOverflow && i == length - 1)
                {

                    break;
                }

                var texture2D = AssetPreview.GetMiniThumbnail(components[i]);

                GUI.DrawTexture(boxRect, texture2D);
                boxRect.x += ICON_SIZE;
            }
            if (isOverflow && length > 1)
            {
                boxRect.x = selectionRect.xMax - ICON_SIZE * 2;
                GUI.Box(boxRect, "~", EditorStyles.label);
            }
        }

        private const int ICON_SIZE = 14;
    }
}
#endif