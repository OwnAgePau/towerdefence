using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TowerInfo : MonoBehaviour {

    public static TowerInfo instance;
    [Header("Menu Object Instances")]
    public GameObject towerInfoPosMenu;
    public GameObject towerInfoMenu;
    public GameObject towerUpgradeMenu;
    public GameObject towerUpgradeCostMenu;
    public GameObject towerUpgradeArrowMenu;

    [Header("Tower Selection Info")]
    public GameObject currentTowerObject;
    public Tower currentTower;

    public GameObject towerSelection;

    [Header("Basic Tower Info")]
    public Text towerName;
    public Text towerDamage;
    public Text towerLevel;
    public string levelText = "Level: ";
    public Text towerProjectiles;
    public Text towerSlow;
    public Text towerRange;
    public Text towerStrongAgainst;
    public GameObject towerImage;

    [Header("Upgrade Tower Info")]
    public Text upgradeCost;
    public Outline upgradeCostOutline;
    public Text damageUpgrade;
    //public Text speedUpgrade;
    public Text projUpgrade;
    public Text slowUpgrade;
    public GameObject upgradeButton;

    private bool isHoveringUpgrade;

    public bool isAnimatingMenu = false;
    public bool isMenuOpen = false;
    public bool shouldMenuBeOpen = false;
    public bool isHovering = false;

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
                // Menu is open but should be closed
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
            this.towerLevel.text = this.levelText + this.currentTower.towerLevel.ToString();
            this.towerRange.text = this.currentTower.range.ToString();
            string strongAgainst = "";
            DamageType type = DamageType.NONE;
            for(int i = 0; i < this.currentTower.strongAgainst.Count; i++) {
                type = this.currentTower.strongAgainst[i];
                if(i > 0){
                    strongAgainst += ", ";
                }
                strongAgainst += type.ToString();
            }
            this.towerStrongAgainst.text = strongAgainst;
            Upgrade towerUpgrade = this.currentTower.getUpgrade();
            if(towerUpgrade == null){
                return;
            }

            this.SetUpgradeText(false);
            this.upgradeCost.text = towerUpgrade.upgradeCost.ToString();
            if (Player.instance.CheckAspirePoints(towerUpgrade.upgradeCost)){
                this.upgradeCostOutline.effectColor = Player.instance.goodColor;
            }
            else{
                this.upgradeCostOutline.effectColor = Player.instance.badColor;
            }
            // The stuff below should go in its own method, and possible even split up in some methods to make it more generic
            int amount = 0;
            UpgradePart part = null;
            for(int i = 0; i < towerUpgrade.upgrades.Length; i++) {
                part = towerUpgrade.upgrades[i];
                if (part.type.Equals(UpgradeType.DAMAGE)){
                    amount = part.amount - this.currentTower.damage;
                    this.damageUpgrade.text = "+" + amount.ToString();// + this.standardDamageText;
                    if(this.isHoveringUpgrade)
                        this.damageUpgrade.gameObject.SetActive(true);
                }
                /*else if (part.type.Equals(UpgradeType.SPEED)){
                    this.speedUpgrade.text = "+" + part.amount.ToString();// + this.standardSpeedText;
                    if (this.isHoveringUpgrade)
                        this.speedUpgrade.gameObject.SetActive(true);
                }*/
                else if (part.type.Equals(UpgradeType.PROJNR)){
                    amount = part.amount - this.currentTower.projectiles;
                    this.projUpgrade.text = "+" + amount.ToString();// + this.standardProjectilesText;
                    if (this.isHoveringUpgrade)
                        this.projUpgrade.gameObject.SetActive(true);
                }
                else if (part.type.Equals(UpgradeType.SLOW)){
                    amount = part.amount - (int)(this.currentTower.slowAmount * 100);
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
        Upgrade upgrade = this.currentTower.UpgradeTower();
        if(upgrade == null){
            this.towerUpgradeArrowMenu.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public void SetUpgradeText(bool state) { 
        this.damageUpgrade.gameObject.SetActive(state);
        //this.speedUpgrade.gameObject.SetActive(state);
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
        // Transform the tower position to a screen position to put the menu 
        Vector3 screenPos = Camera.main.WorldToScreenPoint(this.currentTowerObject.transform.position);
        // Change this position to a normal position on the UI, maybe this looks better I DUNNO
        this.towerInfoPosMenu.transform.position = new Vector3(screenPos.x, screenPos.y, screenPos.z);
        this.shouldMenuBeOpen = true;
        if (!this.isAnimatingMenu && !this.isMenuOpen){
            this.isAnimatingMenu = true;
            //StartCoroutine(LerpHandler.instance.Scale(0.5f, this.towerInfoMenu, true, LerpHandler.instance.curves[3], true));
        }
        if (this.currentTower.towerLevel < this.currentTower.towerUpgradeObject.maxLevel){
            //StartCoroutine(LerpHandler.instance.Scale(0.5f, this.towerUpgradeArrowMenu, true, LerpHandler.instance.curves[3], true));
        }
        else{
            this.towerUpgradeArrowMenu.transform.localScale = new Vector3(0, 0, 0);
        }
    }

    public void HideMenu(){
        // Scale down the menu
        this.shouldMenuBeOpen = false;
        if (!this.isAnimatingMenu && this.isMenuOpen){
            //StartCoroutine(LerpHandler.instance.Scale(0.3f, this.towerInfoMenu, true, LerpHandler.instance.curves[2], false));
            if (this.currentTower.towerLevel < this.currentTower.towerUpgradeObject.maxLevel){
                //StartCoroutine(LerpHandler.instance.Scale(0.3f, this.towerUpgradeArrowMenu, true, LerpHandler.instance.curves[2], false));
            }
        }
    }

    public void ShowTowerUpgradeInfo(){
        //this.towerUpgradeMenu.transform.localScale = new Vector3(1f, 1f, 1f);
        this.towerUpgradeMenu.SetActive(true);
        this.towerUpgradeCostMenu.SetActive(true);
        this.isHovering = true;
    }

    public void HideTowerUpgradeInfo(){
        //this.towerUpgradeMenu.transform.localScale = new Vector3(0f, 0f, 0f);
        this.towerUpgradeMenu.SetActive(false);
        this.towerUpgradeCostMenu.SetActive(false);
        this.isHovering = false;
    }
}