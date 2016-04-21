using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class PlayerData {

    public int villagers;
    public int aspirePoints;
    public int power; // Total damage
    public int score;
    public int enemiesKilled;

    public int lives;

    public int enemyHealth;
    public int enemyScore;

    public PlayerData(int villagers, int aspirePoints, int power, int score, int enemiesKilled, int lives, int enemyHealth, int enemyScore){
        this.villagers = villagers;
        this.aspirePoints = aspirePoints;
        this.power = power;
        this.score = score;
        this.enemiesKilled = enemiesKilled;
        this.lives = lives;
        this.enemyHealth = enemyHealth;
        this.enemyScore = enemyScore;
    }
}
