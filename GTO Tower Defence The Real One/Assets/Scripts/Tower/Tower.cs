using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Tower : MonoBehaviour {

    [Header("General")]
    public string name;
    public float range = 110;
    private TowerRange rangeScript;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    // Upgrades and current level
    public int towerLevel = 0;
    public TowerUpgrade towerUpgradeObject;
    public Sprite towerImage;

    // TO DO : add a pool of bullets to use when shooting, 
    // the demon turrets are quite a problem as they will be spawning QUITE some bullets...
    // so the earlier solution might have been a better one. 
    // But that solution does not support multiple targets visually just yet.

    // Aura towers
    public bool isAuraTower = false;
    public ParticleManager particleManager;
    public int aspireCost;
    public int villagerCost = 1;

    // Target information
    [Header("Target Info")]
    public GameObject target;
    public Focus focus = Focus.FIRST;
    public float height = 8f;
    public int damage = 10;
    public DamageType type;
    public float slowAmount = 0.1f; // start with 10% slow
    public int projectiles = 1;
    public List<DamageType> strongAgainst = new List<DamageType>();

    // Shooting speed TO DO CHANGE THIS!!!!!!
    [Header("Speed")]
    public float cooldown = 0.5f; // How long to wait untill shooting again
    public float currentCooldown = 0.0f; // Time till shooting

    void Start(){
        this.rangeScript = this.GetComponent<TowerRange>();
        this.rangeScript.SetTowerRange(this.range);
        this.rangeScript.SetRangeSphereActivity(false);
    }

	// Update is called once per frame
	void FixedUpdate () {
        // Aura Tower effect
        if (this.isAuraTower){
            if (this.enemiesInRange.Count >= 1){
                // Enemies in range, start the particle accelerator!
                particleManager.PlayParticles();
            }
            else {
                // No more enemies, no need to accelerate the particles!
                particleManager.StopParticles();
            }

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
                if (this.type == DamageType.ICE){ // Change this to use a AOEeffect script that holds all the AOE information, debuf, damage whatever
                    float newSpeed = 1f - this.slowAmount;
                    GameObject debufs = GameObject.Find("Debufs");
                    DebufScript debuf = debufs.transform.FindChild("Slowed").gameObject.GetComponent<DebufScript>();
                    Debuf slowDebuf = debuf.CreateDebuf();
                    slowDebuf.slow = newSpeed;
                    enemy.ApplyEnemyDebuf(slowDebuf, false);
                    slowDebuf.SetEnemy(enemy);
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
        if(upgrade != null){
            this.towerLevel++;
        }
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
        }
    }

    public void RemoveEnemy(GameObject deadEnemy){
        if (deadEnemy != null){
            if (this.enemiesInRange.Contains(deadEnemy)){
                this.enemiesInRange.Remove(deadEnemy);
            }
        }
    }

    public void SetRangeSphereActivity(bool active) {
        this.rangeScript.SetRangeSphereActivity(active);
    }

    public void UpdateTowerRange(){
        this.rangeScript.SetTowerRange(this.range);
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