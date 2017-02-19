using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletHandler : MonoBehaviour {

    public static BulletHandler instance;

    public int startPrefabs = 4;

    public BulletType[] bulletPrefabs;
    // Maybe not use the damageType, as there could potentially be more mage towers to use different bullets

    public List<BulletType> bullets = new List<BulletType>();

    [System.Serializable]
    public struct BulletType{
        public GameObject bullet;

        public BulletType(GameObject bullet){
            this.bullet = bullet;
        }
    }

    /// <summary>
    ///  Create a list of bullet prefabs where a bullet is linked to a damage Type
    /// </summary>
    
   
    /// <summary>
    /// Create a list for each of the bullet prefabs
    /// </summary>

    void Awake(){
        instance = this;
    }

	// Use this for initialization
	void Start () {
        this.InstantiatePool();
    }

    public void InstantiatePool(){
        foreach (BulletType type in this.bulletPrefabs) {
            this.AddNewBullets(this.startPrefabs, type);
        }
    }

    public void AddNewBullets(int amount, BulletType prefab){
        for (int i = 0; i < amount; i++){
            GameObject bulletObject = (GameObject)Instantiate(Resources.Load("bullets/" + prefab.bullet.name));
            bulletObject.transform.parent = this.gameObject.transform;
            bulletObject.name = prefab.bullet.name;
            bulletObject.SetActive(false);
            BulletType bulletType = new BulletType(bulletObject);
            this.bullets.Add(bulletType);
        }
    }

    public GameObject AddNewBullet(BulletType prefab){
        GameObject bulletObject = (GameObject)Instantiate(Resources.Load(prefab.bullet.name));
        bulletObject.transform.parent = this.gameObject.transform;
        bulletObject.name = prefab.bullet.name;
        bulletObject.SetActive(false);
        BulletType bulletType = new BulletType(bulletObject);
        this.bullets.Add(bulletType);
        return bulletObject;
    }

    public GameObject GetInactiveBullet(string name){
        GameObject inactiveBullet = null;
        foreach(BulletType bulletType in this.bullets){
            if (bulletType.bullet.name.Equals(name)){
                if (!bulletType.bullet.active){
                    bulletType.bullet.SetActive(true);
                    return bulletType.bullet;
                }
            }
        }
        // If this is reached, no bullet has been found so a new one is needed
        BulletType newBulletType = this.GetBulletType(name);
        if (newBulletType.bullet != null){
            // Add a new inactive bullet, but this one will be used by this tower, so set it active
            GameObject newBullet = this.AddNewBullet(newBulletType);
            newBullet.SetActive(true);
            return newBullet;
        }
        else {
            Debug.Log("A new Bullet : '" + name + "' could not be created!!!");
        }
        return null;
    }

    public BulletType GetBulletType(string name){
        BulletType nullType = new BulletType();
        foreach (BulletType bulletType in this.bulletPrefabs) {
            if (bulletType.bullet.name.Equals(name)){
                return bulletType;
            }
        }
        return nullType;
    }

    public List<BulletType> GetAllBulletsOfType(string name){
        List<BulletType> foundBullets = new List<BulletType>();
        foreach(BulletType type in this.bullets){
            if (type.bullet.name.Equals(name)){
                foundBullets.Add(type);
            }
        }
        return foundBullets;
    }

    public void ClearPool(){
        this.bullets.Clear();
    }

    public void ChangeBulletColor(string prefabName, Color color) {
        List<BulletType> bullets = this.GetAllBulletsOfType(prefabName);
        foreach(BulletType type in bullets){
            type.bullet.GetComponent<ParticleSystem>().startColor = color;
        }
    }

    public void ChangeBulletExplosion(string prefabName, GameObject explosion){
        List<BulletType> bullets = this.GetAllBulletsOfType(prefabName);
        foreach (BulletType type in bullets){
            type.bullet.GetComponent<Bullet>().explosion = explosion;
        }
    }

    public void ChangeBulletEDebuf(string prefabName, string debuf){
        List<BulletType> bullets = this.GetAllBulletsOfType(prefabName);
        foreach (BulletType type in bullets){
            type.bullet.GetComponent<Bullet>().debufName = debuf;
        }
    }
}
