#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public sealed class PropertiesWindow : EditorWindow
{
    private Object target; // D&Dされたアセットを保持

    [MenuItem("Window/Properties Window")]
    public static void ShowWindow()
    {
        GetWindow<PropertiesWindow>("Properties");
    }

    private void OnGUI()
    {
        GUILayout.Label("Drop Asset Here", EditorStyles.boldLabel);

        // ドラッグ＆ドロップ受付エリア
        Rect dropArea = GUILayoutUtility.GetRect(0, 50, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, target ? target.name : "Drag & Drop Asset");

        HandleDragAndDrop(dropArea);

        // 対象があればインスペクタ風に表示
        if (target != null)
        {
            Editor editor = Editor.CreateEditor(target);
            if (editor != null)
            {
                editor.OnInspectorGUI();
            }
        }
    }

    private void HandleDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;
        if (!dropArea.Contains(evt.mousePosition))
            return;

        switch (evt.type)
        {
            case EventType.DragUpdated:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                evt.Use();
                break;

            case EventType.DragPerform:
                DragAndDrop.AcceptDrag();
                foreach (Object dragged in DragAndDrop.objectReferences)
                {
                    target = dragged; // 最初のアセットだけ保持
                    break;
                }
                evt.Use();
                Repaint();
                break;
        }
    }
}
#endif