using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace P_Lerper {
    public static class ExtensionMethods {

        public static List<GameObject> GetChildren(this Transform trans) {
            List<GameObject> gameObjects = new List<GameObject>();
            Transform childTransform = null;
            for (int i = 0; i < trans.childCount; i++) {
                childTransform = trans.GetChild(i);
                gameObjects.Add(childTransform.gameObject);
                if (childTransform.childCount > 0) {
                    gameObjects.AddRange(childTransform.GetChildren());
                }
            }
            return gameObjects;
        }

        public static List<Material> GetChildMaterials(this Transform trans) {
            List<Material> materials = new List<Material>();
            Transform childTransform = null;
            MeshRenderer renderer = null;
            for (int i = 0; i < trans.childCount; i++) {
                childTransform = trans.GetChild(i);
                renderer = childTransform.GetComponent<MeshRenderer>();
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
            Transform childTransform = null;
            Image image = null;
            for (int i = 0; i < trans.childCount; i++) {
                childTransform = trans.GetChild(i);
                image = childTransform.GetComponent<Image>();
                if (image != null) {
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
            Transform childTransform = null;
            Text text = null;
            for (int i = 0; i < trans.childCount; i++) {
                childTransform = trans.GetChild(i);
                text = childTransform.GetComponent<Text>();
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
}