using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class EnemiesData {

    public List<EnemyData> enemyList = new List<EnemyData>();

    public EnemiesData(){}

    public EnemiesData(List<EnemyData> enemies){
        this.enemyList = enemies;
    }

    public void SetEnemyList(List<EnemyData> enemies){
        this.enemyList = enemies;
    }
}

[Serializable]
public class EnemyData{

    public string name;

    public int fullHealth;
    public int health;
    public int score;
    public float currentSpeed;

    public float timeTillDeath;
    public bool isDead;
    public bool deathScoreRecieved;
    public bool hasReachedEnding;

    // Debufs
    public List<EnemyDebufData> debufs = new List<EnemyDebufData>();
    public List<EnemyDebufData> debufsToRemove = new List<EnemyDebufData>();

    public float x;
    public float y;
    public float z;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    public int gridDestinationX;
    public int gridDestinationZ;
    public int currentPoint;

    public EnemyData(string name, int fullHealth, int health, int score, float currentSpeed, float timeTillDeath, bool isDead, bool deathScoreRecieved, bool hasReachedEnding, 
        List<EnemyDebufData> debufs, List<EnemyDebufData> debufsToRemove, float x, float y, float z, float rotationX, float rotationY, float rotationZ,
        int gridDestinationX, int gridDestinationZ, int currentPoint){
        this.name = name;
        this.fullHealth = fullHealth;
        this.health = health;
        this.score = score;
        this.currentSpeed = currentSpeed;
        this.timeTillDeath = timeTillDeath;
        this.isDead = isDead;
        this.deathScoreRecieved = deathScoreRecieved;
        this.hasReachedEnding = hasReachedEnding;
        this.debufs = debufs;
        this.debufsToRemove = debufsToRemove;
        this.x = x;
        this.y = y;
        this.z = z;
        this.rotationX = rotationX;
        this.rotationY = rotationY;
        this.rotationZ = rotationZ;
        this.gridDestinationX = gridDestinationX;
        this.gridDestinationZ = gridDestinationZ;
        this.currentPoint = currentPoint;
    }
}

[Serializable]
public class EnemyDebufData{
    public string debufName;

    public float timeUntillNextTick = 2f;
    public float debufTime = 0.0f;
    public float slowAmount;

    public EnemyDebufData(string debufName, float timeUntillNextTick, float debufTime, float slowAmount){
        this.debufName = debufName;
        this.timeUntillNextTick = timeUntillNextTick;
        this.debufTime = debufTime;
        this.slowAmount = slowAmount;
    }
}