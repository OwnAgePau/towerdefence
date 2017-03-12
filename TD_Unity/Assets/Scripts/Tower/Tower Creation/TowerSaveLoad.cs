using UnityEngine;
using System.Collections;

public class TowerSaveLoad : MonoBehaviour {

    public static TowerSaveLoad instance;

    void Awake() {
        instance = this;
    }

    public void LoadTower(string name) {

    }

    public void SaveTower(GameObject tower) {

    }
}