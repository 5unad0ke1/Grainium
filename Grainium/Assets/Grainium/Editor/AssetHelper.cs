using System.IO;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    internal static class AssetHelper
    {
        public static T FindAssetAtPath<T>(string nameAsset, string relativePath, bool outsideScope = false) where T : Object
        {
            string path = outsideScope ? $"{relativePath}/{nameAsset}" : GetFullPath($"{relativePath}/{nameAsset}");
            var t = AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (t == null) 
                throw new System.NullReferenceException($"Couldn't load the {nameof(T)} at path :{path}");
            return t as T;
        }

        private static string GetFullPath(string path)
        {
            var upmPath = $"Packages/com.5unad0ke1.grainium/{path}";
            var normalPath = $"Assets/Grainium/{path}";
            return !File.Exists(Path.GetFullPath(upmPath)) ? normalPath : upmPath;
        }
    }
}