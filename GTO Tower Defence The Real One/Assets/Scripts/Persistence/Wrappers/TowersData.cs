using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class TowersData{
    // A list of towers with tower data (location, damage, level)
    public List<TowerData> towerList = new List<TowerData>();

    public TowersData(){}

    public TowersData(List<TowerData> towers){
        this.towerList = towers;
    }

    public void SetTowerList(List<TowerData> towers){
        this.towerList = towers;
    }
}

[Serializable]
public class TowerData{
    public DamageType damageType;

    public int gridX;
    public int gridZ;

    public int level;

    public int damage;
    public int speed;
    public int nrProjectiles;
    public float slowPercentage;

    public float currentCooldown;

    public Focus focusType;

    public TowerData(DamageType damageType, int gridX, int gridZ, int level, int damage, int speed, int nrProjectiles, float slowPercentage, float cooldown, Focus focusType){
        this.damageType = damageType;
        this.gridX = gridX;
        this.gridZ = gridZ;
        this.level = level;
        this.damage = damage;
        this.speed = speed;
        this.nrProjectiles = nrProjectiles;
        this.slowPercentage = slowPercentage;
        this.currentCooldown = cooldown;
        this.focusType = focusType;
    }
}
