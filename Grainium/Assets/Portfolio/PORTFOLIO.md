# Portfolio - Grainium

<!-- Mono 12 -->
```
    ▄▄▄▄                          ██                  ██                        
  ██▀▀▀▀█                         ▀▀                  ▀▀                        
 ██         ██▄████   ▄█████▄   ████     ██▄████▄   ████     ██    ██  ████▄██▄ 
 ██  ▄▄▄▄   ██▀       ▀ ▄▄▄██     ██     ██▀   ██     ██     ██    ██  ██ ██ ██ 
 ██  ▀▀██   ██       ▄██▀▀▀██     ██     ██    ██     ██     ██    ██  ██ ██ ██ 
  ██▄▄▄██   ██       ██▄▄▄███  ▄▄▄██▄▄▄  ██    ██  ▄▄▄██▄▄▄  ██▄▄▄███  ██ ██ ██ 
    ▀▀▀▀    ▀▀        ▀▀▀▀ ▀▀  ▀▀▀▀▀▀▀▀  ▀▀    ▀▀  ▀▀▀▀▀▀▀▀   ▀▀▀▀ ▀▀  ▀▀ ▀▀ ▀▀ 
```
Grainium — “Minimal yet Flexible Editor Framework for Unity”

Hierarchy・Project・Inspectorを統一的・低依存的に拡張できるツールキット。
Unity Editor拡張の「**扱いやすさと分かりやすさの両立**」を追求した。

![Grainium Editor UI Screenshot showing hierarchy and inspector enhancements](Screenshot.png)

---

# #1 開発のキッカケ

## 問題

既存の[Alchemy](https://github.com/annulusgames/Alchemy)では、拡張時に以下の問題があった：
- PrefabのGameObjectでのボタンの干渉
- コンポーネントアイコンにTransformも表示され、視認性が悪い
- アンインストール後に不具合が発生

## 解決

**Grainiumでは**「干渉の少ない構造」と「処理の明確化」を目標に、
``EditorApplication.hierarchyWindowItemOnGUI()``等の既存イベントを活用し、
IMGUIをベースに**独立性重視**の描画構造を設計。

## 開発理由

[Alchemy](https://github.com/annulusgames/Alchemy)の作者本人はプルリクエストを求めるだけで修正には消極的だった。  
修正を試みるも構造が複雑で難しく、自分好みの拡張エディタライブラリを一から設計することにした。

# #2 技術的アプローチ

## 設計思想：
- 外部依存を減らし、リフレクションを最小限化
- IMGUIイベントの明確なフロー分離
- 「拡張ポイント」を明示化して他ツールと競合しにくくする

## 設計



この構造により、他ツールとの競合を防ぎつつ、拡張点を明示化できる

---

### 描画
```mermaid
graph TD

    subgraph UnityEditor
        A1["EditorApplication.hierarchyWindowItemOnGUI"]
        A2["Editor.finishedDefaultHeaderGUI"]
    end

    subgraph Grainium.Core
        B1["HierarchyEnhancer"]
        B2["InspectorButtons"]
        B3["GUITreeMap"]
        B4["GrainiumSettings"]
        B5["HierarchyGUIComponent"]
        B6["HierarchyLayer"]
    end


    %% Connections
    A1 --> B1
    A2 --> B2
    A1 --> B3

    B2 -->|uses| B4
    B3 -->|uses| B4

    B1 -->|uses| B5
    B1 -->|uses| B6
    B5 -->|uses| B4
    B6 -->|uses| B4

    style UnityEditor fill:#333,stroke:#777,color:#fff
    style Grainium.Core fill:#1e3a8a,stroke:#60a5fa,color:#fff
```

---

### クラス図

```mermaid
classDiagram
    direction LR

    class GrainiumSettings {
      <<ScriptableObject>>
      - static string FilePath
      - static string MenuPath
      - static GrainiumSettings _instance
      - bool _showTreeMapHierarchy
      - bool _showComponentIcons
      - bool _showActiveToggles
      - bool _showLayerName
      - float _layerNamePosition
      - bool _showComponentColors
      - Color _colorRigidbody
      - Color _colorCollider
      - Color _colorCamera
      - Color _colorLight
      - Color _colorAudio
      - Color _colorGUI
      - bool _showTreeMapProject
      - bool _showPingButton
      - bool _showPropertiesButton
      + bool ShowTreeMapHierarchy
      + bool ShowTreeMapProject
      + bool ShowActiveToggles
      + bool ShowLayerName
      + float LayerNamePosition
      + bool ShowComponentColors
      + Color ColorRigidbody
      + Color ColorCollider
      + Color ColorCamera
      + Color ColorLight
      + Color ColorAudio
      + Color ColorGUI
      + bool ShowComponentIcons
      + bool ShowPingButton
      + bool ShowPropertiesButton
      + static GrainiumSettings GetOrCreateInstance()
      - static void Save()
      + static SettingsProvider CreateMyCustomSettings()
      - static void OnGUI()
    }

    class GUITreeMap {
      <<InitializeOnLoad>>
      - static Texture2D _textureLine
      - static Texture2D _textureObj
      - static Texture2D _textureChild
      - static Texture2D _textureEnd
      - static Type[] SortedTypes
      + ~ OnGUIHierarchy(int, Rect)
      + ~ OnGUIProject(string, Rect)
      - ~ OnGUIProjectTwoColumnLayout(string, Rect)
      - ~ OnGUIProjectOneColumnLayout(string, Rect)
      - ~ int GetHierarchyDepth(Transform)
      - ~ bool IsLastSibling(Transform)
      - ~ bool IsLastSiblingFolder(string)
      - ~ bool IsOneColumnLayout()
      - ~ bool TryGetColor(GameObject, out Color)
      - ~ bool TryGetFirstType(GameObject, Type[], out Type)
    }

    class HierarchyEnhancer {
      <<InitializeOnLoad>>
      - ~ OnGUI(int, Rect)
    }

    class HierarchyGUIComponent {
      <<InitializeOnLoad>>
      - const int ICON_SIZE
      + ~ OnGUI(GameObject, Rect, bool)
      - ~ OnComponentIcons(Rect, Component[], bool)
      - ~ OnToggle(Rect, GameObject)
      - ~ bool IsPrefab(GameObject)
    }

    class HierarchyLayer {
      <<InitializeOnLoad>>
      + ~ OnGUI(GameObject, Rect, bool)
    }

    class InspectorButtons {
      <<InitializeOnLoad>>
      - ~ OnPostHeaderGUI(Editor)
      - ~ PingButton(Editor)
      - ~ PropertiesWindowButton(Editor)
    }

    class AssetHelper {
      + static T FindAssetAtPath~T~(string, string, bool)
      - static string GetFullPath(string)
    }

    %% イベント購読と依存関係
    GrainiumSettings <.. GUITreeMap : uses (描画可否/色設定)
    GrainiumSettings <.. HierarchyEnhancer : uses (表示設定/位置)
    GrainiumSettings <.. HierarchyGUIComponent : uses (表示設定)
    GrainiumSettings <.. HierarchyLayer : uses (表示設定)
    GrainiumSettings <.. InspectorButtons : uses (ボタン表示)

    AssetHelper <.. GUITreeMap : loads textures

    EditorApplication --> GUITreeMap : hierarchyWindowItemOnGUI\nprojectWindowItemOnGUI
    EditorApplication --> HierarchyEnhancer : hierarchyWindowItemOnGUI
    Editor.finishedDefaultHeaderGUI --> InspectorButtons : OnPostHeaderGUI

    HierarchyEnhancer --> HierarchyGUIComponent : OnGUI(..)
    HierarchyEnhancer --> HierarchyLayer : OnGUI(..)
```

---

### Hierarchy拡張

```mermaid
flowchart LR
    A[Unity Editor\nhierarchyWindowItemOnGUI] --> B{GUITreeMap 有効?}
    B -->|色帯表示設定ON| C[GUITreeMap.OnGUIHierarchy\n- コンポーネント種別から色決定\n- 行頭に半透明色帯]
    B -->|OFF or 後段| D[TreeMapガイド描画\n- Line/Obj/Child/End テクスチャ]
    C --> D
    D --> E[行レイアウト確定]

    A --> F[HierarchyEnhancer.OnGUI]
    F --> G{表示設定}
    G -->|Layer名ON| H[HierarchyLayer.OnGUI\n- miniLabelでLayer名 マウス時はwidth調整]
    G -->|アイコン/トグルON| I[HierarchyGUIComponent.OnGUI\n- アクティブトグル\n- コンポーネントアイコン\n- Prefabマーク分の余白調整\n- ホバー時は全表示/非ホバーは省略+~]
    H --> J[行の右側UI]
    I --> J
    E --> J
    J --> K[最終描画]
```

---
### Inspectorボタン拡張
```mermaid
sequenceDiagram
    participant Editor as Unity Editor
    participant InspectorButtons
    Editor->>InspectorButtons: finishedDefaultHeaderGUI
    InspectorButtons->>InspectorButtons: 設定チェック
    alt Ping
        InspectorButtons->>Editor: EditorGUIUtility.PingObject(target)
    end
    alt Properties
        InspectorButtons->>Editor: EditorUtility.OpenPropertyEditor(target)
    end
```

## 実装例

```cs
// Transform階層の深さを再帰なしで算出
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
```

```cs
// Projectウィンドウのレイアウト（1列/2列）を判定
private static bool IsOneColumnLayout()
{
    var type = typeof(Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
    var windows = Resources.FindObjectsOfTypeAll(type);
    if (windows.Length == 0) return true;

    var field = type.GetField("m_ViewMode", BindingFlags.Instance | BindingFlags.NonPublic);
    int viewMode = (int)field.GetValue(windows[0]);
    return viewMode == 0;
}
```

```cs
// Inspector上部にPing/Propertiesボタンを追加
 [InitializeOnLoad]
 internal static class InspectorButtons
 {
     static InspectorButtons()
     {
         Editor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
     }
     private static void OnPostHeaderGUI(Editor editor)
     {
         GUILayout.BeginHorizontal();
         GUILayout.FlexibleSpace();

         if(GrainiumSettings.GetOrCreateInstance().ShowPingButton)
             PingButton(editor);
         if (GrainiumSettings.GetOrCreateInstance().ShowPropertiesButton)
             PropertiesWindowButton(editor);

         GUILayout.EndHorizontal();
     }
     private static void PingButton(Editor editor)
     {
         if (GUILayout.Button("Ping", GUILayout.Width(120)))
         {
             EditorGUIUtility.PingObject(editor.target);
         }
     }
     private static void PropertiesWindowButton(Editor editor)
     {
         if (GUILayout.Button("Properties", GUILayout.Width(120)))
         {
             EditorUtility.OpenPropertyEditor(editor.target);
         }
     }
 }
```
# #3 学び・改善点

- 改善点
  - GUIStyleの再利用によるGC削減
  - 今後は一部機能をUnityTKベースに置き換えによる、拡張性の向上。
- 学び
  - Event.current.typeの判定順序を理解したことで、Layout→Repaint間のState同期が安定した
  - IMGUIのイベント循環とGUIStyle制御を理解した。
  - 描画コストや計算コストなどのパフォーマンスチューニングの感覚を知れた。

# #4 今後の展望

- UIElement対応版の試作
- Hierarchy・Inspectorを統一的に扱えるエディタツールキットを目指す。
- よく使う設計をUtilityやフレームワーク化し、新規OSSとして公開。


© 2025 OIKAWA Yuki / 5unad0ke1 
GitHub: github.com/5unad0ke1