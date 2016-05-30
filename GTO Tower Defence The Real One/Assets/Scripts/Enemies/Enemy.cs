﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

    // Enemy Info
    [Header("General Stats")]
    public int fullHealth = 50;
    public int health = 50;
    public int score = 20;
    public float currentSpeed = 1f;
    public float speed = 1f;
    public DamageType enemyType;

    [System.Serializable]
    public struct ResistanceToDamageType {
        public DamageType dmgType;
        [Range(0, 1)]
        public float resistance;
    }
    public List<ResistanceToDamageType> resistentTo = new List<ResistanceToDamageType>();
    private Dictionary<DamageType, float> hasResistanceTo = new Dictionary<DamageType, float>();

    [Header("Death")]
    public float timeTillDeath = 2.8f;
    public bool isDead = false;
    public bool deathScoreRecieved = false;
    public bool isEnemyDamaged;

    private float enemyFlickerTime = 0.1f;
    public float enemyCurrentFlickerTime = 0f;

    // Debufs
    [Header("Debufs")]
    public List<Debuf> debufs = new List<Debuf>();
    private List<Debuf> debufsToRemove = new List<Debuf>();

    // Animations
    private Animator animatorComp;
    private Animation animationComp;
    private List<AnimationClip> animationClips = new List<AnimationClip>();

    public List<SkinnedMeshRenderer> meshes = new List<SkinnedMeshRenderer>();

    // Use this for initialization
    void Start() {
        foreach (ResistanceToDamageType resDmgType in this.resistentTo) {
            this.hasResistanceTo.Add(resDmgType.dmgType, resDmgType.resistance);
        }
        this.animatorComp = this.GetComponent<Animator>();
        this.animationComp = this.GetComponent<Animation>();
        if (this.animationComp != null) {
            foreach (AnimationState state in this.animationComp) {
                this.animationClips.Add(state.clip);
            }
            this.animationClips.RemoveAt(0);
        }
        foreach(Transform child in this.transform){
            SkinnedMeshRenderer childRenderer = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (childRenderer != null) {
                this.meshes.Add(childRenderer);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (this.isEnemyDamaged) {
            if(this.enemyCurrentFlickerTime < this.enemyFlickerTime){
                this.enemyCurrentFlickerTime += Time.deltaTime;
            }
            else{
                // Enemy has taken damage, make it flicker
                this.isEnemyDamaged = false;
                this.enemyCurrentFlickerTime = 0f;
                foreach (SkinnedMeshRenderer renderer in this.meshes){
                    Material mat = renderer.material;
                    mat.SetColor("_Color", Player.instance.normalEmissionColor);
                    renderer.material = mat;
                }
            }
        }

        if (this.health <= 0) {
            if (!this.deathScoreRecieved) {
                this.GetComponent<BoxCollider>().enabled = false;
                this.SetAnimationDead();
                this.PreDeath();
                this.RemoveEnemyFromTowers();
                this.EnemyKilledScore();
                this.deathScoreRecieved = true;
            }
            if (!this.isDead) {
                this.timeTillDeath -= Time.deltaTime;
                if (this.timeTillDeath < 0) {
                    this.Die();
                }
            }
        }
        else {
            if (this.debufsToRemove.Count > 0) {
                this.debufsToRemove.RemoveAt(0);
            }
            foreach (Debuf debuf in this.debufs) {
                Transform child = this.transform.FindChild(debuf.debufName);
                if (child == null) {
                    Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y + 0.5f, this.transform.position.z);
                    GameObject childObject = GameObject.Instantiate(debuf.debufParticles, pos, this.transform.rotation) as GameObject;
                    childObject.transform.parent = this.transform;
                    childObject.name = debuf.debufName;
                }
                debuf.DoTick();
            }
        }
    }

    public void SetAnimationDead() {
        if (this.animatorComp != null) {
            this.animatorComp.SetTrigger("isDead");
        }
        if (this.animationComp != null) {
            // Remove first entry so that only the death animations remain
            int highestPosilibity = this.animationClips.Count; // 1
            int random = Random.Range(0, highestPosilibity);
            AnimationClip clip = this.animationClips[random];
            this.animationComp.Play(clip.name);
        }
    }

    public List<Debuf> GetDebufsToRemove() {
        return this.debufsToRemove;
    }

    // Only to use when instantiating a loaded enemy
    public void SetDebufsToRemove(List<Debuf> debufsToRemove) {
        this.debufsToRemove = debufsToRemove;
    }

    public void ApplyEnemyDebuf(Debuf debuf) { // The debuf will stack with the debuf of the same kind
        Debuf enDebuf = this.HasDebuf(debuf.debufName);
        if (enDebuf != null) {
            enDebuf.debufTime += debuf.debufTime;
        }
        else{
            if (!debuf.debufName.Equals("")){
                this.debufs.Add(debuf);
            }
        }
    }

    public void ApplyEnemyDebuf(Debuf debuf, float time){ // Apply debuf, but doesn't stack with the same kind
        Debuf enDebuf = this.HasDebuf(debuf.debufName);
        if (enDebuf != null){
            enDebuf.debufTime = time;
        }
        else{
            this.debufs.Add(debuf);
        }
    }

    public Debuf HasDebuf(string debufName){
        foreach (Debuf enDebuf in this.debufs){
            if (enDebuf.debufName.Equals(debufName)){
                return enDebuf;
            }
        }
        return null;
    }

    public void RemoveDebuf(Debuf debuf){
        if(this != null){
            this.debufsToRemove.Add(debuf);
            Transform child = this.transform.FindChild(debuf.debufName);
            if (child != null){
                Destroy(child.gameObject);
            }
        }
    }

    void PreDeath(){
        this.currentSpeed = 0;
        Rigidbody body = this.GetComponent<Rigidbody>();
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    void RemoveEnemyFromTowers(){
        GameObject[] towers = GameObject.FindGameObjectsWithTag("tower");
        foreach (GameObject tower in towers){
            Tower towerScript = tower.GetComponent<Tower>();
            if(towerScript != null){
                if (towerScript.enemiesInRange.Contains(this.gameObject)){
                    towerScript.RemoveEnemy(this.gameObject);
                }
            } 
        }
    }

    void EnemyKilledScore(){ 
        if (!this.GetComponent<EnemyMovement>().hasReachedEnding){
            Player.instance.AddScore(this.score);
            Player.instance.AddAspirePoints(this.score / 2);
            Player.instance.EnemyKilled();
        }
    }

    void Die(){ 
        this.isDead = true;
        Destroy(this.transform.gameObject); 
    }

    public void DamageEnemy(float damage, DamageType damageType){
        float damageReduction = this.CalcDamageReduction(damage, damageType);
        this.health -= (int)(damage - damageReduction);
        if (this.isEnemyDamaged){
            this.enemyCurrentFlickerTime = 0f;
        }
        this.isEnemyDamaged = true;
        foreach (SkinnedMeshRenderer renderer in this.meshes){
            if(renderer != null){
                Material mat = renderer.material;
                mat.SetColor("_Color", Player.instance.flickerColor);
                renderer.material = mat;
            }
        }
    }

    public float CalcDamageReduction(float damage, DamageType damageType){
        float resistance = 0f;
        if (hasResistanceTo.TryGetValue(damageType, out resistance)) {
            float damageResisted = resistance * damage;
            return damageResisted;
        }
        return 0;
    }

    public void EnemyReachedVillage(){
        Player.instance.LooseLife();
        this.Die();
    }
}