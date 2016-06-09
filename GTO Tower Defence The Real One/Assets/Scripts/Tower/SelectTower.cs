using UnityEngine;
using System.Collections;

public class SelectTower : MonoBehaviour{

    public GridPathfinding gridPathfinding;
    public BFS bfs;

    public static SelectTower instance;

    [Header("Selection")]
    public GameObject selectedTower;
    public int selectedTowerNr;

    [Header("Placement")]
    public GameObject[] towers;
    public Tower[] towerScripts;
    public Tile hoveringTile;
    public GameObject ghostTower;
    private GhostTower ghostTowerScript;
    public bool canPlaceTower = false;

    public Color colourCanPlace = new Color(0.1f, 0.1f, 0.1f);
    public Color colourCantPlace = new Color(1f, 0.1f, 0.1f);

    public Material canPlace;
    public Material cantPlace;

    public GameObject gridParent;

    public TileScript[] tiles;

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start(){
        this.gridPathfinding = GridPathfinding.instance;
        this.bfs = BFS.instance;
        tiles = this.GetAllTiles();
        this.ghostTowerScript = this.ghostTower.GetComponent<GhostTower>();
        this.towerScripts = new Tower[this.towers.Length];
        for(int i = 0; i < this.towers.Length; i++){
            GameObject towerObject = this.towers[i];
            this.towerScripts[i] = towerObject.transform.FindChild(PlaceTower.instance.towerObjectName).gameObject.GetComponent<Tower>();
        }
    }

    // Update is called once per frame
    void Update(){
        if (this.selectedTower != null){
            LayerMask layerMask = (1 << 11);
            GameObject hitObject = this.GetMouseClick("Tile", layerMask);
            if (hitObject != null){
                Tile tile = gridPathfinding.GetTile(hitObject);
                if (tile.tower == null){
                    if (this.hoveringTile == null || !tile.Equals(this.hoveringTile)){
                        this.hoveringTile = tile;
                        ghostTower.transform.position = hitObject.transform.position;
                        this.hoveringTile.ghostTower = ghostTower;
                        this.CanPlaceTower();                  
                    }
                }
            }

            // Do something cool with the emission
            // Let the emission slowly go from 0.5f - 1f and back etc etc
            if(this.selectedTower != null)
            {
                //this.DoFancyEmissionStuff();
            }
        }
    }

    public void StopPlacingTower(){
        this.selectedTower = null;
        this.selectedTowerNr = -1;
        ghostTower.transform.position = new Vector3(999, 999, 999);
        this.HideAllGridSpots();
    }

    public void CanPlaceTower(){
        Material mat = this.ghostTower.GetComponent<MeshRenderer>().material;
        bool isValid = this.gridPathfinding.CheckIsValidPoint(this.hoveringTile.x, this.hoveringTile.z);
        Tile tile = this.gridPathfinding.grid[this.hoveringTile.x][this.hoveringTile.z];
        if (tile.canPlaceTower) {
            this.canPlaceTower = true;
            mat.SetColor("_EmissionColor", this.colourCanPlace);
        }
        else{
            this.canPlaceTower = false;
            mat.SetColor("_EmissionColor", this.colourCantPlace);
        }
        this.hoveringTile.ghostTower = null;
        this.ghostTower.GetComponent<MeshRenderer>().material = mat;


    // was SearchRouteTo
    /*if (this.bfs.SearchRouteTo(this.gridPathfinding.startPoint, this.gridPathfinding.checkPoint) != null && 
        this.bfs.SearchRouteTo(this.gridPathfinding.checkPoint, this.gridPathfinding.endPoint) != null && isValid){ 
        this.canPlaceTower = true;
        mat.SetColor("_EmissionColor", this.colourCanPlace);
    }
    else{
        this.canPlaceTower = false;
        mat.SetColor("_EmissionColor", this.colourCantPlace);
    }
    this.hoveringTile.ghostTower = null;
    this.ghostTower.GetComponent<MeshRenderer>().material = mat;*/
}

    public bool CanPlaceTower(Tile tile){
        bool isValid = this.gridPathfinding.CheckIsValidPoint(tile.x, tile.z);
        // was SearchRouteTo
        if (this.bfs.SearchRouteTo(this.gridPathfinding.startPoint, this.gridPathfinding.checkPoint) != null && 
            this.bfs.SearchRouteTo(this.gridPathfinding.checkPoint, this.gridPathfinding.endPoint) != null && isValid){
            return true;
        }
        else{
            return false;
        }
    }

    public GameObject GetMouseClick(string tag, int layer){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, layer)){
            if (hit.collider.gameObject != null){
                if (hit.collider.gameObject.tag.Equals(tag)){
                    return hit.collider.gameObject;
                }
            }
        }
        return null;
    }

    public void SetSelectedTower(int towerNr){
        if (!Player.instance.isPaused){
            this.selectedTowerNr = towerNr;
            this.selectedTower = this.towers[towerNr];
            Tower selectedTowerScript = this.towerScripts[towerNr]; // OPTIMIZE : Save these references in the list.
            BFS.instance.UpdateTiles();
            this.ShowAllGridSpots();
            this.ghostTowerScript.range = selectedTowerScript.range;
            this.ghostTowerScript.SetRangeSphereActivity(true);
            this.ghostTowerScript.UpdateTowerRange();
            // Call Method to display tower info (to see what you get when building the tower)  || actually needs to happen when hovering the button
        }
    }

    public void DeselectTower(int towerNr){
        if (!Player.instance.isPaused){
            this.selectedTowerNr = -1;
            this.selectedTower = null;
            // Call Method to display tower info (to see what you get when building the tower)  || actually needs to happen when hovering the button
        }
    }

    private TileScript[] GetAllTiles(){
        return this.gridParent.GetComponentsInChildren<TileScript>();
    }

    public void ShowAllGridSpots(){
        foreach (TileScript tileScript in this.tiles){
            Tile tile = GridPathfinding.instance.grid[tileScript.x][tileScript.z];
            MeshRenderer tileRenderer = tileScript.gameObject.GetComponentInChildren<MeshRenderer>();
            if (!tile.canPlaceTower){
                tileRenderer.material = this.cantPlace;
            }
            tile.ghostTower = null;
            tileRenderer.enabled = true;
        }
    }

    public void HideAllGridSpots(){
        foreach (TileScript tileScript in this.tiles){
            MeshRenderer tileRenderer = tileScript.gameObject.GetComponentInChildren<MeshRenderer>();
            tileRenderer.enabled = false;
        }
    }
}