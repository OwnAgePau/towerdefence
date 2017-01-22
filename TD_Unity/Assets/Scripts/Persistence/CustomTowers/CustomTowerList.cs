using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class CustomTowerList { // Not sure if this is needed

    private List<CustomTower> customTowers = new List<CustomTower>();

    public CustomTowerList(List<CustomTower> towers){
        this.customTowers = towers;
    }

    public List<CustomTower> GetTowers(){
        return this.customTowers;
    }
}

[Serializable]
public class CustomTower
{
    // Prefab
    // Prefab Save Slot
    //public int towerPrefabSlot = 0;

    // Towername
    public string towerName;

    // Damage
    public int damage;
    // Aspirecost
    public int aspireCost;
    // Range
    public float range;
    // Is AOE
    public bool isAOE;
    // Whether or not the aoe tower is slowing enemies
    public bool isSlow;

    // Damage type
    public DamageType towerType;
    // Strong against
    public DamageType bonusDamage;

    public string debufName;

    // bullet
    // Bullets also need prefab save slots
    public CustomBullet customBullet;

    /// <summary>
    /// Create a custom tower object
    /// </summary>
    /// <param name="name">Name of the tower</param>
    /// <param name="damage">The towers damage</param>
    /// <param name="aspireCost">the aspire cost of the tower</param>
    /// <param name="range">The range of the tower</param>
    /// <param name="isAOE">Whether or not the tower is an AOE tower</param>
    /// <param name="towerType">the damagetype of this tower</param>
    /// <param name="bonusDamage">the type of enemy this tower will deal bonus damage to</param>
    /// <param name="debufName">the name of the debuf of this tower</param>
    /// <param name="bullet">the bullet object that contains info about the bullet of this tower</param>
    public CustomTower(string name, int damage, int aspireCost, float range, bool isAOE, bool isSlow, DamageType towerType, DamageType bonusDamage, string debufName, CustomBullet bullet){
        this.towerName = name;
        this.damage = damage;
        this.aspireCost = aspireCost;
        this.range = range;
        this.isAOE = isAOE;
        this.isSlow = isSlow;
        this.towerType = towerType;
        this.bonusDamage = bonusDamage;
        this.debufName = debufName;
        this.customBullet = bullet;
    }

    public CustomTower() { }
}

[Serializable]
public class CustomBullet {

    public string name;

    public string audioClip;

    public float cooldown;

    public Color color; // Ik denk dat color niet serializable is, in dit geval gebruik de 3 floats

    // explosion
    // Explosions don't need a prefab slot because we don't change them, actually, we do change the sound of the explosion so probably YES
    public CustomExplosion customExplosion;

    /// <summary>
    /// The object that contains information about a custom made bullet
    /// </summary>
    /// <param name="name">The name of this bullet</param>
    /// <param name="audioClip">The name of the sound of this bullet</param>
    /// <param name="color">The color of this bullet</param>
    /// <param name="explosion">the explosion object that contains info about the explosion of this tower</param>
    public CustomBullet(string name, string audioClip, float cooldown, Color color, CustomExplosion explosion){
        this.name = name;
        this.audioClip = audioClip;
        this.cooldown = cooldown;
        this.color = color;
        this.customExplosion = explosion;
    }

    public CustomBullet() { }
}

[Serializable]
public class CustomExplosion{

    public string name;

    //xpublic string particles;

    public string audioClip;

    /// <summary>
    /// This object contains information about a custom made explosion object
    /// </summary>
    /// <param name="name">The name of the explosion object</param>
    /// <param name="audioClip">the audioClip</param>
    public CustomExplosion(string name, string audioClip){
        this.name = name;
        this.audioClip = audioClip;
    }

    public CustomExplosion() { }
}