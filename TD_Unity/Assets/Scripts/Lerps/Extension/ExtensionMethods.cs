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
        List<Image> images = new List<Image>();
        for (int i = 0; i < trans.childCount; i++) {
            Transform childTransform = trans.GetChild(i);
            Image image = childTransform.GetComponent<Image>();
            if(image != null) {
                images.Add(childTransform.GetComponent<Image>());
            }           
            if (childTransform.childCount > 0) {
                images.AddRange(childTransform.GetChildImages());
            }
        }
        return images;
    }

    public static List<Text> GetChildTexts(this Transform trans) {
        List<Text> texts = new List<Text>();
        for (int i = 0; i < trans.childCount; i++) {
            Transform childTransform = trans.GetChild(i);
            Text text = childTransform.GetComponent<Text>();
            if (text != null) {
                texts.Add(childTransform.GetComponent<Text>());
            }
            if (childTransform.childCount > 0) {
                texts.AddRange(childTransform.GetChildTexts());
            }
        }
        return texts;
    }
}