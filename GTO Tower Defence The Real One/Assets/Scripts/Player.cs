using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public static Player instance;
    public ImageHoverInfo villagersPopUp;
    public string nextVillagerTime = ", next villager in : ";
    public ImageHoverInfo enemiesKilledPopUp;
    public string enemiesKilledText = ", enemies killed : ";

    [Header("Global Player Stats")]
    public int villagers = 5;
    public int aspirePoints;
    public int power; // Total damage
    public int score = 1;
    public int enemiesKilled;
    public int lives = 50;

    public int enemyHealth = 2;
    public float enemyScore = 1;

    public bool isPaused = false;
    public bool hasLost = false;

    [Header("Villager Spawning")]
    public float timeUntillNextVillager = 20f;
    public float currentTime = 20f;
    
    [Header("Pop Up Text")]
    public Color goodColor;
    public Color badColor;
    public GameObject popUpTextPrefab;
    public Canvas canvas;

    [Header("Enemy Flicker Colors")]
    public Color flickerColor;
    public Color normalEmissionColor;

    [Header("Enemy Type Colors")]
    public List<EnemyTypeColor> enemyTypeColor = new List<EnemyTypeColor>();

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start () {
        if(Achievements.instance != null){
            Achievements.instance.totalVillagers = this.villagers;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (this.currentTime > 0f){
            this.currentTime -= Time.deltaTime;
            this.villagersPopUp.extraInfo = this.nextVillagerTime + (int)this.currentTime;
        }
        else{
            this.currentTime = this.timeUntillNextVillager;
            this.AddVillager();
        }

        // Do a calculation of power whenever a tower is added instead of the continues calculation you are doing now
        /*GameObject[] towers = GameObject.FindGameObjectsWithTag("tower");
        int damage = 0;
        if (towers.Length > 0){
            foreach (GameObject tower in towers){
                Tower towerScript = tower.GetComponent<Tower>();
                if(towerScript != null){
                    damage += towerScript.damage;
                }
                 
            }
        }*/
        /// Change the way power is being managed, add power whenever a new tower is placed (works well with showing pop up texts aswell)
        //this.power = damage;

        this.enemiesKilledPopUp.extraInfo = this.enemiesKilledText + this.enemiesKilled;

        if (this.lives <= 0) {
            this.hasLost = true;
            Controls.instance.menu.SetActive(true);
            PlayerPrefs.SetInt("Highscore", GUIcontroller.instance.GetHighscore());
            foreach (RectTransform child in Controls.instance.menu.transform){
                if(child.gameObject.tag.Equals("Menu") || child.gameObject.tag.Equals("LooseMenu")){
                    child.gameObject.SetActive(true);
                }
                else{
                    child.gameObject.SetActive(false);
                }
            }
            //controls.PauseGame();
        }
    }

    public void AddPower(int damage){
        this.power += damage;
    }

    public void RemovePower(int damage){
        this.power -= damage;
    }

    public void AddScore(int score){
        if (!this.hasLost){
            this.score += score;
            this.AddPopUpText(GUIcontroller.instance.scoreText.gameObject.transform.position, this.goodColor, "+" + score);
        }
    }

    public void AddAspirePoints(int aspirePoints){
        if (!this.hasLost){
            this.aspirePoints += aspirePoints;
            this.AddPopUpText(GUIcontroller.instance.aspireText.gameObject.transform.position, this.goodColor, "+" + aspirePoints);
        }
    }

    public void RemoveAspirePoints(int aspirePoints){
        this.aspirePoints -= aspirePoints;
        this.AddPopUpText(GUIcontroller.instance.aspireText.gameObject.transform.position, this.badColor, "-" + aspirePoints);
    }

    public bool CheckAspirePoints(int neededPoints){
        return this.aspirePoints >= neededPoints;
    }

    public void AddVillager(){
        if (!this.hasLost){
            this.villagers += 1;
            Achievements.instance.totalVillagers += 1;
            this.AddPopUpText(GUIcontroller.instance.villagersText.gameObject.transform.position, this.goodColor, "+1");
        }
    }

    public void RemoveVillager(){
        this.villagers -= 1;
        this.AddPopUpText(GUIcontroller.instance.villagersText.gameObject.transform.position, this.badColor, "-1");
    }

    public bool CheckVillagers(){
        return this.villagers - 1 >= 0;
    }

    public void EnemyKilled(){
        if (!this.hasLost){
            this.enemiesKilled++;
        }
    }

    public void IncreaseEnemyHealth(){
        if (!this.hasLost){
            float enemyHealth = this.enemyHealth;
            enemyHealth = enemyHealth / 3;
            this.enemyHealth += (int)enemyHealth;
            //float enemyScore = this.enemyScore;
            float score = this.enemyScore / 3;
            this.enemyScore += score;
        }
    }

    public void LooseLife(){
        if (!this.hasLost){
            this.lives--;
            this.AddPopUpText(GUIcontroller.instance.lifeText.gameObject.transform.position, this.badColor, "-1");
        }
    }

    public void AddPopUpText(Vector3 textLocation, Color color, string text){
        Vector3 aboveTextLocation = new Vector3(textLocation.x, textLocation.y - 5f, textLocation.z);
        GameObject popUpText = GameObject.Instantiate(this.popUpTextPrefab, aboveTextLocation, this.gameObject.transform.rotation) as GameObject;
        popUpText.name = this.popUpTextPrefab.name;
        popUpText.GetComponent<Text>().text = text;
        //popUpText.GetComponent<PopUpText>().textColor = color;
        popUpText.GetComponent<Outline>().effectColor = color;
        popUpText.transform.parent = this.canvas.transform;
    }
}

[System.Serializable]
public struct EnemyTypeColor
{
    public DamageType enemyType;
    public Color color;

    public EnemyTypeColor(DamageType dmgType, Color color)
    {
        this.enemyType = dmgType;
        this.color = color;
    }
}