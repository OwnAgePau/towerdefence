using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletHandler : MonoBehaviour {

    public static BulletHandler instance;

    public GameObject[] bulletPrefabs;
    public List<GameObject> fireballs = new List<GameObject>();
    public List<GameObject> magicbolts = new List<GameObject>();
    public List<GameObject> poisonorbs = new List<GameObject>();

    void Awake(){
        instance = this;
    }

	// Use this for initialization
	void Start () {
        this.AddNewBullets(4, this.bulletPrefabs[0], this.fireballs);
        this.AddNewBullets(4, this.bulletPrefabs[1], this.magicbolts);
        this.AddNewBullets(4, this.bulletPrefabs[2], this.poisonorbs);
    }

    public void AddNewBullets(int amount, GameObject prefab, List<GameObject> bulletList){
        for (int i = 0; i < amount; i++){
            GameObject obj = (GameObject)Instantiate(Resources.Load(prefab.name));
            obj.transform.parent = this.gameObject.transform;
            obj.name = prefab.name;
            obj.SetActive(false);
            bulletList.Add(obj);
        }
    }

    public GameObject AddNewBullet(GameObject prefab, List<GameObject> bulletList){
        GameObject obj = (GameObject)Instantiate(Resources.Load(prefab.name));
        obj.transform.parent = this.gameObject.transform;
        obj.name = prefab.name;
        obj.SetActive(false);
        bulletList.Add(obj);
        Debug.Log(obj.name);
        return obj;
    }

    public GameObject GetInactiveBullet(string name){
        if (name.Equals(bulletPrefabs[0].name)){
            return this.GetInactiveBullet(name, this.fireballs, 0);
        }
        else if (name.Equals(bulletPrefabs[1].name)){
            return this.GetInactiveBullet(name, this.magicbolts, 1);
        }
        else if (name.Equals(bulletPrefabs[2].name)){
            return this.GetInactiveBullet(name, this.poisonorbs, 2);
        }
        return null;
    }

    private GameObject GetInactiveBullet(string name, List<GameObject> list, int nr){
        GameObject inactiveBullet = null;
        if (list.Count > 0){
            inactiveBullet = list[0];
            inactiveBullet.SetActive(true);
            list.RemoveAt(0);
            return inactiveBullet;
        }
        else{
            this.AddNewBullets(2, bulletPrefabs[nr], list);
        }
        //list.Remove(bullet);
        //bullet.SetActive(true);
        return null;
    }

    public void AddActiveBullet(GameObject obj){
        if (obj.name.Equals(bulletPrefabs[0].name)){
            this.AddActiveBullet(obj, this.fireballs);
        }
        else if (obj.name.Equals(bulletPrefabs[1].name)){
            this.AddActiveBullet(obj, this.magicbolts);
        }
        else if (obj.name.Equals(bulletPrefabs[2].name)){
            this.AddActiveBullet(obj, this.poisonorbs);
        }
    }

    private void AddActiveBullet(GameObject obj, List<GameObject> list){
        list.Add(obj);
        obj.SetActive(false);
    }

    public void ClearPool(){
        for (int i = this.fireballs.Count - 1; i > 0; i--){
            GameObject obj = this.fireballs[i];
            this.fireballs.RemoveAt(i);
            Destroy(obj);
        }
        for (int i = this.magicbolts.Count - 1; i > 0; i--){
            GameObject obj = this.magicbolts[i];
            this.magicbolts.RemoveAt(i);
            Destroy(obj);
        }
        for (int i = this.poisonorbs.Count - 1; i > 0; i--){
            GameObject obj = this.poisonorbs[i];
            this.poisonorbs.RemoveAt(i);
            Destroy(obj);
        }
    }
}
