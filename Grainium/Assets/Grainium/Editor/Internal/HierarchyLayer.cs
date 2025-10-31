using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    [InitializeOnLoad]
    internal static class HierarchyLayer
    {
        static HierarchyLayer()
        {
            //EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        }
        public static void OnGUI(GameObject gameObj, Rect selectionRect,bool isMouseContains)
        {
            if (!GrainiumSettings.GetOrCreateInstance().ShowLayerName)
            {
                return;
            }
            if (gameObj == null)
            {
                return;
            }
            //selectionRect.x = selectionRect.max.x * GrainiumSettings.GetOrCreateInstance().LayerNamePosition;


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