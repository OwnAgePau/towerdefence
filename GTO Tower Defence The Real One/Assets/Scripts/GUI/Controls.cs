using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Controls : MonoBehaviour {

    public static Controls instance;

    void Awake(){
        instance = this;
    }

    private GameObject mainGameObject;
    //private int speed = 20;
    private GameObject gameManager;
    public SelectTower selectTowerInstance;
    public PlaceTower placeTowerInstance;
    public TowerInfo towerInfoInstance;
    public GridPathfinding gridInstance;

    public GameObject menu;
    public GameObject[] buttons;

    public AudioScript menuSound;

    [Header("Emission")]
    public float minimalEmission = 0.5f;
    public float maxEmission = 1f;
    public float normalEmission = 0f;
    public Material selectedMaterial;
    public bool isGoingUp = true;
    public bool isEmissionOn = false;

    // Use this for initialization
    void Start () {
        if (GameObject.Find("Hit")){
            this.menuSound = GameObject.Find("Hit").GetComponent<AudioScript>();
        }
        this.mainGameObject = this.transform.gameObject;
        gameManager = this.transform.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        // Is grid tile
        if (Input.GetButtonDown("Fire1")){
            if (this.selectTowerInstance.selectedTower != null){
                this.BuildTower();
            }
            else{
                LayerMask layerMask = (1 << 10);
                GameObject hitObject = SelectTower.instance.GetMouseClick("tower", layerMask);
                if (hitObject != null){
                    if (hitObject.GetComponent<Tower>() != null){
                        this.towerInfoInstance.currentTower = hitObject.GetComponent<Tower>();
                        this.towerInfoInstance.currentTowerObject = hitObject;
                        this.selectedMaterial = this.towerInfoInstance.currentTower.GetComponent<MeshRenderer>().material;
                        this.selectedMaterial.SetColor("_EmissionColor", new Color(1f, 0f, 0f));
                        this.isEmissionOn = true;
                        this.towerInfoInstance.OpenMenu();
                        // TO DO HIGHLIGHT TOWER
                    }
                }
                else{
                    //this.towerInfoInstance.currentTower = null;
                    //this.towerInfoInstance.HideTowerInfo();
                    if (!this.towerInfoInstance.isHovering){
                        if (this.selectedMaterial != null){
                            this.selectedMaterial.SetColor("_EmissionColor", new Color(0f, 0f, 0f));
                            this.selectedMaterial = null;
                        }
                        this.towerInfoInstance.HideMenu();
                        this.isEmissionOn = false;
                    }
                }
            }
        }
        if (Input.GetButtonDown("Fire2")){
            // Cancel Building
            this.selectTowerInstance.StopPlacingTower();
            // Cancel Selection
            this.towerInfoInstance.currentTower = null;
        }
        if (Input.GetButtonDown("Cancel")){
            PersistenceData.instance.isSaved = false;
            this.PauseGame();
        }

        if (this.isEmissionOn){
            this.DoFancyEmissionStuff();
        }
    }

    // Not sure if this is the place for this method yet
    public void DoFancyEmissionStuff(){
        Color emissionColour = this.selectedMaterial.GetColor("_EmissionColor");
        // Use boolean to determine wether flowing up or down with the emission
        if (this.isGoingUp){
            if (emissionColour.r <= 1f){
                emissionColour.r += 1 * Time.deltaTime;
            }
            else {
                this.isGoingUp = false;
            }
        }
        else{
            if (emissionColour.r > 0.5f){
                emissionColour.r -= 1 * Time.deltaTime;
            }
            else{
                this.isGoingUp = true;
            }
        }
        selectedMaterial.SetColor("_EmissionColor", emissionColour);
    }

    public void PauseMenu(){
        PersistenceData.instance.isSaved = false;
        this.PauseGame();
    }

    public void PauseGame(){
        this.menuSound.PlaySound();
        this.menu.SetActive(!this.menu.active);
        Player.instance.isPaused = !Player.instance.isPaused;
        SelectTower.instance.StopPlacingTower();
        if (Time.timeScale > 0.0f){
            Time.timeScale = 0.0f;
        }
        else{
            Time.timeScale = 1.0f;
        }
        foreach (GameObject button in this.buttons){
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.enabled = !buttonScript.enabled;
        }
    }

    void BuildTower(){
        //Tile tile = gridInstance.GetTile(collider);
        placeTowerInstance.PlaceTowerOnGrid(selectTowerInstance.selectedTower);
    }
}