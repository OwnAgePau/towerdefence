using UnityEngine;
using System.Collections;

public class DebufScript : MonoBehaviour {

    public string debufName;
    public int damage;
    public float slow;
    public bool isTimed;
    public float debufTime = 0.0f;
    public DamageType typeOfDamage;
    public GameObject debufParticles;

    //public Debuf debufTemplate;

    // Use this for initialization
    void Start () {
        //debufTemplate = new Debuf(this.debufName, this.damage, this.debufTime);
	}

    public Debuf CreateDebuf(){
        return new Debuf(this.debufName, this.damage, this.slow, this.isTimed, this.typeOfDamage, this.debufTime, debufParticles);
    }
}