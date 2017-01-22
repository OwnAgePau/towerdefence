using UnityEditor;
using System.Collections;
using UnityEngine;

[CustomPropertyDrawer(typeof(Upgrade))]
public class UpgradePropertyDrawer : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        // Draw label
        label.text = "Level";
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        // Calculate rects
        //var amountRect = new Rect(position.x, position.y, 30, position.height);
        var unitRect = new Rect(position.x, position.y, 50, position.height);
        var nameRect = new Rect(position.x - 15, position.y + 20, position.width, position.height);
        // Draw fields - passs GUIContent.none to each so they are drawn without labels
        //EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("upgrades"), GUIContent.none);
        EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("level"), GUIContent.none);
        GUIContent cost = new GUIContent("Cost");
        EditorGUI.indentLevel += 1;
        EditorGUILayout.LabelField(cost);
        EditorGUI.PropertyField(nameRect, property.FindPropertyRelative("upgradeCost"), GUIContent.none);
        //EditorGUI.indentLevel += 1;
        EditorList.Show(property.FindPropertyRelative("upgrades"));
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }
}
