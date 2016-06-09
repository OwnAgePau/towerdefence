using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {

    private Enemy enemy;

    public Vector3 scale;
    public float enemyHealthBarSize;
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
	void FixedUpdate () { 
    }

    public void UpdateHealthBar(){
        this.healthPercentage = this.RecalculateLifePercentage();
        this.UpdateHealthBarSize();
        this.UpdateHealthBarColor();
        if (enemy.health < 0){
            this.gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBarColor(){
        if (this.healthPercentage < 0.61f && this.healthPercentage > 0.32f){
            bar.color = this.orange;
        }
        if (this.healthPercentage < 0.31f){
            bar.color = this.red;
        }
    }

    private float RecalculateLifePercentage(){
        float health = (float)enemy.health;
        float fullHealth = (float)enemy.fullHealth;
        return health / fullHealth;
    }

    private void UpdateHealthBarSize(){
        float currentWidth = this.scale.x * this.healthPercentage;
        this.transform.localScale = new Vector3(currentWidth, scale.y, scale.z);
    }
}