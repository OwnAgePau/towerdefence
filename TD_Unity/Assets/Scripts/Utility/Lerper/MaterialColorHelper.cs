using UnityEngine;
using System.Collections;

public static class MaterialColorHelper {

    public static Color GetColor(Material mat) {
        if(mat.GetColor("_Color") != null) {
            return mat.GetColor("_Color");
        }
        else if(mat.GetColor("_TintColor") != null) {
            return mat.GetColor("_TintColor");
        }
        else if (mat.GetColor("_EmissiveColor") != null) {
            return mat.GetColor("_EmissiveColor");
        }
        else if (mat.GetColor("_EmisColor") != null) {
            return mat.GetColor("_EmisColor");
        }
        return Color.black;
    }

    public static void SetColor(Material material, Color colour) {
        if (material.GetColor("_Color") != null) {
            material.SetColor("_Color", colour);
        }
        else if (material.GetColor("_TintColor") != null) {
            material.SetColor("_TintColor", colour);
        }
        else if (material.GetColor("_EmissiveColor") != null) {
            material.SetColor("_EmissiveColor", colour);
        }
        else if (material.GetColor("_EmisColor") != null) {
            material.SetColor("_EmisColor", colour);
        }
    }
}