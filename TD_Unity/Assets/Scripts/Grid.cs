using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

    private float startPosX = -40;
    private float startPosZ = -28;

    //Ground Tiles
    /*public int length = 10;
    public int width = 10;
    private int tileSize = 10;
    public GameObject groundTile;
    private GameObject groundParent;*/

    //Grid Tiles
    public int gridLength = 20;
    public int gridWidth = 20;
    private float gridTileSize = 4f;
    public GameObject gridTile;
    private GameObject gridParent;

    public GridLocation[][] grid;

    //Selection
    public GameObject highlightedTile;

    // Use this for initialization
    void Start () {
        //this.groundParent = GameObject.FindGameObjectWithTag("groundParent");
        this.gridParent = GameObject.FindGameObjectWithTag("gridParent");
        this.InitGrid();
        //this.CreateGround();
        this.CreateGrid();
	}

    // Update is called once per frame
    void Update() {
	}

    void InitGrid() {
        this.grid = new GridLocation[this.gridLength][];
        for (int i = 0; i < this.gridLength; i++){
            this.grid[i] = new GridLocation[this.gridWidth];
            for (int j = 0; j < this.gridWidth; j++){
                this.grid[i][j] = new GridLocation(i, j, null);
            }
        }
    }

    /*void CreateGround(){
        for(int i = 0; i < this.length; i++){
            for(int j = 0; j < this.width; j++){
                var x = this.startPosX + (i * this.tileSize);
                var z = this.startPosZ + (j * this.tileSize);
                GameObject groundTile = (GameObject)Instantiate(this.groundTile, new Vector3(x, 0, z), this.transform.rotation);
                groundTile.transform.parent = this.groundParent.transform;
            }
        }
    }*/

    void CreateGrid(){
        for (int i = 0; i < this.gridLength; i++){
            for (int j = 0; j < this.gridWidth; j++){
                float x = this.startPosX + (i * this.gridTileSize);
                float z = this.startPosZ + (j * this.gridTileSize);
                GameObject gridTile = (GameObject)Instantiate(this.gridTile, new Vector3(x, 0.1f, z), this.transform.rotation);
                gridTile.transform.parent = this.gridParent.transform;
                gridTile.layer = 11;
                if (this.grid[i][j].gridItem == null){
                    this.grid[i][j].gridItem = gridTile;
                }
            }
        }
    }

    public void HighlightTile(GameObject tile){
        this.UnHighlightTile();
        this.highlightedTile = tile;
        this.highlightedTile.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    public void UnHighlightTile(){
        if (this.highlightedTile != null){
            this.highlightedTile.GetComponent<MeshRenderer>().material.color = Color.blue;
        }
        this.highlightedTile = null;
    }

    public void PlaceTower(GameObject tile, GameObject tower){
        GridLocation cord = this.GetGridLocation(tile);
        if(cord != null){
            float x = this.startPosX + (cord.x * this.gridTileSize);
            float z = this.startPosZ + (cord.z * this.gridTileSize);
            if (this.grid[cord.x][cord.z].gridItem.GetComponent<Tower>() == null){
                this.grid[cord.x][cord.z].gridItem = (GameObject)Instantiate(tower, new Vector3(x, tile.transform.position.y + 0.5f, z), tower.transform.rotation);
            }
            else{
                
            }
        }
        else{
            Debug.Log("Found tower, can't place another!");
            // Staat al een toren
        }
    }

    GridLocation GetGridLocation(GameObject tile){
        for (int i = 0; i < this.gridLength; i++){
            for (int j = 0; j < this.gridWidth; j++){
                if (this.grid[i][j].gridItem == tile){
                    return this.grid[i][j];
                }
            }
        }
        return null;
    }
}

public class GridLocation{
    public int x;
    public int z;
    public GameObject gridItem;

    public GridLocation(int x, int z, GameObject gridItem){
        this.x = x;
        this.z = z;
        this.gridItem = gridItem;
    }
}