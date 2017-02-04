using UnityEditor;
using System.Collections;
using UnityEngine;

[CustomEditor(typeof(Lerper)), CanEditMultipleObjects]
public class EditorLerper : Editor {

    SerializedProperty lerpToOrFrom;
    SerializedProperty position;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        Lerper lerper = (Lerper)target;
        EditorGUI.indentLevel = 0;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true);
        SerializedProperty lerpWrapper = serializedObject.FindProperty("lerpWrapper");
        EditorGUILayout.PropertyField(lerpWrapper);
        if (lerpWrapper.isExpanded) {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Lerper", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("duration"));
            EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("startDelay"));
            EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("curve"));
            EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("objectToLerp"));
            SerializedProperty startOnAwakeProp = lerpWrapper.FindPropertyRelative("startOnAwake");
            SerializedProperty startOnEnableProp = lerpWrapper.FindPropertyRelative("startOnEnable");
            this.position = lerpWrapper.FindPropertyRelative("position");

            if (!startOnAwakeProp.boolValue) {
                EditorGUILayout.PropertyField(startOnEnableProp);
            }
            if (!startOnEnableProp.boolValue) {
                EditorGUILayout.PropertyField(startOnAwakeProp);
            }

            this.lerpToOrFrom = lerpWrapper.FindPropertyRelative("lerpToOrFrom");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Lerp Type", EditorStyles.boldLabel);
            SerializedProperty lerpTypeProperty = lerpWrapper.FindPropertyRelative("lerpType");
            EditorGUILayout.PropertyField(lerpTypeProperty);
            LerpType lerpTypeValue = (LerpType)lerpTypeProperty.enumValueIndex;

            switch (lerpTypeValue) {
                case LerpType.Color:
                    this.ShowColorLerpFields(lerpWrapper);
                    break;
                case LerpType.Position:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
                    this.ShowPositionLerpFields(lerpWrapper);
                    break;
                case LerpType.Rotation:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
                    this.ShowRotationLerpFields(lerpWrapper);
                    break;
                case LerpType.Scale:
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("scale"));
                    //EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("endScale"));
                    EditorGUILayout.PropertyField(this.lerpToOrFrom);
                    break;
            }

            SerializedProperty loopingProperty = lerpWrapper.FindPropertyRelative("amountOfLoops");
            SerializedProperty goToAfterFinishedProperty = lerpWrapper.FindPropertyRelative("goToAfterFinished");
            if (loopingProperty.intValue == 0) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Next Lerper", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(goToAfterFinishedProperty);
            }

            if (goToAfterFinishedProperty.objectReferenceValue == null) {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Looping", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(loopingProperty);
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("loopWithDelay"));
            }
        }
        serializedObject.ApplyModifiedProperties();
    }


    void ShowColorLerpFields(SerializedProperty lerpWrapper) {
        SerializedProperty lerpColorTypeProperty = lerpWrapper.FindPropertyRelative("lerpColorType");
        EditorGUILayout.PropertyField(lerpColorTypeProperty);
        LerpColorType lerpColorType = (LerpColorType)lerpColorTypeProperty.enumValueIndex;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Color", EditorStyles.boldLabel);
        switch (lerpColorType) {
            case LerpColorType.ColorShift:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("normalMaterial"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("color"));
                //EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("endColor"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpColorType.Fade:
                this.ShowFadeLerpFields(lerpWrapper);
                break;
            case LerpColorType.HueShift:
                this.ShowHueShiftLerpFields(lerpWrapper);
                break;
        }
    }

    void ShowHueShiftLerpFields(SerializedProperty lerpWrapper) {
        SerializedProperty lerpHueShiftTypeProperty = lerpWrapper.FindPropertyRelative("lerpHueShiftType");
        EditorGUILayout.PropertyField(lerpHueShiftTypeProperty);
        LerpHueShiftType lerpHueShiftType = (LerpHueShiftType)lerpHueShiftTypeProperty.enumValueIndex;
        switch (lerpHueShiftType) {
            case LerpHueShiftType.Material:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("normalMaterial"));
                break;
            case LerpHueShiftType.Text:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("text"));
                break;
        }
    }

    void ShowFadeLerpFields(SerializedProperty lerpWrapper) {
        SerializedProperty lerpMaterialTypeProperty = lerpWrapper.FindPropertyRelative("lerpMaterialType");
        EditorGUILayout.PropertyField(lerpMaterialTypeProperty);
        LerpMaterialType lerpMaterialType = (LerpMaterialType)lerpMaterialTypeProperty.enumValueIndex;
        switch (lerpMaterialType) {
            case LerpMaterialType.Mesh:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("alpha"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpMaterialType.Image:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("alpha"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpMaterialType.Text:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("alpha"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpMaterialType.Particle:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("alpha"));
                //EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("endAlpha"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("resetOnFinish"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("normalMaterial"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpMaterialType.Other:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("alpha"));
                //EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("endAlpha"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("resetOnFinish"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("normalMaterial"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
        }
        EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("applyToChildren"));
    }

    void ShowPositionLerpFields(SerializedProperty lerpWrapper) {
        SerializedProperty lerpPositionTypeProperty = lerpWrapper.FindPropertyRelative("lerpPositionType");
        EditorGUILayout.PropertyField(lerpPositionTypeProperty);
        LerpPositionType lerpPositionType = (LerpPositionType)lerpPositionTypeProperty.enumValueIndex;
        switch (lerpPositionType) {
            case LerpPositionType.Force:
                EditorGUILayout.PropertyField(this.position);
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpPositionType.ForceWithCurve:
                EditorGUILayout.PropertyField(this.position);
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpPositionType.PointToPoint:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("startPos"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("endPos"));
                break;
            case LerpPositionType.Point:
                EditorGUILayout.PropertyField(this.position);
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpPositionType.ObjectWithOffset:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("target"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("targetOffset"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
            case LerpPositionType.UI_PointToPoint:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("UI_StartPosition"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("UI_EndPosition"));
                break;
            case LerpPositionType.UI_Point:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("UI_Position"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
        }
    }

    void ShowRotationLerpFields(SerializedProperty lerpWrapper) {
        SerializedProperty lerpRelativeTypeProperty = lerpWrapper.FindPropertyRelative("lerpRelative");
        EditorGUILayout.PropertyField(lerpRelativeTypeProperty);
        LerpRelative lerpRelative = (LerpRelative)lerpRelativeTypeProperty.enumValueIndex;
        switch (lerpRelative) {
            case LerpRelative.NotRelative:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("startRotation"));
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("endRotation"));
                break;
            case LerpRelative.Relative:
                EditorGUILayout.PropertyField(lerpWrapper.FindPropertyRelative("rotation"));
                EditorGUILayout.PropertyField(this.lerpToOrFrom);
                break;
        }
    }
}