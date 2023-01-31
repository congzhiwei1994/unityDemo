using System;
using UnityEngine;
using UnityEditor;

namespace czw.FlowMapTool
{
    public class FlowMapViewUtils
    {
        public static Vector3 Vector3GUI(string text, Vector3 value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(text));
            var newValue = EditorGUILayout.Vector3Field("", value);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }

        public static float SliderGUI(string text, float value, float leftValue, float rightValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(text));
            var newValue = EditorGUILayout.Slider(value, leftValue, rightValue);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }
        public static int IntSliderGUI(string text, int value, int leftValue, int rightValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(text));
            var newValue = EditorGUILayout.IntSlider(value, leftValue, rightValue);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }
        
        public static Enum EnumPopupGUI(string text, Enum value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(text));
            var newValue = EditorGUILayout.EnumPopup(value);
            EditorGUILayout.EndHorizontal();
            return newValue;
        }
    }
}