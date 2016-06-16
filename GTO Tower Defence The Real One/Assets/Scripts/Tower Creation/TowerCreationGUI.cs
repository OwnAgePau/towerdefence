using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TowerCreationGUI : MonoBehaviour {

    [Header("Tower Info")]
    public Tower tower;
    public Text nameText;
    public int damageType;
    public int weakness;
    public int strengths;
    public int damage;
    public float range;
    public int aspireCost;
    public bool isAOE;
    public bool isSlow;
    public int towerColor;

    [Header("Bullet Info")]
    public TowerProjectile projectile;
    public Bullet bullet;
    public int nrOfProjectiles;
    public int bulletParticlesIndex;
    public int explosionParticlesIndex;
    public int debufParticlesIndex;
    public float cooldown;

    public float r;
    public float g;
    public float b;
    public Color chosenColor;

    [Header("Dropdown Content")]
    public Dropdown damageTypeDropdown;
    public Dropdown weaknessDropdown;
    public Dropdown strengthsDropdown;
    public Dropdown bulletDropdown;
    public Dropdown explosionDropdown;

    public List<GameObject> bulletParticles = new List<GameObject>();
    public Text selectedBulletParticle;
    public List<GameObject> explosionParticles = new List<GameObject>();
    public Text selectedExplosionParticle;
    public List<GameObject> debufParticles = new List<GameObject>();
    public Text selectedDebufParticles;
    public GameObject[] towerColors;

    // Use this for initialization
    void Start () {
        this.bullet = this.projectile.bullet.GetComponent<Bullet>();
	    foreach(GameObject bulletOption in this.bulletParticles){
            this.bulletDropdown.options.Add(new Dropdown.OptionData(bulletOption.name));
        }
        foreach (GameObject explosionOption in this.explosionParticles){
            this.explosionDropdown.options.Add(new Dropdown.OptionData(explosionOption.name));
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetBulletParticle() {
        // optionNr = this.bulletDropdown
        string name = this.selectedBulletParticle.text;
        this.projectile.bullet = this.bulletParticles.Find(x => x.name == name);
    }

    public void SetBulletExplosion(){
        string name = this.selectedExplosionParticle.text;
        if (!name.Equals("None")){
            this.bullet.explosion = this.explosionParticles.Find(x => x.name == name);
        }
    }
    public void SetBulletDebuf(){
        string name = this.selectedDebufParticles.text;
        if (!name.Equals("None")){
            //this.bullet.explosion = this.explosionParticles.Find(x => x.name == name);
        }
    }

    public void SetNewBullet(GameObject bulletObject){
        this.bullet = bulletObject.GetComponent<Bullet>();
    }

    public void CreateTower(){
        // Create new prefabs for the changes made, these towers should be saved somewhere to be able to load them in.
    }
}