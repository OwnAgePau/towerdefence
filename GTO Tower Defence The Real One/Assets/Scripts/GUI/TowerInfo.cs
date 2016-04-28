using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TowerInfo : MonoBehaviour {

    public static TowerInfo instance;
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

    public bool isAnimatingMenu = false;
    public bool isMenuOpen = false;
    public bool shouldMenuBeOpen = false;

    //public string standardDamageText = " Damage";
    //public string standardSpeedText = " Speed";
    //public string standardProjectilesText = " Projectiles";
    //public string standardSlowText = "% Slow";
    // Upgrades

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(GUIcontroller.instance.isUICanvasLoaded && !this.isAnimatingMenu){
            if(this.isMenuOpen && !this.shouldMenuBeOpen){
                // Menu is open but should be close
                this.HideMenu();
            }
            else if(!this.isMenuOpen && this.shouldMenuBeOpen){
                // Menu is closed but should be open
                this.OpenMenu();
            }
        }

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
        if (state){
            this.ShowTowerUpgradeInfo();
        }
        else{
            this.HideTowerUpgradeInfo();
        }
    }

    public void OpenMenu(){
        // Scale up the menu
        this.shouldMenuBeOpen = true;
        if (!this.isAnimatingMenu && !this.isMenuOpen){
            this.isAnimatingMenu = true;
            StartCoroutine(LerpHandler.instance.Scale(0.5f, this.towerInfoMenu, true, LerpHandler.instance.curves[3], true));
            StartCoroutine(LerpHandler.instance.Scale(0.5f, this.towerUpgradeArrowMenu, true, LerpHandler.instance.curves[3], true));
        }
    }

    public void HideMenu(){
        // Scale down the menu
        this.shouldMenuBeOpen = false;
        if (!this.isAnimatingMenu && this.isMenuOpen){
            StartCoroutine(LerpHandler.instance.Scale(0.3f, this.towerInfoMenu, true, LerpHandler.instance.curves[2], false));
            StartCoroutine(LerpHandler.instance.Scale(0.3f, this.towerUpgradeArrowMenu, true, LerpHandler.instance.curves[2], false));
        }
    }

    public void ShowTowerUpgradeInfo()
    {
        this.towerUpgradeMenu.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void HideTowerUpgradeInfo()
    {
        this.towerUpgradeMenu.transform.localScale = new Vector3(0f, 0f, 0f);
    }
}