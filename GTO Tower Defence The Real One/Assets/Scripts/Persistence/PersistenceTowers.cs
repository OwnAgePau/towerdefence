using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistenceTowers : MonoBehaviour {

    public static PersistenceTowers instance;

    void Awake(){
        instance = this;
    }

    public TowersData CreateTowersData(){
        TowersData towersData = new TowersData();
        List<TowerData> towerList = new List<TowerData>();
        GameObject[] towers = GameObject.FindGameObjectsWithTag("tower");
        foreach (GameObject tower in towers){
            Tower towerScript = tower.GetComponent<Tower>();
            Tile tile = GridPathfinding.instance.GetTileWithTower(tower);
            TowerData towerData = new TowerData(towerScript.type, tile.x, tile.z, towerScript.towerLevel,
                towerScript.damage, towerScript.cooldown, towerScript.projectiles, towerScript.slowAmount, towerScript.currentCooldown, towerScript.focus);
            towerList.Add(towerData);
        }
        towersData.SetTowerList(towerList);
        return towersData;
    }

    public void LoadTowersData(TowersData data){
        foreach (TowerData towerData in data.towerList){
            Tile tile = GridPathfinding.instance.grid[towerData.gridX][towerData.gridZ];
            GameObject prefab = this.GetTowerPrefab(towerData.damageType);
            float x = GridPathfinding.instance.startPosX + (tile.x * GridPathfinding.instance.tileSize);
            float z = GridPathfinding.instance.startPosZ + (tile.z * GridPathfinding.instance.tileSize);
            GameObject towerInstance = PlaceTower.instance.PlaceNewTower(prefab, tile, x, 0.5f, z);
            Tower tower = towerInstance.GetComponent<Tower>();
            tower.towerLevel = towerData.level;
            tower.damage = towerData.damage;
            tower.cooldown = towerData.cooldown;
            tower.currentCooldown = towerData.currentCooldown;
            tower.projectiles = towerData.nrProjectiles;
            tower.slowAmount = towerData.slowPercentage;
            tower.focus = towerData.focusType;
        }
    }

    public GameObject GetTowerPrefab(DamageType type){
        GameObject[] towers = SelectTower.instance.towers;
        foreach (GameObject tower in towers){
            Tower towerScript = tower.GetComponent<Tower>();
            if (towerScript.type.Equals(type)){
                return tower;
            }
        }
        return null;
    }

    public BulletsData CreateBulletsData(){
        BulletsData bulletsData = new BulletsData();
        List<BulletData> bulletDataList = new List<BulletData>();
        GameObject bulletParent = GameObject.FindGameObjectWithTag("bulletParent");
        Bullet[] bulletScripts = bulletParent.GetComponentsInChildren<Bullet>();
        foreach (Bullet bullet in bulletScripts) {
            Tower firedFrom = bullet.GetFiredFrom();
            Tile tile = GridPathfinding.instance.GetTileWithTower(firedFrom.gameObject);
            PositionWrapper destination = new PositionWrapper(bullet.destination.transform.position);
            PositionWrapper bulletPosition = new PositionWrapper(bullet.transform.position);
            BulletData bulletData = new BulletData(bullet.gameObject.name, bullet.speed, bullet.deathTimer, tile.x, tile.z, destination, bulletPosition);
            bulletDataList.Add(bulletData);
        }
        bulletsData.SetBulletsList(bulletDataList);
        return bulletsData;
    }

    public void LoadBulletsData(BulletsData data){
        foreach(BulletData bulletData in data.bulletList){
            Vector3 bulletPosition = new Vector3(bulletData.bulletPosition.x, bulletData.bulletPosition.y, bulletData.bulletPosition.z);
            GameObject bulletObj = (GameObject)Instantiate(Resources.Load(bulletData.name), bulletPosition, this.transform.rotation);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            bulletScript.deathTimer = bulletData.deathTimer;
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject targetEnemy = null;
            foreach(GameObject enemy in enemies){
                Vector3 enemyPos = enemy.transform.position;
                if (enemyPos.x.Equals(bulletData.destinationPosition.x)){
                    if (enemyPos.y.Equals(bulletData.destinationPosition.y)){
                        if (enemyPos.z.Equals(bulletData.destinationPosition.z)){
                            targetEnemy = enemy;
                        }
                    }
                }
            }
            bulletScript.destination = targetEnemy;
            bulletScript.speed = bulletData.speed;
            Tile tile = GridPathfinding.instance.grid[bulletData.firedFromPosX][bulletData.firedFromPosZ];
            if (tile.tower != null) {
                bulletScript.SetFiredFrom(tile.tower.GetComponent<Tower>());
            }
        }
    }
}
