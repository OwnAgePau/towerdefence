using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionHandler : MonoBehaviour{

    public static ExplosionHandler instance;

    //public DebufScript[] explosionScripts = new DebufScript[3];

    public ExplosionType[] explosionPrefabs;
    // Using scripts instead of gameobjects so proper prefabs of the debufs are initiated.
    // So these can probably not be loaded before the start of the game and must be done run time (void start)

    public List<ExplosionType> explosions = new List<ExplosionType>();

    [System.Serializable]
    public struct ExplosionType{
        public ParticleSystem particleSystem;
        public GameObject explosion;

        public ExplosionType(ParticleSystem particleSystem, GameObject explosion){
            this.particleSystem = particleSystem;
            this.explosion = explosion;
        }
    }

    /// <summary>
    ///  Create a list of debuf prefabs where a debuf is linked to a name
    /// </summary>


    /// <summary>
    /// Create a list for each of the debuf prefabs
    /// </summary>

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start(){
        // Instantiate the pool
        this.InstantiatePool();
    }

    public void InstantiatePool(){
        foreach (ExplosionType explosionType in this.explosionPrefabs){
            this.AddNewExplosionObjects(4, explosionType);
        }
    }

    public void AddNewExplosionObjects(int amount, ExplosionType prefab){
        for (int i = 0; i < amount; i++){
            // U need to create the prefabs according to the scripts
            GameObject explosionObject = (GameObject)Instantiate(prefab.explosion);
            explosionObject.transform.parent = this.gameObject.transform;
            explosionObject.name = prefab.explosion.name;
            explosionObject.SetActive(false);
            ParticleSystem particleSystem = explosionObject.GetComponent<ParticleSystem>();
            ExplosionType explosionType = new ExplosionType(particleSystem, explosionObject);
            this.explosions.Add(explosionType);
        }
    }

    public ExplosionType AddNewExplosion(ExplosionType prefab){
        GameObject explosionObject = (GameObject)Instantiate(prefab.explosion);
        explosionObject.transform.parent = this.gameObject.transform;
        explosionObject.name = prefab.explosion.name;
        explosionObject.SetActive(false);
        ParticleSystem particleSystem = explosionObject.GetComponent<ParticleSystem>();
        ExplosionType explosionType = new ExplosionType(particleSystem, explosionObject);
        this.explosions.Add(explosionType);
        return explosionType;
    }

    public GameObject GetInactiveExplosionType(string name){
        foreach (ExplosionType explosionType in this.explosions){
            if (explosionType.explosion.name.Equals(name)){
                if (!explosionType.explosion.active){
                    explosionType.explosion.SetActive(true);
                    explosionType.particleSystem.enableEmission = true;
                    return explosionType.explosion;
                }
            }
        }
        // If this is reached, no debuf has been found so a new one is needed
        ExplosionType expType = this.GetExplosionType(name);
        if (expType.explosion != null){
            // Add a new inactive debuf, but this one will be used by this tower, so set it active
            ExplosionType newExplosionType = this.AddNewExplosion(expType);
            newExplosionType.explosion.SetActive(true);
            newExplosionType.particleSystem.enableEmission = true;
            return newExplosionType.explosion;
        }
        else{
            Debug.Log("A new Explosion : '" + name + "' could not be created!!!");
        }
        return null;
    }

    public void AddActiveExplosionToPool(GameObject explosion){
        ExplosionType type = this.GetExplosionType(explosion);
        type.explosion.transform.parent = this.transform;
        type.explosion.SetActive(false);
        type.particleSystem.enableEmission = false;
    }

    public ExplosionType GetExplosionType(string name){
        ExplosionType nullType = new ExplosionType();
        foreach (ExplosionType explosionType in this.explosionPrefabs){
            if (explosionType.explosion.name.Equals(name)){
                return explosionType;
            }
        }
        return nullType;
    }

    public ExplosionType GetExplosionType(GameObject explosion){
        ExplosionType nullType = new ExplosionType();
        foreach (ExplosionType explosionType in this.explosions){
            if (explosionType.explosion.Equals(explosion)){
                return explosionType;
            }
        }
        return nullType;
    }

    public void ClearPool(){
        this.explosions.Clear();
    }
}