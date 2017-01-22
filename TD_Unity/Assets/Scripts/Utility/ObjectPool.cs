using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour {

    public GameObject objectPrefab;

    public List<GameObject> objects = new List<GameObject>();
    public int amountOfObjects;

    public GameObject parent;

    private int amountUnused = 0;

    private bool areInactives;
    private float durationTooManyObjects;

    public bool allowDeactivation = true;
    public float maxInactiveTime = 10f;

	// Use this for initialization
	void Start () {
        this.InstantiatePool();
    }

    void FixedUpdate(){
        if (this.allowDeactivation){
            this.CountInActiveObjects();
            if (this.areInactives){
                this.durationTooManyObjects += Time.deltaTime;
            }
            else{
                this.durationTooManyObjects = 0;
            }
            if (this.durationTooManyObjects > this.maxInactiveTime){
                this.DestroyInactive();
            }
        }
    }

    void CountInActiveObjects(){
        foreach (GameObject obj in this.objects){
            if (!obj.activeInHierarchy)
            {
                this.amountUnused += 1;
            }
        }
        this.areInactives = this.amountUnused > 0;
    }

    void DestroyInactive(){
        this.durationTooManyObjects = 0;
        GameObject inactive = this.GetInactiveObject();
        this.objects.Remove(inactive);
        if (inactive != null){
            Destroy(inactive);
        }
    }
	
    /// <summary>
    /// Instantiate an object for the amount of objects needed in the pool
    /// </summary>
    public void InstantiatePool(){
        int amountLeft = this.amountOfObjects - this.objects.Count;
        for (int i = 0; i < amountLeft; i++){
            GameObject instance = this.AddNewObject();
            instance.SetActive(false);
        }
    }

    /// <summary>
    /// Retrieve an inactive object
    /// </summary>
    /// <returns>Returns an inactive object as an active object</returns>
    public GameObject GetInactiveObjectToSpawn(){
        GameObject inactive = this.GetInactiveObject();
        this.durationTooManyObjects = 0;
        return inactive != null ? inactive : this.AddNewObject();
    }

    private GameObject GetInactiveObject(){
        foreach (GameObject obj in this.objects){
            if (!obj.active) {
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }

    GameObject AddNewObject(){
        GameObject instance = Instantiate(objectPrefab);
        instance.transform.SetParent(this.parent.transform, false);
        instance.SetActive(true);
        this.objects.Add(instance);
        return instance;
    }

    /// <summary>
    /// Only use when u have to get rid of the pool for some reason
    /// </summary>
    public void ClearPool(){
        this.objects.Clear();
    }
}