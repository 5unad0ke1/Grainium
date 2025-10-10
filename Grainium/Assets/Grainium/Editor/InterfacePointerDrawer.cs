#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Sunadokei.EditorEx
{
    [CustomPropertyDrawer(typeof(InterfacePointer<>), true)]
    public class InterfacePointerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // _object フィールドを取得
            var objProp = property.FindPropertyRelative("_object");

            // Inspector 上の ObjectField
            EditorGUI.BeginChangeCheck();
            Object obj = EditorGUI.ObjectField(position, label, objProp.objectReferenceValue, typeof(Object), false);

            // Generic Type T を取得
            var fieldType = fieldInfo.FieldType;
            var genericType = fieldType.IsGenericType ? fieldType.GetGenericArguments()[0] : null;

            if (genericType != null && genericType.IsInterface)
            {
                // 選択可能かチェック
                if (obj == null || genericType.IsAssignableFrom(obj.GetType()))
                {
                    objProp.objectReferenceValue = obj;
                }
                else
                {
                    EditorGUI.ObjectField(position, label, null, typeof(Object), false);
                    EditorGUILayout.HelpBox($"{obj.name} does not implement {genericType.Name}", MessageType.Warning);
                }
            }
            else
            {
                objProp.objectReferenceValue = obj;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
