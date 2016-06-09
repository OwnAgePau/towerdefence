using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistenceEnemies : MonoBehaviour{

    public static PersistenceEnemies instance;

    void Awake(){
        instance = this;
    }

    public EnemiesData CreateEnemiesData(){
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        EnemiesData enemiesData = new EnemiesData();
        List<EnemyData> enemyList = new List<EnemyData>();
        foreach (GameObject enemy in enemies){
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            EnemyMovement enemyMoveScript = enemy.GetComponent<EnemyMovement>();
            Tile destination = GridPathfinding.instance.GetTile(enemyMoveScript.destination);
            List<EnemyDebufData> debufsList = new List<EnemyDebufData>();
            foreach (Debuf debuf in enemyScript.debufs){
                EnemyDebufData debufData = new EnemyDebufData(debuf.debufName, debuf.damage, debuf.timeUntillNextTick, debuf.debufTime, debuf.slow);
                debufsList.Add(debufData);
            }
            List<EnemyDebufData> debufsToRemoveList = new List<EnemyDebufData>();
            foreach (Debuf debuf in enemyScript.GetDebufsToRemove()){
                EnemyDebufData debufData = new EnemyDebufData(debuf.debufName, debuf.damage, debuf.timeUntillNextTick, debuf.debufTime, debuf.slow);
                debufsToRemoveList.Add(debufData);
            }
            EnemyData enemyData = new EnemyData(enemy.name, enemyScript.fullHealth, enemyScript.health, enemyScript.score, enemyScript.currentSpeed, enemyScript.timeTillDeath,
                enemyScript.isDead, enemyScript.deathScoreRecieved, enemyMoveScript.hasReachedEnding, debufsList, debufsToRemoveList,
                enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z,
                enemy.transform.rotation.x, enemy.transform.rotation.y, enemy.transform.rotation.z,
                destination.x, destination.z, enemyMoveScript.GetCurrentPoint());
            enemyList.Add(enemyData);
        }
        enemiesData.SetEnemyList(enemyList);
        return enemiesData;
    }

    public void LoadEnemiesData(EnemiesData data){
        foreach (EnemyData enemyData in data.enemyList){
            GameObject prefab = this.GetEnemyPrefab(enemyData.name);
            Vector3 position = new Vector3(enemyData.x, enemyData.y, enemyData.z);
            Quaternion rotation = new Quaternion(enemyData.rotationX, enemyData.rotationY, enemyData.rotationZ, 0);
            GameObject enemy = (GameObject)Instantiate(prefab, position, rotation);
            enemy.name = prefab.name;
            GameObject parent = GameObject.FindGameObjectWithTag("enemyParent");
            enemy.transform.parent = parent.transform;
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.fullHealth = enemyData.fullHealth;
            enemyScript.health = enemyData.health;
            enemyScript.score = enemyData.score;
            enemyScript.currentSpeed = enemyData.currentSpeed;
            enemyScript.timeTillDeath = enemyData.timeTillDeath;
            enemyScript.isDead = enemyData.isDead;
            enemyScript.deathScoreRecieved = enemyData.deathScoreRecieved;
            List<Debuf> debufsList = new List<Debuf>();
            foreach (EnemyDebufData debufData in enemyData.debufs){
                Debuf debuf = GameObject.Find(debufData.debufName).GetComponent<DebufScript>().CreateDebuf();
                debuf.SetEnemy(enemyScript);
                debuf.damage = debufData.debufDamage;
                debuf.slow = debufData.slowAmount;
                debuf.timeUntillNextTick = debufData.timeUntillNextTick;
                debuf.debufTime = debufData.debufTime;
                debufsList.Add(debuf);
            }
            enemyScript.debufs = debufsList;
            List<Debuf> debufsToRemoveList = new List<Debuf>();
            foreach (EnemyDebufData debufData in enemyData.debufsToRemove){
                Debuf debuf = GameObject.Find(debufData.debufName).GetComponent<DebufScript>().CreateDebuf();
                debuf.SetEnemy(enemyScript);
                debuf.slow = debufData.slowAmount;
                debuf.damage = debufData.debufDamage;
                debuf.timeUntillNextTick = debufData.timeUntillNextTick;
                debuf.debufTime = debufData.debufTime;
                debufsList.Add(debuf);
            }
            enemyScript.SetDebufsToRemove(debufsToRemoveList);
            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            movement.SetCurrentPoint(enemyData.currentPoint);
            movement.hasReachedEnding = enemyData.hasReachedEnding;
        }
    }

    public GameObject GetEnemyPrefab(string name){
        /*Waves waves = Waves.instance;
          foreach (GameObject enemy in waves.enemies){
            if (enemy.name.Equals(name)){
                return enemy;
            }
        }*/
        return Resources.Load(name) as GameObject;
    }
}