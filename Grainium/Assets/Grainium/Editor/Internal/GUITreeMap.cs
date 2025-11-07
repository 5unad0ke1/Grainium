#if UNITY_EDITOR
using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    [InitializeOnLoad]
    internal static class GUITreeMap
    {
        static GUITreeMap()
        {
            _textureLine = AssetHelper.FindAssetAtPath<Texture2D>("Line.png", "Texture");
            _textureObj = AssetHelper.FindAssetAtPath<Texture2D>("Obj.png", "Texture");
            _textureChild = AssetHelper.FindAssetAtPath<Texture2D>("Child.png", "Texture");
            _textureEnd = AssetHelper.FindAssetAtPath<Texture2D>("End.png", "Texture");

            EditorApplication.hierarchyWindowItemOnGUI += OnGUIHierarchy;
            EditorApplication.projectWindowItemOnGUI += OnGUIProject;
        }

        private static Texture2D _textureLine;
        private static Texture2D _textureObj;
        private static Texture2D _textureChild;
        private static Texture2D _textureEnd;

        private static Type[] SortedTypes =
        {
            typeof(Rigidbody),typeof(Rigidbody2D),
            typeof(Collider),typeof(Collider2D),
            typeof(Camera),
            typeof(AudioSource),
            typeof(Light),
            typeof(RectTransform)
        };

        
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
                OnGUIProjectOneColumnLayout(guid, selectionRect);
            }
            else
            {
                OnGUIProjectTwoColumnLayout(guid, selectionRect);
            }
        }
        private static void OnGUIProjectTwoColumnLayout(string guid, Rect selectionRect)
        {
            if (Event.current.type != EventType.Repaint)
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
        private static void OnGUIProjectOneColumnLayout(string guid,Rect selectionRect)
        {
            //開発中で未実装
            return;

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
        private static bool IsLastSiblingFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
                return false;

            string parent = Path.GetDirectoryName(folderPath);
            if (string.IsNullOrEmpty(parent))
                return false;
            parent = parent.Replace("\\", "/");

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
            if (!TryGetFirstType(obj, SortedTypes, out var type))
            {
                color = Color.clear;
                return false;
            }

            if (type == typeof(Rigidbody) || type == typeof(Rigidbody2D))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorRigidbody;
                return true;
            }
            if (type == typeof(Collider) || type == typeof(Collider2D))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorCollider;
                return true;
            }
            if (type == typeof(Camera))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorCamera;
                return true;
            }
            if (type == typeof(AudioSource))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorAudio;
                return true;
            }
            if (type == typeof(Light))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorLight;
                return true;
            }
            if (type == typeof(RectTransform))
            {
                color = GrainiumSettings.GetOrCreateInstance().ColorGUI;
                return true;
            }

            color = Color.clear;
            return false;
        }
        private static bool TryGetFirstType(GameObject obj, Type[] sorted, out Type result)
        {
            for (int i = 0; i < sorted.Length; i++)
            {
                if (!obj.TryGetComponent(sorted[i], out _))
                    continue;
                result = sorted[i];
                return true;
            }
            result = null;
            return false;
        }
    }
}
#endif