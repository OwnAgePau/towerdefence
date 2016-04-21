using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public GameObject destination;
    public Vector3 bulletStartPos;

    private Tower firedFrom;

    public int speed = 20;
    //public int damage = 10;

    public float deathTimer = 5f;

    private BulletParticle particleObject;
    public GameObject explosion;

    public string debufName;
    public Debuf debuf;

	// Use this for initialization
	void Start () {
        this.particleObject = this.GetComponentInChildren<BulletParticle>();
        GameObject debufs = GameObject.Find("Debufs");
        DebufScript debufScript = debufs.transform.FindChild(debufName).gameObject.GetComponent<DebufScript>();
        if(debufScript != null){
            this.debuf = debufScript.CreateDebuf();
        } 
	}
	
	// Update is called once per frame
	void Update () {
        if (this.destination != null){
            this.transform.LookAt(this.destination.transform);
            if (this.particleObject != null){
                this.particleObject.lookAt = bulletStartPos;
            }
            var step = speed * Time.deltaTime;
            float enemyHeightOffset = this.destination.GetComponent<BoxCollider>().size.y / 2;
            Vector3 destPos = this.destination.transform.position;
            Vector3 targetPos = new Vector3(destPos.x, destPos.y + enemyHeightOffset, destPos.z);
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetPos, step);
        }
        else{
            Destroy(this.transform.gameObject);
            //BulletHandler.instance.AddActiveBullet(this.gameObject);
        }

        if (this.deathTimer > 0f){
            this.deathTimer -= Time.deltaTime;
        }
        else{
            Destroy(this.transform.gameObject);
            //BulletHandler.instance.AddActiveBullet(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider col){
        Enemy enemy = col.GetComponent<Enemy>();
        if (enemy != null){
            enemy.DamageEnemy(this.firedFrom.damage, this.firedFrom.type);
            if (!debuf.debufName.Equals("")){
                enemy.ApplyEnemyDebuf(this.debuf);
                debuf.SetEnemy(enemy);
            }
            if (this.explosion != null){
                this.CreateExplosion(col.gameObject);
            }
            Destroy(this.transform.gameObject);
            //BulletHandler.instance.AddActiveBullet(this.gameObject);
        }
    }

    void CreateExplosion(GameObject destination){
        GameObject gameObject = GameObject.Instantiate(explosion, destination.transform.position, destination.transform.rotation) as GameObject;
    }

    public void SetDestination(GameObject destination){
        this.destination = destination;
    }

    public void SetFiredFrom(Tower tower){
        this.firedFrom = tower;
    }

    public Tower GetFiredFrom(){
        return this.firedFrom;
    }
}