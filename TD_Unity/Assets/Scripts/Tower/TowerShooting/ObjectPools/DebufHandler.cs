using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebufHandler : MonoBehaviour{

    public static DebufHandler instance;

    public int startPrefabs = 3;

    public DebufScript[] debufScripts;

    public DebufType[] debufPrefabs;
    // Using scripts instead of gameobjects so proper prefabs of the debufs are initiated.
    // So these can probably not be loaded before the start of the game and must be done run time (void start)

    public List<DebufType> debufs = new List<DebufType>();

    [System.Serializable]
    public struct DebufType{
        public string debufName;
        public GameObject debuf;

        public DebufType(string debufName, GameObject debuf){
            this.debufName = debufName;
            this.debuf = debuf;
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
        // Instantiate the prefabs
        this.InstantiatePrefabs();
        // Instantiate the pool
        this.InstantiatePool();
    }

    private void InstantiatePrefabs(){
        this.debufPrefabs = new DebufType[this.debufScripts.Length];
        for (int i = 0; i < this.debufScripts.Length; i++){
            DebufScript script = this.debufScripts[i];
            this.debufPrefabs[i].debufName = script.debufName;
            this.debufPrefabs[i].debuf = script.debufParticles;
        }
    }

    public void InstantiatePool(){
        foreach (DebufType type in this.debufPrefabs){
            this.AddNewDebufObjects(this.startPrefabs, type);
        }
    }

    public void AddNewDebufObjects(int amount, DebufType prefab){
        for (int i = 0; i < amount; i++){
            // U need to create the prefabs according to the scripts
            GameObject debufOjbect = (GameObject)Instantiate(prefab.debuf);
            debufOjbect.transform.parent = this.gameObject.transform;
            debufOjbect.name = prefab.debufName;
            debufOjbect.SetActive(false);
            DebufType debufType = new DebufType(prefab.debufName, debufOjbect);
            this.debufs.Add(debufType);
        }
    }

    public GameObject AddNewDebuf(DebufType prefab){
        GameObject debufOjbect = (GameObject)Instantiate(prefab.debuf);
        debufOjbect.transform.parent = this.gameObject.transform;
        debufOjbect.name = prefab.debuf.name;
        debufOjbect.SetActive(false);
        DebufType debufType = new DebufType(prefab.debufName, debufOjbect);
        this.debufs.Add(debufType);
        return debufOjbect;
    }

    public GameObject GetInactiveDebuf(string name){
        foreach (DebufType debufType in this.debufs){
            if (debufType.debufName.Equals(name)){
                if (!debufType.debuf.active){
                    debufType.debuf.SetActive(true);
                    return debufType.debuf;
                }
            }
        }
        // If this is reached, no debuf has been found so a new one is needed
        DebufType newDebufType = this.GetDebufType(name);
        if (newDebufType.debuf != null){
            // Add a new inactive debuf, but this one will be used by this tower, so set it active
            GameObject newDebuf = this.AddNewDebuf(newDebufType);
            newDebuf.SetActive(true);
            return newDebuf;
        }
        else{
            Debug.Log("A new Debuf : '" + name + "' could not be created!!!");
        }
        return null;
    }

    public void AddActiveDebufToPool(GameObject debufObject){
        debufObject.transform.parent = this.transform;
        debufObject.SetActive(false);
    }

    public DebufType GetDebufType(string name){
        DebufType nullType = new DebufType();
        foreach (DebufType debufType in this.debufPrefabs){
            if (debufType.debufName.Equals(name)){
                return debufType;
            }
        }
        return nullType;
    }

    public void ClearPool(){
        this.debufs.Clear();
    }
}