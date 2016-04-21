using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Wave : MonoBehaviour {

    public int amountOfEnemies;
    public string typeOfEnemy = "";
    public GameObject enemyPrefab;
    public List<GameObject> enemies = new List<GameObject>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void SpawnEnemyInWave(GameObject enemy){
        this.enemies.Add(enemy);
    }

    public void KillEnemy(GameObject enemy){
        this.enemies.Remove(enemy);
    }
}
