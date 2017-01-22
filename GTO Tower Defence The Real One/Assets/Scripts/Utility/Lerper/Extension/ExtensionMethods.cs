using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ExtensionMethods {

    public static List<Material> GetChildMaterials(this Transform trans) {
        List<Material> materials = new List<Material>();
        for (int i = 0; i < trans.childCount; i++) {
            Transform childTransform = trans.GetChild(i);
            MeshRenderer renderer = childTransform.GetComponent<MeshRenderer>();
            if (renderer != null) {
                materials.Add(renderer.materials[0]);
            }
            if (childTransform.childCount > 0) {
                materials.AddRange(childTransform.GetChildMaterials());
            }
        }
        return materials;
    }

    public static List<Image> GetChildImages(this Transform trans) {
        List<Image> materials = new List<Image>();
        for (int i = 0; i < trans.childCount; i++) {
            Transform childTransform = trans.GetChild(i);
            Image image = childTransform.GetComponent<Image>();
            if(image != null) {
                materials.Add(childTransform.GetComponent<Image>());
            }           
            if (childTransform.childCount > 0) {
                materials.AddRange(childTransform.GetChildImages());
            }
        }
        return materials;
    }
}