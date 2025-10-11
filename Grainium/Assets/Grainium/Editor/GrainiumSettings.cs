#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Grainium.EditorEx
{
    public sealed class GrainiumSettings : ScriptableObject
    {
        private static readonly string FilePath = "ProjectSettings/GrainiumSettings.json";
        private static readonly string MenuPath = "Project/Grainium";

        private static GrainiumSettings _instance;


        [SerializeField] private bool _showTreeMapHierarchy = true;
        [SerializeField] private bool _showComponentIcons = true;
        [SerializeField] private bool _showActiveToggles = true;
        [SerializeField] private bool _showComponentColors = true;
        [SerializeField] private Color _colorRigidbody = Color.blue;
        [SerializeField] private Color _colorCollider = Color.green;
        [SerializeField] private Color _colorCamera = Color.cyan;
        [SerializeField] private Color _colorLight = Color.yellow;
        [SerializeField] private Color _colorAudio = new Color(1, 0.6f, 0, 1);
        [SerializeField] private Color _colorGUI = Color.white;

        [SerializeField] private bool _showTreeMapProject = true;

        [SerializeField] private bool _showPingButton = true;
        [SerializeField] private bool _showPropertiesButton = true;



        public bool ShowTreeMapHierarchy => _showTreeMapHierarchy;
        public bool ShowTreeMapProject => _showTreeMapProject;
        public bool ShowActiveToggles => _showActiveToggles;
        public bool ShowComponentColors => _showComponentColors;
        public Color ColorRigidbody => _colorRigidbody;
        public Color ColorCollider => _colorCollider;
        public Color ColorCamera => _colorCamera;
        public Color ColorLight => _colorLight;
        public Color ColorAudio => _colorAudio;
        public Color ColorGUI => _colorGUI;
        public bool ShowComponentIcons => _showComponentIcons;
        public bool ShowPingButton => _showPingButton;
        public bool ShowPropertiesButton => _showPropertiesButton;

        internal static GrainiumSettings GetOrCreateInstance()
        {
            if (_instance != null)
                return _instance;

            _instance = CreateInstance<GrainiumSettings>();

            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                JsonUtility.FromJsonOverwrite(json, _instance);
            }

            return _instance;
        }

        private static void Save()
        {
            File.WriteAllText(FilePath, JsonUtility.ToJson(_instance));
        }

        [SettingsProvider]
        internal static SettingsProvider CreateMyCustomSettings()
        {
            return new SettingsProvider(MenuPath, SettingsScope.Project)
            {
                label = "Grainium",
                guiHandler = searchContext =>
                {
                    OnGUI();
                }
            };
        }
        private static void OnGUI()
        {
            using SerializedObject serializedObject = new(GetOrCreateInstance());
            using EditorGUI.ChangeCheckScope check = new();


            GUILayout.Label("Hierarchy", EditorStyles.boldLabel);

            var showTreeMapHierarchy = serializedObject.FindProperty("_showTreeMapHierarchy");
            EditorGUILayout.PropertyField(showTreeMapHierarchy, new GUIContent("Show TreeMap"));

            var showActiveToggles = serializedObject.FindProperty("_showActiveToggles");
            EditorGUILayout.PropertyField(showActiveToggles);

            var showComponentIcons = serializedObject.FindProperty("_showComponentIcons");
            EditorGUILayout.PropertyField(showComponentIcons);


            var showComponentColors = serializedObject.FindProperty("_showComponentColors");
            EditorGUILayout.PropertyField(showComponentColors);
            if (showComponentColors.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorRigidbody"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorCollider"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorCamera"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorLight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorAudio"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_colorGUI"));
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(10);
            GUILayout.Label("Project", EditorStyles.boldLabel);

            var showTreeMapProject = serializedObject.FindProperty("_showTreeMapProject");
            EditorGUILayout.PropertyField(showTreeMapProject, new GUIContent("Show TreeMap"));

            GUILayout.Space(10);
            GUILayout.Label("Inspector", EditorStyles.boldLabel);

            var showPingButton = serializedObject.FindProperty("_showPingButton");
            EditorGUILayout.PropertyField(showPingButton);

            var showPropertiesButton = serializedObject.FindProperty("_showPropertiesButton");
            EditorGUILayout.PropertyField(showPropertiesButton);

            if (check.changed)
            {
                serializedObject.ApplyModifiedProperties();
                Save();
            }
        }
    }
}
#endif