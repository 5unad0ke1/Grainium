#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    [InitializeOnLoad]
    internal static class HierarchyGUIComponent
    {
        private const int ICON_SIZE = 14;

        public static void OnGUI(GameObject gameObj, Rect selectionRect, bool isMouseContains)
        {
            bool showComponentIcons = GrainiumSettings.GetOrCreateInstance().ShowComponentIcons;
            bool showActiveToggles = GrainiumSettings.GetOrCreateInstance().ShowActiveToggles;
            if (!showComponentIcons && !showActiveToggles)
                return;

            if (gameObj == null)
            {
                return;
            }

            var components = gameObj.GetComponents<Component>();
            if (components.Length == 0)
            {
                return;
            }





            if (IsPrefab(gameObj))
            {
                selectionRect.xMax -= ICON_SIZE;
            }

            if (showActiveToggles)
            {
                OnToggle(selectionRect, gameObj);
                selectionRect.xMax -= ICON_SIZE;
            }
            if (showComponentIcons)
            {
                OnComponentIcons(selectionRect, components, isMouseContains);
            }
        }
        private static void OnComponentIcons(Rect selectionRect, Component[] components, bool isMouseContains)
        {
            Rect boxRect = new(selectionRect);
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

                // コンポーネントのアイコン画像を取得
                var texture2D = AssetPreview.GetMiniThumbnail(components[i]);

                GUI.DrawTexture(boxRect, texture2D);
                boxRect.x += ICON_SIZE;
            }
            if (isOverflow&&length > 1)
            {
                boxRect.x = selectionRect.xMax - ICON_SIZE * 2;
                GUI.Box(boxRect, "~", EditorStyles.label);
            }
        }
        private static void OnToggle(Rect rect,GameObject gameObj)
        {
            rect.xMin = rect.xMax - ICON_SIZE;
            rect.width = ICON_SIZE;
            rect.height = ICON_SIZE;
            rect.x = rect.xMin;

            bool active = gameObj.activeSelf;
            bool value = GUI.Toggle(rect, active, string.Empty);
            if (active != value)
            {
                Undo.RecordObject(gameObj, "Toggle Active");
                gameObj.SetActive(value);
                EditorUtility.SetDirty(gameObj);
            }
        } 
        private static bool IsPrefab(GameObject gameObj)
        {
            if (gameObj == null)
                return false;
            var prefabType = PrefabUtility.GetPrefabAssetType(gameObj);
            return prefabType != PrefabAssetType.NotAPrefab;
        }
    }
}
#endif