using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

namespace Grainium.EditorEx
{
    [InitializeOnLoad]
    internal static class HierarchyLayer
    {
        static HierarchyLayer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        }
        private static void OnGUI(int instanceID, Rect selectionRect)
        {
            if (!GrainiumSettings.GetOrCreateInstance().ShowLayerName)
            {
                return;
            }
            var gameObj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObj == null)
            {
                return;
            }
            selectionRect.x = selectionRect.max.x * GrainiumSettings.GetOrCreateInstance().LayerNamePosition;
            GUI.Box(selectionRect, LayerMask.LayerToName(gameObj.layer), EditorStyles.miniLabel);
        }
    }
}