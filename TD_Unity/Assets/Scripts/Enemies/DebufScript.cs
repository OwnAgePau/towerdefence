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

    // Create a debuf that is scaled to the tower Level
    public Debuf CreateDebuf(int towerLevel){
        return new Debuf(this.debufName, this.damage * (towerLevel + 1 / 2), this.slow, this.isTimed, this.typeOfDamage, this.debufTime, debufParticles);
    }

    public Debuf CreateDebuf(){
        return new Debuf(this.debufName, (float)this.damage, this.slow, this.isTimed, this.typeOfDamage, this.debufTime, debufParticles);
    }
}