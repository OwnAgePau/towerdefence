using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DemonBeam : MonoBehaviour {

    public GameObject particleAim;
    private GameObject particle;
    private ParticleSystem partSystem;

    public float cooldown = 0.2f; // Beam damage speed

    //private GameObject target;
    private List<GameObject> targets;

    private Tower towerScript;

    public AudioScript beamSound;

    // Use this for initialization
    void Start () {
        towerScript = this.GetComponent<Tower>();
        particle = this.particleAim.transform.FindChild("Particle").gameObject;
        partSystem = particle.GetComponent<ParticleSystem>();
        //this.beamSound = GameObject.Find("Laserbeam").GetComponent<AudioScript>();
    }
	
	// Update is called once per frame
	void Update () {
        // Rotate beam to the enemy
        if (towerScript.enemiesInRange.Count > 0){
            this.targets = new List<GameObject>();
            for(int i = 0; i < this.towerScript.projectiles; i++){
                GameObject target = towerScript.GetTarget(this.targets);
                //target = towerScript.target;
                if (target != null){
                    this.targets.Add(target);
                    partSystem.Play();
                    this.beamSound.PlaySound();
                    Vector3 targetAim = new Vector3(target.transform.position.x, target.transform.position.y + 0.5f, target.transform.position.z);
                    this.particleAim.transform.LookAt(targetAim);
                    particle.transform.rotation = this.particleAim.transform.rotation;
                    this.DoDamage(target);
                }
                else{
                    partSystem.Stop();
                    this.beamSound.StopPlaying();
                }
            }   
        }
        else{
            partSystem.Stop();
            this.beamSound.StopPlaying();
        }
	}

    public void DoDamage(GameObject target){
        int damage = this.towerScript.damage;
        if (this.towerScript.currentCooldown < 0.1f){
            Enemy enemy = target.GetComponent<Enemy>();
            enemy.DamageEnemy(damage, this.towerScript.type);
            this.towerScript.currentCooldown = this.cooldown;
        }
        else{
            this.towerScript.currentCooldown -= Time.deltaTime;
        }
    }
}
