using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerProjectile : MonoBehaviour {

    private DebufScript debufScript;
    public GameObject debuf;

    private GameObject target;
    private Tower towerScript;

    private List<GameObject> targets = new List<GameObject>();

    public GameObject bulletParent;
    public GameObject bullet;

    private AudioScript projectileSounds;
    public AudioSource source;

    public List<GameObject> bullets = new List<GameObject>();

    // Use this for initialization
    void Start () {
        this.bulletParent = GameObject.FindGameObjectWithTag("bulletParent");
        this.projectileSounds = this.gameObject.GetComponent<AudioScript>();
        this.source = this.gameObject.GetComponent<AudioSource>();
        
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
            // Was this.target = towerScript.GetTarget();
            Vector3 bulletStartPos = new Vector3(this.transform.position.x, this.transform.position.y + (towerScript.height / 2), this.transform.position.z);
            // bulletStartPos, this.transform.rotation
            //GameObject bullet = BulletHandler.instance.GetInactiveBullet(this.bullet.name);
            GameObject bullet = (GameObject)Instantiate(this.bullet, bulletStartPos, this.transform.rotation);
            bullet.transform.parent = this.bulletParent.transform;
            bullet.name = this.bullet.name;
            bullet.transform.position = bulletStartPos;
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.bulletStartPos = bulletStartPos;
            bulletScript.SetDestination(target);
            bulletScript.SetFiredFrom(towerScript);
            if (this.debuf != null)
            {
                bulletScript.debuf = this.debufScript.CreateDebuf();
            }
        }
        
    }
}