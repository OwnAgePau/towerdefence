using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HoverInfo : MonoBehaviour {

    public Tower tower;

    public int aspireCost;
    public string nameToShow;
    public bool isDisabled;
    public bool isButton; // Veranderen naar button object

    private Text towerAspireCostTxt;
    private Text towerVillagerCostTxt;
    private Text towerNameTxt;
    private Image towerAspireImg;
    private Image towerVillagerImg;

    // Use this for initialization
    void Start() {
        this.towerAspireCostTxt = GUIcontroller.instance.buildCost.GetComponent<Text>();
        this.towerVillagerCostTxt = GUIcontroller.instance.buildVillagerCost.GetComponent<Text>();
        this.towerNameTxt = GUIcontroller.instance.buildName.GetComponent<Text>();
        this.towerAspireImg = GUIcontroller.instance.hoverImage;
        this.towerVillagerImg = GUIcontroller.instance.towerVillagerCost;
    }

    // Update is called once per frame
    void Update() {
        if (tower != null) {
            this.aspireCost = tower.aspireCost;
            this.nameToShow = tower.gameObject.name;
            this.SetButtonState();
        }
        else if (this.isButton) {
            this.SetButtonState();
            this.nameToShow = this.gameObject.name;
        }
        else {
            this.nameToShow = this.gameObject.name;
        }
    }

    private void SetButtonState() {
        Image image = this.gameObject.GetComponent<Image>();
        Button button = this.gameObject.GetComponent<Button>();
        if (Player.instance.aspirePoints < this.aspireCost || Player.instance.villagers < 1){
            image.color = new Color(0.5f, 0.001f, 0.001f);
            this.isDisabled = true;
            button.enabled = false;
        }
        else{
            image.color = new Color(1f, 1f, 1f);
            this.isDisabled = false;
            button.enabled = true;
        }
    }
    

    // These 2 methods below could better go to GUIcontroller, or even better a new controller for the BuildButtons
    public void SetCostText(int towerNr){
        Tower tower = SelectTower.instance.towers[towerNr].GetComponent<Tower>();
        int cost = tower.aspireCost;
        this.towerNameTxt.text = tower.name;
        this.towerAspireCostTxt.text = cost.ToString();
        this.towerVillagerCostTxt.text = 1.ToString();
        this.towerAspireImg.gameObject.SetActive(true);
        this.towerVillagerImg.gameObject.SetActive(true);
        Outline costOutline = this.towerAspireCostTxt.GetComponent<Outline>();
        Outline nameOutline = this.towerNameTxt.GetComponent<Outline>();
        Outline vilCostOutline = this.towerVillagerCostTxt.GetComponent<Outline>();
        nameOutline.effectColor = Player.instance.goodColor;
        if (cost > Player.instance.aspirePoints){
            costOutline.effectColor = Player.instance.badColor;
            nameOutline.effectColor = Player.instance.badColor;
        }
        else{
            costOutline.effectColor = Player.instance.goodColor;
        }

        if(1 > Player.instance.villagers){
            vilCostOutline.effectColor = Player.instance.badColor;
            nameOutline.effectColor = Player.instance.badColor;
        }
        else{
            vilCostOutline.effectColor = Player.instance.goodColor;
        }
    }

    public void ExitHover(){
        this.towerAspireCostTxt.text = "";
        this.towerNameTxt.text = "";
        this.towerVillagerCostTxt.text = "";
        this.towerAspireImg.gameObject.SetActive(false);
        this.towerVillagerImg.gameObject.SetActive(false);
    }
}