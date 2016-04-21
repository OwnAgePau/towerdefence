using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(UpgradePart))]
public class UpgradePartPropertyDrawer : PropertyDrawer{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        label.text = "Upgrade";
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        // Calculate rects
        var typeRect = new Rect(position.x, position.y, 85, position.height);
        var amountRect = new Rect(position.x + 90, position.y, position.width - 90, position.height);
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(typeRect, property.FindPropertyRelative("type"), GUIContent.none);
        EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
