#if UNITY_EDITOR
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    [InitializeOnLoad]
    internal static class GUITreeMap
    {
        private static Texture2D _textureLine;
        private static Texture2D _textureObj;
        private static Texture2D _textureChild;
        private static Texture2D _textureEnd;

        static GUITreeMap()
        {
            //AssetDatabase.LoadAssetAtPath()
            _textureLine = AssetHelper.FindAssetAtPath<Texture2D>("Line.png", "Texture");
            _textureObj = AssetHelper.FindAssetAtPath<Texture2D>("Obj.png", "Texture");
            _textureChild = AssetHelper.FindAssetAtPath<Texture2D>("Child.png", "Texture");
            _textureEnd = AssetHelper.FindAssetAtPath<Texture2D>("End.png", "Texture");

            EditorApplication.hierarchyWindowItemOnGUI += OnGUIHierarchy;
            EditorApplication.projectWindowItemOnGUI += OnGUIProject;
        }
        private static void OnGUIHierarchy(int instanceID, Rect selectionRect)
        {
            var gameObj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObj == null)
            {
                return;
            }

            int depth = GetHierarchyDepth(gameObj.transform);
            Transform parent = gameObj.transform;
            bool hasChild = parent.childCount > 0;

            if (GrainiumSettings.GetOrCreateInstance().ShowComponentColors)
                if (TryGetColor(gameObj, out var color))
                {
                    var rect = selectionRect;
                    rect.xMax = 14 * 3 + 4;
                    rect.xMin = 14 * 2 + 4;
                    color.a = 0.5f;
                    EditorGUI.DrawRect(rect, color);
                }

            if (!GrainiumSettings.GetOrCreateInstance().ShowTreeMapHierarchy)
            {
                return;
            }

            selectionRect.width = 14;
            selectionRect.height = 16;
            selectionRect.x = selectionRect.xMax - selectionRect.width * 2 - 1;

            if (!hasChild)
            {
                GUI.DrawTexture(selectionRect, _textureChild);
            }
            selectionRect.x -= selectionRect.width;

            if (IsLastSibling(parent))
            {
                GUI.DrawTexture(selectionRect, _textureEnd);
            }
            else
            {
                GUI.DrawTexture(selectionRect, _textureObj);
            }
            selectionRect.x -= selectionRect.width;


            for (int i = 0; i < depth; i++)
            {
                if (parent.parent != null && IsLastSibling(parent.parent))
                {
                    selectionRect.x -= selectionRect.width;
                    parent = parent.parent;
                    continue;
                }
                GUI.DrawTexture(selectionRect, _textureLine);
                selectionRect.x -= selectionRect.width;
                parent = parent.parent;
            }
        }
        private static void OnGUIProject(string guid, Rect selectionRect)
        {
            if (!GrainiumSettings.GetOrCreateInstance().ShowTreeMapProject)
            {
                return;
            }

            if (IsOneColumnLayout())
            {
                return;
            }
            if (selectionRect.x < 16 || selectionRect.height > 16)
            {
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            var splitPath = path.Split('/');
            int depth = splitPath.Length - 2;

            if (false && depth >= 3)
            {
                Color original = GUI.color;
                GUI.color = Color.red;
                GUI.Box(selectionRect, string.Empty);
                GUI.color = original;
            }

            if (depth <= -1)
                return;

            selectionRect.width = 14;
            selectionRect.height = 16;
            selectionRect.x = selectionRect.xMin - selectionRect.width - 1;

            if (AssetDatabase.GetSubFolders(path).Length == 0)
            {
                GUI.DrawTexture(selectionRect, _textureChild);
            }
            selectionRect.x -= selectionRect.width;

            if (IsLastSiblingFolder(path))
            {
                GUI.DrawTexture(selectionRect, _textureEnd);
            }
            else
            {
                GUI.DrawTexture(selectionRect, _textureObj);
            }
            selectionRect.x -= selectionRect.width;

            for (int i = 0; i < depth; i++)
            {
                path = Path.GetDirectoryName(path).Replace("\\", "/");
                if (!string.IsNullOrEmpty(path) && IsLastSiblingFolder(path))
                {
                    selectionRect.x -= selectionRect.width;
                    continue;
                }
                GUI.DrawTexture(selectionRect, _textureLine);
                selectionRect.x -= selectionRect.width;
            }
        }
        private static int GetHierarchyDepth(Transform t)
        {
            int depth = 0;
            while (t.parent != null)
            {
                depth++;
                t = t.parent;
            }
            return depth;
        }
        private static bool IsLastSibling(Transform t)
        {
            if (t.parent == null)
            {
                return false;
            }
            return t.GetSiblingIndex() == t.parent.childCount - 1;
        }
        private static T GetAsset<T>(string name) where T : Object
        {
          
            string[] guids = AssetDatabase.FindAssets(name);
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null && asset.name == name)
                {
                    return asset;
                }
            }
            return null;
        }
        private static bool IsLastSiblingFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
                return false;

            string parent = Path.GetDirectoryName(folderPath).Replace("\\", "/");
            if (string.IsNullOrEmpty(parent))
                return false;

            string[] siblings = AssetDatabase.GetSubFolders(parent);
            if (siblings.Length == 0)
                return false;

            string last = siblings[^1];

            return folderPath == last;
        }
        private static bool IsOneColumnLayout()
        {
            var type = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            var windows = Resources.FindObjectsOfTypeAll(type);
            if (windows.Length == 0) return true;

            var field = type.GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
            int viewMode = (int)field.GetValue(windows[0]);
            return viewMode == 0;
        }
        private static bool TryGetColor(GameObject obj, out Color color)
        {
            if (obj.TryGetComponent(out Rigidbody rb) || obj.TryGetComponent(out Rigidbody2D rb2d))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorRigidbody;
                return true;
            }
            if (obj.TryGetComponent(out Collider col) || obj.TryGetComponent(out Collider2D col2))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorCollider;
                return true;
            }
            if (obj.TryGetComponent(out Camera cam))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorCamera;
                return true;
            }
            if (obj.TryGetComponent(out AudioSource audio))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorAudio;
                return true;
            }
            if(obj.TryGetComponent(out Light light))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorLight;
                return true;
            }
            if (obj.TryGetComponent(out RectTransform rect))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorGUI;
                return true;
            }

            color = Color.clear;
            return false;
        }
    }
}
#endif