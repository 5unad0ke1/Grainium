using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorExtensions.Hierarchy
{
    [InitializeOnLoad]
    internal static class Enhancer
    {
        static Enhancer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        }


        private static void OnGUI(int instanceID, Rect selectionRect)
        {
            Rect rect = selectionRect;
            var gameObj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObj == null)
                return;

            rect.xMax += 16;

            if (IsPrefab(gameObj))
            {
                rect.xMax -= PREFAB_BUTTON_SIZE;
            }

            rect.xMin += EditorStyles.label.CalcSize(new GUIContent(gameObj.name)).x + 16 + 4;

            if (GrainiumSettings.GetOrCreateInstance().ShowActiveToggles)
                DrawToggle(gameObj, ref rect);

            if (GrainiumSettings.GetOrCreateInstance().ShowChildCount)
                DrawChildCount(gameObj, ref rect);

            if (GrainiumSettings.GetOrCreateInstance().ShowLayerName)
                DrawLayerName(gameObj, ref rect);

            if (GrainiumSettings.GetOrCreateInstance().ShowComponentIcons)
                GUIComponent.OnGUI(gameObj, rect, rect.Contains(Event.current.mousePosition));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawLayerName(in GameObject gameObj, ref Rect rect)
        {
            Rect rectSize = rect;
            rectSize.xMax = rectSize.xMin + rectSize.width * 0.4f;
            HierarchyLayer.OnGUI(gameObj, rectSize);

            rect.xMin += rectSize.width;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawToggle(in GameObject gameObj, ref Rect rect)
        {
            Rect rectSize = rect;
            rectSize.xMin = rectSize.xMax - ACTIVE_TOGGLE_SIZE;
            ActiveToggle.Draw(gameObj, rectSize);

            rect.xMax -= ACTIVE_TOGGLE_SIZE;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawChildCount(in GameObject gameObj, ref Rect rect)
        {
            Rect rectSize = rect;
            int size = ChildCountDrawer.GetSize(gameObj);
            rectSize.xMin = rectSize.xMax - size;
            ChildCountDrawer.Draw(gameObj, rectSize);

            rect.xMax -= size;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsPrefab(in GameObject gameObj)
        {
            if (gameObj == null)
                return false;
            var prefabType = PrefabUtility.GetPrefabAssetType(gameObj);
            return prefabType != PrefabAssetType.NotAPrefab;
        }


        private const int ACTIVE_TOGGLE_SIZE = 16;
        private const int PREFAB_BUTTON_SIZE = 16;
    }
}
