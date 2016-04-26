using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TowerInfo : MonoBehaviour {

    //public static TowerInfo instance;
    [Header("Menu Object Instances")]
    public GameObject towerInfoMenu;
    public GameObject towerUpgradeMenu;
    public GameObject towerUpgradeArrowMenu;

    [Header("Tower Selection Info")]
    public GameObject currentTowerObject;
    public Tower currentTower;

    public GameObject towerSelection;

    [Header("Basic Tower Info")]
    public Text towerName;
    public Text towerDamage;
    public Text towerLevel;
    public Text towerProjectiles;
    public Text towerSlow;
    public GameObject towerImage;
    [Header("Upgrade Tower Info")]
    public Text damageUpgrade;
    public Text speedUpgrade;
    public Text projUpgrade;
    public Text slowUpgrade;
    public GameObject upgradeButton;

    private bool isHoveringUpgrade;

    private bool isAnimatingMenu = false;

    //public string standardDamageText = " Damage";
    //public string standardSpeedText = " Speed";
    //public string standardProjectilesText = " Projectiles";
    //public string standardSlowText = "% Slow";
    // Upgrades

    /*void Awake(){
        instance = this;
    }*/

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(this.currentTower != null && GUIcontroller.instance.isUICanvasLoaded){
            // Display info of tower on screen
            this.towerSelection.SetActive(true);
            this.towerName.text = this.currentTowerObject.name;
            this.towerDamage.text = this.currentTower.damage.ToString();
            this.towerProjectiles.text = this.currentTower.projectiles.ToString();
            this.towerSlow.text = this.currentTower.slowAmount.ToString();
            this.towerImage.GetComponent<Image>().name = this.towerName.text;
            this.towerImage.GetComponent<Image>().sprite = this.currentTower.towerImage;
            //this.towerImage.mainTexture = this.currentTower.towerImage;
            this.towerLevel.text = this.currentTower.towerLevel.ToString();
            Upgrade towerUpgrade = this.currentTower.getUpgrade();

            this.SetUpgradeText(false);
            foreach (UpgradePart part in towerUpgrade.upgrades){
                if (part.type.Equals(UpgradeType.DAMAGE)){
                    int amount = part.amount - this.currentTower.damage;
                    this.damageUpgrade.text = "+" + amount.ToString();// + this.standardDamageText;
                    if(this.isHoveringUpgrade)
                        this.damageUpgrade.gameObject.SetActive(true);
                }
                else if (part.type.Equals(UpgradeType.SPEED)){
                    this.speedUpgrade.text = "+" + part.amount.ToString();// + this.standardSpeedText;
                    if (this.isHoveringUpgrade)
                        this.speedUpgrade.gameObject.SetActive(true);
                }
                else if (part.type.Equals(UpgradeType.PROJNR)){
                    int amount = part.amount - this.currentTower.projectiles;
                    this.projUpgrade.text = "+" + amount.ToString();// + this.standardProjectilesText;
                    if (this.isHoveringUpgrade)
                        this.projUpgrade.gameObject.SetActive(true);
                }
                else if (part.type.Equals(UpgradeType.SLOW)){
                    int amount = part.amount - (int)(this.currentTower.slowAmount * 100);
                    this.slowUpgrade.text = "+" + amount.ToString() + "%";// + this.standardSlowText;
                    if (this.isHoveringUpgrade)
                        this.slowUpgrade.gameObject.SetActive(true);
                }
            }
        }
        else{
            this.towerSelection.SetActive(false);
            this.SetUpgradeText(false);
        }
	}

    public void HideTowerInfo(){
        this.currentTower = null;
        
    }

    public void Upgrade(){
        this.currentTower.UpgradeTower();
    }

    public void SetUpgradeText(bool state) { 
        this.damageUpgrade.gameObject.SetActive(state);
        this.speedUpgrade.gameObject.SetActive(state);
        this.projUpgrade.gameObject.SetActive(state);
        this.slowUpgrade.gameObject.SetActive(state);
    }

    public void SetIsHovering(bool state){
        this.isHoveringUpgrade = state;
    }

    public void OpenMenu(){
        // Scale up the menu
        if(!this.isAnimatingMenu){
            StartCoroutine(LerpHandler.instance.Scale(5f, this.towerInfoMenu, true, LerpSmoothness.Smooth));
            this.isAnimatingMenu = true;
        }  
    }

    public void HideMenu(){
        // Scale down the menu
        if (!this.isAnimatingMenu){
            StartCoroutine(LerpHandler.instance.Scale(5f, this.towerInfoMenu, true, LerpSmoothness.SmoothDown));
            this.isAnimatingMenu = false;
        }
        else
        {
            // Start coroutine to close menu after it opened.
        }
    }
}