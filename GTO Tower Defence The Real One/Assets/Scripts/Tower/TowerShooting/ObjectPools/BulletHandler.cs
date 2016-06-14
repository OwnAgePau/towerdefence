using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletHandler : MonoBehaviour {

    public static BulletHandler instance;

    public BulletType[] bulletPrefabs = new BulletType[4]; 
    // Maybe not use the damageType, as there could potentially be more mage towers to use different bullets

    public List<BulletType> bullets = new List<BulletType>();

    [System.Serializable]
    public struct BulletType{
        public DamageType type;
        public GameObject bullet;

        public BulletType(DamageType type, GameObject bullet){
            this.type = type;
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
            this.AddNewBullets(4, type);
        }
    }

    public void AddNewBullets(int amount, BulletType prefab){
        for (int i = 0; i < amount; i++){
            GameObject bulletObject = (GameObject)Instantiate(Resources.Load(prefab.bullet.name));
            bulletObject.transform.parent = this.gameObject.transform;
            bulletObject.name = prefab.bullet.name;
            bulletObject.SetActive(false);
            BulletType bulletType = new BulletType(prefab.type, bulletObject);
            this.bullets.Add(bulletType);
        }
    }

    public GameObject AddNewBullet(BulletType prefab){
        GameObject bulletObject = (GameObject)Instantiate(Resources.Load(prefab.bullet.name));
        bulletObject.transform.parent = this.gameObject.transform;
        bulletObject.name = prefab.bullet.name;
        bulletObject.SetActive(false);
        BulletType bulletType = new BulletType(prefab.type, bulletObject);
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

    public void ClearPool(){
        this.bullets.Clear();
    }
}
