#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    [InitializeOnLoad]
    internal static class HierarchyGUIComponent
    {
        private const int ICON_SIZE = 14;

        static HierarchyGUIComponent()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        }

        private static void OnGUI(int instanceID, Rect selectionRect)
        {
            bool showComponentIcons = GrainiumSettings.GetOrCreateInstance().ShowComponentIcons;
            bool showActiveToggles = GrainiumSettings.GetOrCreateInstance().ShowActiveToggles;

            if (!showComponentIcons && !showActiveToggles)
                return;

            // instanceID をオブジェクト参照に変換
            var gameObj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObj == null)
            {
                return;
            }

            // オブジェクトが所持しているコンポーネント一覧を取得
            var components = gameObj.GetComponents<Component>();
            if (components.Length == 0)
            {
                return;
            }

            if (showActiveToggles)
            {
                selectionRect.x = selectionRect.xMax - ICON_SIZE * components.Length;
            }
            else
            {

                selectionRect.x = selectionRect.xMax - ICON_SIZE * (components.Length - 1);
            }
            if (IsPrefab(gameObj))
            {
                selectionRect.x -= ICON_SIZE;
            }
            selectionRect.width = ICON_SIZE;
            selectionRect.height = ICON_SIZE;

            foreach (var component in components)
            {
                if (!showComponentIcons)
                {
                    selectionRect.x += ICON_SIZE;
                    continue;
                }

                if (component is Transform)
                {
                    selectionRect.x += ICON_SIZE;
                    continue;
                }

                // コンポーネントのアイコン画像を取得
                var texture2D = AssetPreview.GetMiniThumbnail(component);

                GUI.DrawTexture(selectionRect, texture2D);
                selectionRect.x += ICON_SIZE;
            }

            if (!showActiveToggles)
                return;

            bool active = gameObj.activeSelf;
            bool value = GUI.Toggle(selectionRect, active, "");
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