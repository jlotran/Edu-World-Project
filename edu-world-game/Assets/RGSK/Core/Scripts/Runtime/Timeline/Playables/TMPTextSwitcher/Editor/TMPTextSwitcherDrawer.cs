using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

namespace RGSK.Editor
{
    [CustomPropertyDrawer(typeof(TMPTextSwitcherBehaviour))]
    public class TMPTextSwitcherDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
        {
            int fieldCount = 3;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty colorProp = property.FindPropertyRelative("color");
            SerializedProperty fontSizeProp = property.FindPropertyRelative("fontSize");
            SerializedProperty textProp = property.FindPropertyRelative("text");

            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, textProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, fontSizeProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, colorProp);
        }
    }
}