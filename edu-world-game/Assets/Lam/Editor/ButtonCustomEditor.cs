using UnityEngine;
using UnityEditor;

namespace Lam.UI
{
    [CustomEditor(typeof(ButtonCustom))]
    public class ButtonCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Lấy đối tượng đang chỉnh sửa
            ButtonCustom buttonCustom = (ButtonCustom)target;

            // Header cho Hover Effect
            EditorGUILayout.LabelField("Hover Settings", EditorStyles.boldLabel);
            buttonCustom.hoverEffect = (ButtonEffect)EditorGUILayout.EnumPopup("Hover Effect", buttonCustom.hoverEffect);

            // Hiển thị các trường tuỳ thuộc vào Hover Effect
            switch (buttonCustom.hoverEffect)
            {
                case ButtonEffect.None:
                    break;

                case ButtonEffect.Scale:
                    buttonCustom.hoverZoomFactor = EditorGUILayout.FloatField("Zoom Factor", buttonCustom.hoverZoomFactor);
                    buttonCustom.hoverDuration = EditorGUILayout.FloatField("Duration", buttonCustom.hoverDuration);
                    break;

                case ButtonEffect.HighlightBorder:
                    buttonCustom.hoverBorderColor = EditorGUILayout.ColorField("Border Color", buttonCustom.hoverBorderColor);
                    buttonCustom.hoverDuration = EditorGUILayout.FloatField("Duration", buttonCustom.hoverDuration);
                    break;

                case ButtonEffect.ChangeColor:
                    buttonCustom.hoverHighlightColor = EditorGUILayout.ColorField("Highlight Color", buttonCustom.hoverHighlightColor);
                    buttonCustom.hoverDuration = EditorGUILayout.FloatField("Duration", buttonCustom.hoverDuration);
                    break;
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Press Settings", EditorStyles.boldLabel);
            buttonCustom.pressEffect = (ButtonEffect)EditorGUILayout.EnumPopup("Press Effect", buttonCustom.pressEffect);

            switch (buttonCustom.pressEffect)
            {
                case ButtonEffect.None:
                    break;

                case ButtonEffect.Scale:
                    buttonCustom.pressZoomFactor = EditorGUILayout.FloatField("Zoom Factor", buttonCustom.pressZoomFactor);
                    buttonCustom.pressDuration = EditorGUILayout.FloatField("Duration", buttonCustom.pressDuration);
                    break;

                case ButtonEffect.HighlightBorder:
                    buttonCustom.pressBorderColor = EditorGUILayout.ColorField("Border Color", buttonCustom.pressBorderColor);
                    buttonCustom.pressDuration = EditorGUILayout.FloatField("Duration", buttonCustom.pressDuration);
                    break;

                case ButtonEffect.ChangeColor:
                    buttonCustom.pressHighlightColor = EditorGUILayout.ColorField("Highlight Color", buttonCustom.pressHighlightColor);
                    buttonCustom.pressDuration = EditorGUILayout.FloatField("Duration", buttonCustom.pressDuration);
                    break;
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(buttonCustom);
            }
        }
    }
}
