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

    [Header("Upgrade Cost")]
    public Outline upgradCost;

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

    // Use this for initialization
    void Start () {
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

    void OnGUI(){
        /*this.hoverImage.enabled = this.isHoverAspireCost;
        this.hoverTowerAspireText.text = this.hoverAspireText;
        this.hoverTowernameText.text = this.hoverText;*/
        /*if (this.isHovering) {
            //Vector3 mousePos = Input.mousePosition;
            // To do check whether you are at the left side or right side of the screen and position the text likewise (so it is readable)
            GUI.Label(new Rect(mousePos.x + 10 - (this.hoverText.Length * 6), Screen.height - (mousePos.y + 40), 200, 40), this.hoverText);
            if (this.isHoverAspireCost){
                GUI.Label(new Rect(mousePos.x + 30 - (this.hoverText.Length * 6), Screen.height - (mousePos.y + 20), 200, 40), this.hoverAspireText);
                GUI.DrawTexture(new Rect(mousePos.x + 10 - (this.hoverText.Length * 6), Screen.height - (mousePos.y + 20), 20, 20), this.hoverAspireImage);
            }
            else{
                GUI.Label(new Rect(mousePos.x + 10 - (this.hoverText.Length * 6), Screen.height - (mousePos.y + 20), 200, 40), this.hoverAspireText);
            }
        }*/
    }
}