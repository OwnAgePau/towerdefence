using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TowerCreationGUI : MonoBehaviour {

    [Header("Main Camera")]
    public Vector3 cameraStartPos;

    [Header("Tower Info")]
    private bool towerRangeActive = false;
    private float timeTillRangeInactive = 1f;
    private float maxActiveTime = 1f;
    public Tower tower;
    public Text nameText;
    public CustomTower customTower; // This object can be use to store all the data from below
    public int towerColor;
    public Text damageText;
    public Slider damageSlider;
    public Text rangeText;
    public Slider rangeSlider;
    public Text towerCost;

    [Header("Bullet Info")]
    public TowerProjectile projectile;
    public Bullet bullet;
    public int nrOfProjectiles;
    public int bulletParticlesIndex;
    public int explosionParticlesIndex;
    public int debufParticlesIndex;
    public float cooldown;
    public Slider cooldownSlider;
    public Slider aoeCooldownSlider;

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

    [Header("Sounds")]
    public ExplosionHandler explHandler;
    public List<AudioClip> bulletSounds;
    public List<AudioClip> AOEsounds;
    public List<AudioClip> explosionSounds;
    public AudioScript towerAudio;
    public AudioScript bulletAudio;
    public Dropdown towerTabSounds;
    public Dropdown bulletTabSounds;

    [Header("Tabs")]
    public GameObject bulletTab;
    public GameObject aoeTab;

    [Header("AOE")]
    public GameObject[] particleDamageObjects;
    public GameObject[] particleSlowObjects;
    public Dropdown aoeParticlesDropdown;
    public ParticleSystem aoeParticleSystem;

    // Use this for initialization
    void Start () {
        this.customTower = new CustomTower();
        this.customTower.customBullet = new CustomBullet();
        this.customTower.customBullet.customExplosion = new CustomExplosion();
        this.cameraStartPos = Camera.main.transform.position;
        this.bullet = this.projectile.bullet.GetComponent<Bullet>();
        if(this.bullet.explosion != null){
            this.bulletAudio = this.bullet.explosion.GetComponent<AudioScript>();
        }
        this.bulletParticleSystem = this.projectile.bullet.GetComponent<ParticleSystem>();
	    foreach(GameObject bulletOption in this.bulletParticles){
            this.bulletDropdown.options.Add(new Dropdown.OptionData(bulletOption.name));
        }
        foreach (GameObject explosionOption in this.explosionParticles){
            this.explosionDropdown.options.Add(new Dropdown.OptionData(explosionOption.name));
        }
        foreach(AudioClip bulletOption in this.bulletSounds){
            this.towerTabSounds.options.Add(new Dropdown.OptionData(bulletOption.name));
        }
        foreach (AudioClip explOption in this.explosionSounds){
            this.bulletTabSounds.options.Add(new Dropdown.OptionData(explOption.name));
        }
        foreach (Transform debuf in GameObject.FindGameObjectWithTag("Debuf").GetComponentInChildren<Transform>()){
            this.debufDropdown.options.Add(new Dropdown.OptionData(debuf.gameObject.name));
        }
        foreach(DamageType type in System.Enum.GetValues(typeof(DamageType))){
            this.damageTypeDropdown.options.Add(new Dropdown.OptionData(type.ToString().ToLower()));
            this.strengthsDropdown.options.Add(new Dropdown.OptionData(type.ToString().ToLower()));
        }
        for(int i = 0; i < this.particleDamageObjects.Length; i++) {
            this.aoeParticlesDropdown.options.Add(new Dropdown.OptionData(this.particleDamageObjects[i].gameObject.name));
        }
        this.SetAOEParticle();
        this.chosenColor = new Color(this.r, this.g, this.b);
        this.selectedExplosionParticle.text = "None";
        this.selectedDebufParticles.text = "None";
        this.SetBulletExplosion();
        this.SetBulletDebuf();
        this.cooldown = this.tower.cooldown;
        this.customTower.damage = this.tower.damage;
        this.tower.particleManager.StopParticles();
    }
	
	// Update is called once per frame
	void Update () {
        if (this.towerRangeActive) {
            if(this.timeTillRangeInactive > 0f){
                this.timeTillRangeInactive -= Time.deltaTime * 1;
            }
            else{
                this.towerRangeActive = false;
                this.ChangeVisibilityRange(false);
            }
        }
        this.CalculateTowerCost();
	}

    public void SetBulletParticle() {
        string name = this.selectedBulletParticle.text;
        this.projectile.bullet = this.bulletParticles.Find(x => x.name == name);
        this.customTower.customBullet.name = name;
        Material newMat = BulletSaveLoad.instance.SaveParticleTexture(this.projectile.bullet.GetComponent<ParticleSystemRenderer>().sharedMaterial, BulletSaveLoad.material_path);
        // A new material is going to be used instead of the obave used bullet
        BulletSaveLoad.instance.CreateBullet(name, newMat);

        this.SetBulletSound();
    }

    public void SetAOEParticle() { // TO DO: This can easily be saved and adjusted using the booleans, but it is not implemented in the actual game
        int selectedParticle = this.aoeParticlesDropdown.value - 1;
        this.tower.selectedParticle = selectedParticle;
        for (int i = 0; i < this.particleDamageObjects.Length; i++) {
            this.particleDamageObjects[i].SetActive(i == selectedParticle);
        }
    }

    public void SetBulletExplosion(){
        string name = this.selectedExplosionParticle.text;
        if (!name.Equals("None")){
            GameObject explosion = this.explosionParticles.Find(x => x.name == name);
            BulletHandler.instance.ChangeBulletExplosion(this.projectile.bullet.name, explosion);
            this.customTower.customBullet.customExplosion.name = name; // TO DO: Make it so a player chooses his own unique name for the explosion when creating a new explosion
        }
        else{
            BulletHandler.instance.ChangeBulletExplosion(this.projectile.bullet.name, null);
            this.customTower.customBullet.customExplosion.name = "";
        }
    }
    public void SetBulletDebuf(){
        string name = this.selectedDebufParticles.text;
        for(int i = 0; i < this.enemy.debufs.Count; i++) {
            Debuf debuf = this.enemy.debufs[i];
            this.enemy.RemoveDebuf(debuf);
        }
        if (!name.Equals("None")) {
            this.projectile.debufScript = this.debufScript.transform.FindChild(name).GetComponent<DebufScript>();
            
        }
        else{
            this.projectile.debufScript = null;
        }
    }

    public void SetNewBullet(GameObject bulletObject){
        this.bullet = bulletObject.GetComponent<Bullet>();
    }

    public void SetBulletSound(){
        // If AOE use earthshake, if also slow use ice cracking
        int selectedOption = this.towerTabSounds.value;
        AudioClip bulletSound = this.bulletSounds[selectedOption];
        this.towerAudio.clips = new AudioClip[1];
        this.towerAudio.clips[0] = bulletSound;
    }

    public void SetExplosionSound(){
        int selectedOption = this.bulletTabSounds.value;
        AudioClip explosionSound = this.explosionSounds[selectedOption];
        AudioClip[] explosionSounds = new AudioClip[1] { explosionSound };
        this.explHandler.SetExplosionSound(this.bullet.explosion.name, explosionSounds); 
    }

    public void SetColor() {
        this.r = this.rSlider.value;
        this.g = this.gSlider.value;
        this.b = this.bSlider.value;
        this.chosenColor = new Color(this.r, this.g, this.b);
        this.colorDisplay.color = this.chosenColor;
        // Set the bullet to this color aswell
        BulletHandler.instance.ChangeBulletColor(this.projectile.bullet.name, this.chosenColor);
    }

    public void SetCooldown(){
        this.cooldown = this.customTower.isAOE ? this.aoeCooldownSlider.value : this.cooldownSlider.value;
        if (this.customTower.isAOE) {
            this.aoeParticleSystem.playbackSpeed = 3.6f - this.aoeCooldownSlider.value;
        }
        this.tower.cooldown = this.cooldown;
    }

    public void SetDamageType(){
        int selectedValue = this.damageTypeDropdown.value;
        this.customTower.towerType = (DamageType)selectedValue;
        this.tower.type = this.customTower.towerType;
    }

    public void SetStrength(){
        this.tower.strongAgainst.Clear();
        this.customTower.bonusDamage = (DamageType)this.strengthsDropdown.value;
        this.tower.strongAgainst.Add(this.customTower.bonusDamage);
    }

    public void SetDamage(){
        this.tower.damage = (int)this.damageSlider.value;
        this.customTower.damage = this.tower.damage;
        this.damageText.text = this.tower.damage.ToString();
    }

    public void SetRange(){
        this.tower.range = (int)this.rangeSlider.value;
        this.customTower.range = this.tower.range;
        this.tower.UpdateTowerRange();
        this.rangeText.text = (this.tower.range / 100).ToString("#.0");
        Camera.main.transform.position = new Vector3(this.cameraStartPos.x, this.cameraStartPos.y, this.cameraStartPos.z - (this.tower.range / 25));
        this.towerRangeActive = true;
        this.timeTillRangeInactive = this.maxActiveTime;
        this.ChangeVisibilityRange(true);
    }

    public void ChangeVisibilityRange(bool isVisible) {
        this.tower.SetRangeSphereActivity(isVisible);
    }

    public void SetIsAOE(){
        this.customTower.isAOE = !this.customTower.isAOE;
        this.tower.isAuraTower = this.customTower.isAOE;
        if (!this.customTower.isAOE){
            this.tower.particleManager.StopParticles();
        }
        this.projectile.enabled = !this.customTower.isAOE;
        /// Change tower to AOE and switch bullet tab for AOE tab
        /// If AOE is OFFF switch back, the ISSLOW checkbox should be in the AOE tab as that determines whether aoe is damage or slow
    }
    public void SetIsSlow(){
        this.customTower.isSlow = !this.customTower.isSlow;
        /// Change tower to not do any damage (lock the damage to 0 on the slider)
    }

    public void CalculateTowerCost(){
        float towerCost = 100;
        // All used values should not be hardcoded
        if (this.customTower.isAOE){
            towerCost += 50;
            if (!this.customTower.isSlow){
                towerCost += 25;
            }
        }
        if (!this.bullet.debufName.Equals("")) {
            towerCost += 25;
            if (!this.bullet.debufName.Equals("Slowed")){
                towerCost += 25;
            }
        }
        if(this.customTower.bonusDamage != DamageType.NONE) {
            towerCost += 15;
        }
        towerCost += this.tower.damage * 2;
        towerCost -= (this.tower.cooldown * 30);
        towerCost += this.tower.range / 5;

        // If tower is AOE - damage +75
        // tower AOE - slow +50
        // bullet debuf - damage +50
        // bullet debuf - slow +25
        // starting damage = damage / 2 (maybe 1 damage == 1 aspire)
        // cooldown = cooldown / 2 (0.1 = 
        // range = range / 10  (range 90 = 9)
        this.customTower.aspireCost = (int)towerCost;
        this.towerCost.text = this.customTower.aspireCost.ToString();
    }

    public void ChangeTab() {
        bool bulletTabActive = this.bulletTab.activeSelf;
        this.bulletTab.SetActive(!bulletTabActive);
        bool aoeTabActive = this.aoeTab.activeSelf;
        this.aoeTab.SetActive(!aoeTabActive);
    }


    public void SetCameraPosition(int tabID){
        if (tabID.Equals(0)){
            Camera.main.transform.position = new Vector3(this.cameraStartPos.x, this.cameraStartPos.y, this.cameraStartPos.z - (this.tower.range / 25));
        }
        else {
            Camera.main.transform.position = this.cameraStartPos;
        }
    }

    public void CreateTower(){
        // Create a prefab of the tower, Also when entering a name of the tower you can implement a check to see if one of those towers exists
    }


    /*
    Tower Creation Process:
       
    Tower aspects are chosen and changed
    Tower aspects are then saved into scripts that are serialized and put into a file. 
        When saving bullets/towers/explosions make it so it is a unique name so that the same name can't occur twice, this makes loading/saving a lot easier
    These scripts can be loaded into the game when starting the game up so the allready made turrets/bullets/explosions are available to the player (For this it would be easier to make the bullet and explosion creation a part of the tower creation in a more clear way)
     
    Before the start of the game a player gets to choose all the turrets he/she wishes to use in that play session.
    Then for each of the scripts a tower prefab is instantiated which will then serve as the "prefab" for that tower
    same thing for bullets and explosions, these also need to be then set in the bullethandler and explosionhandler

    To make searching these towers easier there should be a loader script for these turrets that put them in a handy file to then allow easy foreach searching.
    */
}