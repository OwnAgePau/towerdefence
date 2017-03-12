using UnityEngine;
using UnityEditor;
using System.Collections;

public class BulletSaveLoad : MonoBehaviour {

    public static BulletSaveLoad instance;

    private int assetNr = 0;
    private string latestAssetName = "";

    private int prefabNr = 0;
    private string latestPrefabName = "";

    void Awake() {
        instance = this;
    }

    public void CreateBullet(string name, Material mat) {
        string[] search_results = GetBulletPrefabs(name);
        Debug.Log("Bullet prefab founds : " + search_results.Length + ", with name : " + name);
        
        string newName = name + "(" + prefabNr + ")";
        if (search_results.Length > 0) {
            GameObject prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(search_results[0]);
            prefab.GetComponent<ParticleSystemRenderer>().material = mat;       
            while (PrefabWithNameExists(newName)) {
                assetNr++;
                newName = name + "(" + prefabNr + ")";
            }
            prefab.name = newName;
            Debug.Log(search_results[0] + ", " + prefab.name);

            PrefabUtility.CreatePrefab(prefab_path + newName + ".prefab", prefab);
            //AssetDatabase.CreateAsset(prefab, prefab_path + newName + ".prefab");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            prefabNr++;
        }

        latestPrefabName = name;
    }

    public void SaveBullet(GameObject bullet) {
        latestAssetName = "";
        latestPrefabName = "";
    }

    public void LoadParticleTexture(string name) {

    }

    /// <summary>
    /// Save a new material and give a new name
    /// </summary>
    public Material SaveParticleTexture(Material mat, string fileName) {
        Material newMat = Instantiate(mat);
        newMat.name = mat.name;
        string name = newMat.name + "(" + assetNr + ")";
        if (!name.Equals(latestAssetName) && !latestAssetName.Equals("")) {
            assetNr = 0;
            name = newMat.name + "(" + assetNr + ")";
            AssetDatabase.DeleteAsset(fileName + latestAssetName + ".mat");
        }
        while (MaterialWithNameExists(name)) {
            assetNr++;
            name = newMat.name + "(" + assetNr + ")";
        }
        latestAssetName = name;
        AssetDatabase.CreateAsset(newMat, fileName + name + ".mat");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        assetNr++;
        return newMat;
    }

    bool MaterialWithNameExists(string name) {
        string[] search_results = this.GetAllCustomMaterials();
        Material mat = null;
        for(int i = 0; i < search_results.Length; i++) {
            mat = (Material)AssetDatabase.LoadMainAssetAtPath(search_results[i]);
            if (mat.name.Equals(name)) {
                return true;
            }
        }
        return false;
    }


    bool PrefabWithNameExists(string name) {
        string[] search_results = this.GetAllBulletPrefabs();
        GameObject prefab = null;
        for (int i = 0; i < search_results.Length; i++) {
            prefab = (GameObject)AssetDatabase.LoadMainAssetAtPath(search_results[i]);
            if (prefab.name.Equals(name)) {
                return true;
            }
        }
        return false;
    }

    public const string material_path = "Assets/Resources/CustomCreations/Bullets/Materials/";
    public const string prefab_path = "Assets/Resources/CustomCreations/Bullets/";

    public string[] GetAllCustomMaterials() {
        return System.IO.Directory.GetFiles("Assets/Resources/CustomCreations/Bullets/Materials/", "*.mat", System.IO.SearchOption.AllDirectories);
    }

    public string[] GetBulletPrefabs(string name) {
        return System.IO.Directory.GetFiles("Assets/Resources/CustomCreations/Bullets/", name + ".prefab", System.IO.SearchOption.AllDirectories);
    }

    public string[] GetAllBulletPrefabs() {
        return System.IO.Directory.GetFiles("Assets/Resources/CustomCreations/Bullets/", "*.prefab", System.IO.SearchOption.AllDirectories);
    }
}