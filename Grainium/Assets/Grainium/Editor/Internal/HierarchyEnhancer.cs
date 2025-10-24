using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    [InitializeOnLoad]
    internal static class HierarchyEnhancer
    {
        static HierarchyEnhancer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        }
        private static void OnGUI(int instanceID, Rect selectionRect)
        {
            Rect boxRect = new(selectionRect);
            boxRect.xMin = boxRect.height * 4;

            bool showComponentIcons = GrainiumSettings.GetOrCreateInstance().ShowComponentIcons;
            bool showActiveToggles = GrainiumSettings.GetOrCreateInstance().ShowActiveToggles;
            bool showLayerName = GrainiumSettings.GetOrCreateInstance().ShowLayerName;
            if (!showComponentIcons && !showActiveToggles && !showLayerName)
                return;

            var gameObj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObj == null)
            {
                return;
            }
            selectionRect.xMin += selectionRect.height * 2;
            selectionRect.xMax += selectionRect.height;
            selectionRect.x = selectionRect.xMin;

            float layerOffset = GrainiumSettings.GetOrCreateInstance().LayerNamePosition;
            float layerRatio = 0.2f;

            float textX = selectionRect.xMin + EditorStyles.label.CalcSize(new GUIContent(gameObj.name)).x - selectionRect.height;

            Rect layerRect = new(selectionRect);
            Rect componentsRect = new(selectionRect);

            layerRect.xMin = Mathf.Lerp(boxRect.xMin, boxRect.xMax, layerOffset);
            layerRect.xMax = Mathf.Lerp(boxRect.xMin, boxRect.xMax, layerOffset + layerRatio);
            float range = layerRect.xMax - layerRect.xMin;
            if (layerRect.xMin < textX)
            {
                layerRect.xMin = textX;
                layerRect.xMax = layerRect.xMin + range;
            }
            layerRect.x = layerRect.xMin;

            componentsRect.xMin = showLayerName ? layerRect.xMax : textX;
            componentsRect.xMax = selectionRect.xMax;
            componentsRect.x = componentsRect.xMin;

            bool isMouseContainsLayer = layerRect.Contains(Event.current.mousePosition);
            bool isMouseContainsCompo = componentsRect.Contains(Event.current.mousePosition);

            if (isMouseContainsCompo)
            {
                if (showLayerName)
                {
                    HierarchyLayer.OnGUI(gameObj, layerRect, isMouseContainsLayer);
                }
                if (showComponentIcons || showActiveToggles)
                {
                    HierarchyGUIComponent.OnGUI(gameObj, componentsRect, isMouseContainsCompo);
                }
            }
            else
            {
                if (showComponentIcons || showActiveToggles)
                {
                    HierarchyGUIComponent.OnGUI(gameObj, componentsRect, isMouseContainsCompo);
                }
                if (showLayerName)
                {
                    HierarchyLayer.OnGUI(gameObj, layerRect, isMouseContainsLayer);
                }
            }
        }
    }
}