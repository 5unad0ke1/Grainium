#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class MySettingsProvider
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettings()
    {
        var provider = new SettingsProvider("Project/5un4d0k31", SettingsScope.Project)
        {
            label = "5un4d0k31",
            guiHandler = (searchContext) =>
            {
                OnGUI();
            }
        };
        return provider;
    }
    private static void OnGUI()
    {
        GUILayout.Label("TreeMap Settings", EditorStyles.boldLabel);
        bool hierarchy = Toggle("Hierarchy", PrefsKeyName.TreeMapHierarchy);
        Toggle("Project", PrefsKeyName.TreeMapProject);

        GUILayout.Space(10);
        if (hierarchy && Toggle("Use Components Colors", PrefsKeyName.TreeMapColor))
        {
            ColorField("Color Rigidbody", PrefsKeyName.TreeMapColor_RB);
            ColorField("Color Collider", PrefsKeyName.TreeMapColor_Collider);
            ColorField("Color Camera", PrefsKeyName.TreeMapColor_Cam);
            ColorField("Color Light", PrefsKeyName.TreeMapColor_Light);
            ColorField("Color Audio", PrefsKeyName.TreeMapColor_Audio);
            ColorField("Color GUI", PrefsKeyName.TreeMapColor_GUI);
        }
    }
    private static bool Toggle(string label, string key)
    {
        bool value = EditorPrefs.GetBool(key, false);
        bool newValue = EditorGUILayout.Toggle(label, value);
        if (newValue != value)
        {
            EditorPrefs.SetBool(key, newValue);
        }
        return newValue;
    }
    private static void ColorField(string label, string key)
    {
        Color value = Prefs.GetColor(key, Color.white);
        Color newValue = EditorGUILayout.ColorField(label, value);
        if (newValue != value)
        {
            Prefs.SetColor(key, newValue);
        }
    }
}
public static class Prefs
{

    public static Color GetColor(string key)
        => GetColor(key, Color.clear);
    public static Color GetColor(string key, Color defaultValue)
    {
        Color color;
        color.r = EditorPrefs.GetFloat(key + "_r", defaultValue.r);
        color.g = EditorPrefs.GetFloat(key + "_g", defaultValue.g);
        color.b = EditorPrefs.GetFloat(key + "_b", defaultValue.b);
        color.a = EditorPrefs.GetFloat(key + "_a", defaultValue.a);
        return color;
    }
    public static void SetColor(string key, Color value)
    {
        EditorPrefs.SetFloat(key + "_r", value.r);
        EditorPrefs.SetFloat(key + "_g", value.g);
        EditorPrefs.SetFloat(key + "_b", value.b);
        EditorPrefs.SetFloat(key + "_a", value.a);
    }
}
#endif