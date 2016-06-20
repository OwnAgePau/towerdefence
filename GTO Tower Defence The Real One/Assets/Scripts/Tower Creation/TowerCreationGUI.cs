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
    public Slider cooldownSlider;

    [Header("Bullet Color")]
    public float r;
    public float g;
    public float b;
    public Color chosenColor;
    public Image colorDisplay;
    public Slider rSlider;
    public Slider gSlider;
    public Slider bSlider;

    [Header("Dropdown Content")]
    public Dropdown damageTypeDropdown;
    public Dropdown weaknessDropdown;
    public Dropdown strengthsDropdown;
    public Dropdown bulletDropdown;
    public Dropdown explosionDropdown;
    public Dropdown debufDropdown;

    public List<GameObject> bulletParticles = new List<GameObject>();
    public Text selectedBulletParticle;
    public ParticleSystem bulletParticleSystem;
    public List<GameObject> explosionParticles = new List<GameObject>();
    public Text selectedExplosionParticle;
    public List<GameObject> debufParticles = new List<GameObject>();
    public Text selectedDebufParticles;
    public GameObject[] towerColors;
    public GameObject debufScript;

    [Header("Enemy")]
    public Enemy enemy;

    // Use this for initialization
    void Start () {
        this.bullet = this.projectile.bullet.GetComponent<Bullet>();
        this.bulletParticleSystem = this.projectile.bullet.GetComponent<ParticleSystem>();
	    foreach(GameObject bulletOption in this.bulletParticles){
            this.bulletDropdown.options.Add(new Dropdown.OptionData(bulletOption.name));
        }
        foreach (GameObject explosionOption in this.explosionParticles){
            this.explosionDropdown.options.Add(new Dropdown.OptionData(explosionOption.name));
        }
        foreach(Transform debuf in GameObject.FindGameObjectWithTag("Debuf").GetComponentInChildren<Transform>()){
            this.debufDropdown.options.Add(new Dropdown.OptionData(debuf.gameObject.name));
        }
        this.chosenColor = new Color(this.r, this.g, this.b);
        this.selectedExplosionParticle.text = "None";
        this.selectedDebufParticles.text = "None";
        this.SetBulletExplosion();
        this.SetBulletDebuf();
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
            GameObject explosion = this.explosionParticles.Find(x => x.name == name);
            BulletHandler.instance.ChangeBulletExplosion(this.projectile.bullet.name, explosion);
        }
        else{
            BulletHandler.instance.ChangeBulletExplosion(this.projectile.bullet.name, null);
        }
    }
    public void SetBulletDebuf(){
        string name = this.selectedDebufParticles.text;
        for(int i = 0; i < this.enemy.debufs.Count; i++) {
            Debuf debuf = this.enemy.debufs[i];
            this.enemy.RemoveDebuf(debuf);
        }
        if (!name.Equals("None")) {
            //DebufScript debufScript = this.debufScript.transform.FindChild(name).GetComponent<DebufScript>();
            //BulletHandler.instance.ChangeBulletEDebuf(this.projectile.bullet.name, debufScript);
            this.projectile.debufScript = this.debufScript.transform.FindChild(name).GetComponent<DebufScript>();
        }
        else{
            //BulletHandler.instance.ChangeBulletEDebuf(this.projectile.bullet.name, null);
            this.projectile.debufScript = null;
        }
    }

    public void SetNewBullet(GameObject bulletObject){
        this.bullet = bulletObject.GetComponent<Bullet>();
    }

    public void SetColor() {
        this.r = this.rSlider.value;
        this.g = this.gSlider.value;
        this.b = this.bSlider.value;
        this.chosenColor = new Color(this.r, this.g, this.b);
        this.colorDisplay.color = this.chosenColor;
        //this.bulletParticleSystem.startColor = this.chosenColor;
        // Set the bullet to this color aswell
        BulletHandler.instance.ChangeBulletColor(this.projectile.bullet.name, this.chosenColor);
    }

    public void SetCooldown(){
        this.cooldown = this.cooldownSlider.value;
        this.tower.cooldown = this.cooldown;
    }

    public void CreateTower(){
        // Create new prefabs for the changes made, these towers should be saved somewhere to be able to load them in.
    }
}