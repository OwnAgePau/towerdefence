using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Debuf {

    public string debufName;
    public DamageType typeOfDamage;

    public float damage;
    public float slow;

    public bool isTimed = true;
    public float damageTickInterval = 2f;
    public float timeUntillNextTick = 2f;

    public float debufTime = 0.0f;

    private Enemy enemy;

    public GameObject debufParticles;

    // Use this for initialization
    public Debuf(string name, float damage, float slow, bool isTimed, DamageType typeOfDamage, float length, GameObject debufParticles){
        this.debufName = name;
        this.damage = damage;
        this.slow = slow;
        this.isTimed = isTimed;
        this.debufTime = length;
        this.debufParticles = debufParticles;
        this.typeOfDamage = typeOfDamage;
    }

    public Debuf(string name, float damage, float slow, bool isTimed, DamageType typeOfDamage, float length, GameObject debufParticles, Enemy enemy) {
        this.debufName = name;
        this.damage = damage;
        this.slow = slow;
        this.isTimed = isTimed;
        this.debufTime = length;
        this.enemy = enemy;
        this.debufParticles = debufParticles;
        this.typeOfDamage = typeOfDamage;
    }

    public void SetEnemy(Enemy enemy){
        this.enemy = enemy;
        float health = enemy.health;
        this.damage = (health / 100) * this.damage;
    }
	
	// Update is called once per frame
	public void DoTick() {
        if (this.isTimed){
            if (this.debufTime > 0.0f){
                this.debufTime -= Time.deltaTime;
                if (this.timeUntillNextTick > 0.0f){ 
                    this.timeUntillNextTick -= Time.deltaTime;
                }
                else{
                    enemy.DamageEnemy(this.damage, this.typeOfDamage);
                    this.timeUntillNextTick = this.damageTickInterval;
                }
            }
            else{
                enemy.RemoveDebuf(this);
            }
        }
        else{
            // Effect is continious, AKA slow effect
            if(this.slow > 0.0f){
                enemy.currentSpeed = enemy.speed * slow;
            }

            if (this.debufTime > 0.0f){
                this.debufTime -= Time.deltaTime;
            }
            else{
                enemy.currentSpeed = enemy.speed;
                enemy.RemoveDebuf(this);
            }
        }
    }
}