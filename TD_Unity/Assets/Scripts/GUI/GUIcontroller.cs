using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIcontroller : MonoBehaviour {

    public static GUIcontroller instance;

    public bool isUICanvasLoaded = false;

    [Header("Player Resources Info")]
    public Text scoreText;
    public Text lifeText;
    public Text aspireText;
    public Text powerText;
    public Text villagersText;

    [Header("Hover Text Objects")]
    public Image hoverImage;
    public Image towerVillagerCost;
    public Text hoverTowernameText;
    public Text hoverTowerAspireText;
    public Text hoverUpgradeCostText;

    [Header("Building Cost")]
    public GameObject buildCost;
    public GameObject buildName;
    public GameObject buildVillagerCost;

    [Header("Hover Info")]
    public HoverInfo hoverUpgradeObject;
    public string hoverText;
    public string hoverAspireText;
    public Texture hoverAspireImage;
    public bool isHovering;
    public bool isHoverAspireCost;

    [Header("Ending Menu Info")]
    public Text score;
    public Text highScore;

    public GameObject gameSaved;

    public GameObject[] mobileGUIButtons;

    void Awake(){
        instance = this;
    }
	
	// Update is called once per frame
	void Update () {
        if (this.isUICanvasLoaded){
            this.scoreText.text = Player.instance.score.ToString();
            this.lifeText.text = Player.instance.lives.ToString();
            this.aspireText.text = Player.instance.aspirePoints.ToString();
            this.powerText.text = Player.instance.power.ToString();
            this.villagersText.text = Player.instance.villagers.ToString();
            this.score.text = "Your Score : " + Player.instance.score.ToString();
            this.highScore.text = "Your Highscore : " + this.GetHighscore().ToString();
            if (PersistenceData.instance.isSaved){
                gameSaved.SetActive(true);
            }
        }
        // Not sure if this should still be in here
        if (!Application.platform.Equals(RuntimePlatform.WindowsPlayer) && !Application.platform.Equals(RuntimePlatform.WindowsEditor)){
            foreach(GameObject obj in mobileGUIButtons){
                obj.SetActive(true);
            }
        }
    }

    public int GetHighscore(){
        // Get the highscore from Player Prefs
        if(PlayerPrefs.HasKey("Highscore")){
            int highscore = PlayerPrefs.GetInt("Highscore");
            if(Player.instance.score > highscore){
                return Player.instance.score;
            }
            return highscore;
        }
        else{
            return Player.instance.score;
        }
    }
}