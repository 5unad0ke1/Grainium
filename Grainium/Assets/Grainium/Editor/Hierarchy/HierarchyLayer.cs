using UnityEditor;
using UnityEngine;

namespace Grainium.EditorExtensions.Hierarchy
{
    [InitializeOnLoad]
    internal static class HierarchyLayer
    {
        public static void OnGUI(GameObject gameObj, Rect selectionRect)
        {
            if (!GrainiumSettings.GetOrCreateInstance().ShowLayerName)
            {
                return;
            }
            if (gameObj == null)
            {
                return;
            }


            string name = LayerMask.LayerToName(gameObj.layer);
            var gui = EditorStyles.miniLabel;
            if (selectionRect.Contains(Event.current.mousePosition))
            {
                selectionRect.xMax = selectionRect.xMin + gui.CalcSize(new GUIContent(name)).x;
            }

            GUI.Box(selectionRect, LayerMask.LayerToName(gameObj.layer), gui);
            GUI.Box(selectionRect, string.Empty);
        }
    }
}