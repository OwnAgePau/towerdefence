using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class BulletsData {

    public List<BulletData> bulletList = new List<BulletData>();

    public BulletsData(List<BulletData> bulletsData){
        this.bulletList = bulletsData;
    }

    public BulletsData(){}

    public void SetBulletsList(List<BulletData> bulletsData){
        this.bulletList = bulletsData;
    }
}

[Serializable]
public class BulletData{

    public string name;
    public int speed;
    public float deathTimer;

    public int firedFromPosX;
    public int firedFromPosZ;
    public PositionWrapper destinationPosition;
    public PositionWrapper bulletPosition; 

    public BulletData(string name, int speed, float deathTimer, int firedFromPosX, int firedFromPosZ, 
        PositionWrapper destPosition, PositionWrapper bulletPosition){
        this.name = name;
        this.speed = speed;
        this.deathTimer = deathTimer;
        this.firedFromPosX = firedFromPosX;
        this.firedFromPosZ = firedFromPosZ;
        this.destinationPosition = destPosition;
        this.bulletPosition = bulletPosition;
    }
}

[Serializable]
public class PositionWrapper{
    public float x;
    public float y;
    public float z;

    public PositionWrapper(float x, float y, float z){
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public PositionWrapper(Vector3 position){
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }
}
