using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Tower : MonoBehaviour {

    public List<GameObject> enemiesInRange = new List<GameObject>();
    // Upgrades and current level
    public int towerLevel = 0;
    public TowerUpgrade towerUpgradeObject;
    public Sprite towerImage;

    // Aura towers
    public bool isAuraTower = false;

    public int aspireCost;

    // Target information
    public GameObject target;
    public Focus focus = Focus.FIRST;
    public float height = 8f;
    public int damage = 10;
    public DamageType type;
    public float slowAmount = 0.1f; // start with 10% slow
    public int projectiles = 1;

    // Shooting speed TO DO CHANGE THIS!!!!!!
    public float cooldown = 0.5f; // How long to wait untill shooting again
    public float currentCooldown = 0.0f; // Time till shooting

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        //this.target = this.GetTarget();
        // Aura Tower effect
        if (this.isAuraTower){
            if (this.currentCooldown <= 0.1f){
                this.DealAuraDamage();
                this.currentCooldown = this.cooldown;
            }
            else{
                this.currentCooldown -= Time.deltaTime;
            }
        }
    }

    public void DealAuraDamage(){
        foreach (GameObject enemyObj in this.enemiesInRange){
            if (enemyObj != null){
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (this.type == DamageType.ICE){ // Slow
                    float newSpeed = 1f - this.slowAmount;
                    Debug.Log("Slow Amount : " + this.slowAmount + ", slow : " + newSpeed);
                    GameObject debufs = GameObject.Find("Debufs");
                    DebufScript debuf = debufs.transform.FindChild("Slowed").gameObject.GetComponent<DebufScript>();
                    Debuf slowDebuf = debuf.CreateDebuf();
                    slowDebuf.slow = newSpeed;
                    enemy.ApplyEnemyDebuf(slowDebuf, slowDebuf.debufTime);
                    slowDebuf.SetEnemy(enemy);
                    //enemy.currentSpeed = enemy.speed * slow;
                }
                else{ // Damage
                    enemy.DamageEnemy(this.damage, this.type);
                }
            }
        }
    }

    public Upgrade getUpgrade(){
        TowerUpgrade upgradeScript = this.towerUpgradeObject;
        return upgradeScript.GetNextUpgrade(this.towerLevel);
    }

    public Upgrade UpgradeTower(){
        // Check upgrade cost
        TowerUpgrade upgradeScript = this.towerUpgradeObject;
        Upgrade upgrade = upgradeScript.UpgradeTower(this.towerLevel);
        this.towerLevel++;
        return upgrade;
    }

    public GameObject GetTarget(List<GameObject> targets){
        List<GameObject> viableTargets = new List<GameObject>();
        GameObject target = null;
        for (int i = 0; i < this.enemiesInRange.Count; i++) {
            // Check of een enemy niet in de range zit
            if (this.enemiesInRange[i] != null && !targets.Contains(this.enemiesInRange[i])){
                viableTargets.Add(this.enemiesInRange[i]);
            }
        }
        if(viableTargets.Count > 0){
            if (this.focus.Equals(Focus.FIRST)){
                target = viableTargets[0];
                /*foreach (GameObject enemy in this.enemiesInRange){
                    if (enemy != null){
                        target = enemy;
                    }
                }*/
            }
            else if (this.focus.Equals(Focus.LAST)){
                target = viableTargets[viableTargets.Count - 1];
            }
        }
        if(target != null){
            //Debug.Log(target.name);
        }
        return target;  
    }

    void OnTriggerEnter(Collider col){
        Enemy enemy = col.GetComponent<Enemy>();
        if (enemy != null) {
            this.enemiesInRange.Add(col.gameObject);
        }
    }

    void OnTriggerExit(Collider col){
        Enemy enemy = col.GetComponent<Enemy>();
        if (enemy != null){
            this.RemoveEnemy(col.gameObject);
            /*if(this.type == DamageType.ICE){
                enemy.currentSpeed = enemy.speed;
            }*/
        }
    }

    public void RemoveEnemy(GameObject deadEnemy){
        if (deadEnemy != null){
            if (this.enemiesInRange.Contains(deadEnemy)){
                this.enemiesInRange.Remove(deadEnemy);
                /*if (this.target != null){
                    if (this.target.Equals(deadEnemy)){
                        //this.target = this.GetTarget();
                    }
                }*/
            }
        }
    }
}

public enum DamageType{
    MAGE,
    FIRE,
    ICE,
    POISON,
    EARTH,
    DEMON,
    NONE
}

public enum Focus{
    FIRST,
    LAST,
    STRONGEST,
    FASTEST
}