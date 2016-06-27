using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerProjectile : MonoBehaviour {

    public DebufScript debufScript;
    public GameObject debuf;

    private GameObject target;
    private Tower towerScript;

    private List<GameObject> targets = new List<GameObject>();

    public GameObject bulletParent;
    public GameObject bullet;

    private AudioScript projectileSounds;

    public List<GameObject> bullets = new List<GameObject>(); // A tower Projectile should have a pool of bullets avaible to choose from, to keep from destroying creating and just recycling bullets.

    // Use this for initialization
    void Start () {
        this.bulletParent = GameObject.FindGameObjectWithTag("bulletParent");
        this.projectileSounds = this.gameObject.GetComponent<AudioScript>();
        
        this.towerScript = this.GetComponent<Tower>();
        if (this.debuf != null){
            debufScript = this.debuf.GetComponent<DebufScript>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (towerScript.enemiesInRange.Count > 0){
            if (towerScript.currentCooldown < 0.1f){
                // For elke projectile
                this.targets = new List<GameObject>();
                for (int i = 0; i < towerScript.projectiles; i++){
                    // Get new target for each shot that isnt the target of another shot
                    this.Shoot(i);
                }
                towerScript.currentCooldown = towerScript.cooldown;
            }
            else{
                towerScript.currentCooldown -= Time.deltaTime;
            }
        }
    }

    void Shoot(int targetNr){
        this.projectileSounds.PlaySound();
        GameObject target = towerScript.GetTarget(this.targets);
        if(target != null){
            this.targets.Add(target);
            GameObject bulletObj = BulletHandler.instance.GetInactiveBullet(this.bullet.name);
            if (bulletObj != null) {
                Vector3 bulletStartPos = new Vector3(this.transform.position.x, this.transform.position.y + (towerScript.height / 2), this.transform.position.z);
                bulletObj.transform.position = bulletStartPos;
                bulletObj.name = this.bullet.name;
                bulletObj.transform.position = bulletStartPos;
                Bullet bulletScript = bulletObj.GetComponent<Bullet>();
                bulletScript.bulletStartPos = bulletStartPos;
                if (this.debufScript != null){
                    bulletScript.debuf = this.debufScript.CreateDebuf(this.towerScript.towerLevel);
                    Debug.Log(this.debufScript.debufName + ", " + bulletScript.debuf.debufName);
                    bulletScript.debufName = this.debufScript.debufName;
                }
                else{
                    bulletScript.debufName = "";
                }
                bulletScript.SetDestination(target);
                bulletScript.SetFiredFrom(towerScript);
            }            
        }
        
    }
}