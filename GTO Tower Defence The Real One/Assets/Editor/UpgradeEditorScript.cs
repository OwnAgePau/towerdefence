using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TowerUpgrade))]
public class UpgradeEditorScript : Editor {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void OnInspectorGUI(){
        // This is where the magic happens.
        TowerUpgrade towerUpgrade = (TowerUpgrade)target;
        towerUpgrade.hasEndlessUpgrades = EditorGUILayout.Toggle("Has Endless Upgrades", towerUpgrade.hasEndlessUpgrades);
        towerUpgrade.maxProjectiles = EditorGUILayout.IntField("Max Projectiles", towerUpgrade.maxProjectiles);
        towerUpgrade.tower = EditorGUILayout.ObjectField("Tower Script", towerUpgrade.tower, typeof(Tower), true) as Tower;
        towerUpgrade.hoverInfo = EditorGUILayout.ObjectField("Hover Info", towerUpgrade.hoverInfo, typeof(HoverInfo), true) as HoverInfo;
        serializedObject.FindProperty("tower");
        serializedObject.Update();
        EditorList.Show(serializedObject.FindProperty("upgrades"));
        //EditorGUILayout.PropertyField(serializedObject.FindProperty("upgrades"), true);
        serializedObject.ApplyModifiedProperties();
    }
}
