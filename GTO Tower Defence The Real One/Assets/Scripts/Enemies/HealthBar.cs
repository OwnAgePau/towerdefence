using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

    private Enemy enemy;

    public Vector3 scale;
    public float enemyFullHealth;
    public float healthPercentage;

    public Color red;
    public Color orange;

    public SpriteRenderer bar;

	// Use this for initialization
	void Start () {
        enemy = this.gameObject.GetComponentInParent<Enemy>();
        this.enemyFullHealth = enemy.fullHealth;
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.LookAt(Camera.main.gameObject.transform);
        float health = (float)enemy.health;
        float fullHealth = (float)enemy.fullHealth;
        this.healthPercentage = health / fullHealth;
        float currentWidth = this.scale.x * this.healthPercentage;
        this.transform.localScale = new Vector3(currentWidth, scale.y, scale.z);
        if(this.healthPercentage < 0.61f && this.healthPercentage > 0.32f){
            bar.color = this.orange;
        }
        if (this.healthPercentage < 0.31f){
            bar.color = this.red;
        }
        if (enemy.health < 0){
            this.gameObject.SetActive(false);
        }
    }
}
