using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridPathfinding : MonoBehaviour{

    public static GridPathfinding instance;

    public EditorGridInstantiate editorGrid; // Could be removed
    private BFS bfs;
    private PlaceTower towerPlacementManager;

    public GameObject gridObject;
    public Tile[][] grid;
    public int width = 20;
    public int height = 20;

    public float startPosX = 0;
    public float startPosZ = 0;

    public float tileSize = 4f;

    //Materials
    /*public Material red; // Destination
    public Material green; // Start
    public Material blue; // Obstacle
    public Material yellow; // Movable
    public Material purple; // Route */

    //public GameObject towerPrefab;
    public GameObject gridParent;

    public List<Tile> currentRoute = new List<Tile>();
    public GameObject[] wayPoints;

    public Tile startPoint;
    public TileScript startTile;

    public Tile checkPoint;
    public TileScript tileWaypoint;

    public Tile endPoint;
    public TileScript endTile;

    public bool isColouringCheckPoints = true;

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start(){
        this.towerPlacementManager = PlaceTower.instance;
        this.bfs = BFS.instance;
        // Instead of instantiating the grid load the objects from the scene into the grid, grid is instantiated through editor script
        this.LoadGrid();
        //this.InstantiateGrid();

        //Current test level setup
        // Don't set the points in code, but do it in the editor!
        //this.startPoint = this.grid[12][0];
        //this.checkPoint = this.grid[4][8];
        //this.currentTarget = this.grid[15][14];

        // Temp start tile
        this.startPoint = this.grid[this.startTile.x][this.startTile.z];
        
        //foreach (TileScript tileScript in this.tileWayPoints){
        this.checkPoint = this.grid[this.tileWaypoint.x][this.tileWaypoint.z];
        //}

        // Temp end tile
        this.endPoint = this.grid[this.endTile.x][this.endTile.z];

        // BFS (Route Calculation)
        Tile root = this.bfs.BuildGrid();
        this.bfs.Traverse(root);
        this.bfs.CalculateNewRoute(startPoint, checkPoint, endPoint);
        if(PersistenceData.instance != null){
            PersistenceData.instance.LoadGame();
        }
        
    }

    public List<Tile> GetRoute() {
        return this.currentRoute;
    }

    public void SetRoute(List<Tile> route){
        this.currentRoute = route;
    }

    void LoadGrid(){
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        this.grid = new Tile[this.width][];
        for (int i = 0; i < this.width; i++){
            this.grid[i] = new Tile[this.height];
            for (int j = 0; j < this.height; j++){
                this.grid[i][j] = new Tile(null, i, j);
            }
        }
        for (int j = 0; j < tiles.Length; j++){
            TileScript tile = tiles[j].GetComponent<TileScript>();
            this.grid[tile.x][tile.z].tileObject = tiles[j];
            this.grid[tile.x][tile.z].isObstacle = tile.isObstacle;
            this.grid[tile.x][tile.z].canPlaceTower = !tile.isObstacle;
        }
    }

    void InstantiateGrid(){
        this.grid = new Tile[this.width][];
        for (int i = 0; i < this.width; i++){
            this.grid[i] = new Tile[this.height];
            for (int j = 0; j < this.height; j++){
                float x = this.startPosX + (i * this.tileSize);
                float z = this.startPosZ + (j * this.tileSize);
                GameObject tile = (GameObject)Instantiate(this.gridObject, new Vector3(x, 0, z), this.transform.rotation);
                tile.transform.parent = this.gridParent.transform;
                tile.name = "Tile x : " + i + ", z : " + j;
                this.grid[i][j] = new Tile(tile, i, j);
            }
        }
    }

    public Tile GetTile(GameObject tileObject){
        Tile currentTile = null;
        for (int i = 0; i < this.width; i++){
            for (int j = 0; j < this.height; j++){
                if(this.grid[i][j].tileObject == tileObject){
                    currentTile = this.grid[i][j];
                }
            }
        }
        return currentTile;
    }

    public Tile GetTileWithTower(GameObject tower){
        Tile currentTile = null;
        for (int i = 0; i < this.width; i++){
            for (int j = 0; j < this.height; j++){
                if (this.grid[i][j].tower != null){
                    if (this.grid[i][j].tower == tower){
                        currentTile = this.grid[i][j];
                    }
                }
            }
        }
        return currentTile;
    }

    public bool CheckGridLocation(int x, int z){
        if(x >= 0 && z >= 0 && x < this.width && z < this.height){
            return true;
        }
        else{
            return false;
        }
    }

    public bool CheckIsValidPoint(int x, int z){
        if(this.startPoint.x.Equals(x) && this.startPoint.z.Equals(z)){
            return false;
        }
        else if(this.checkPoint.x.Equals(x) && this.checkPoint.z.Equals(z)){
            return false;
        }
        else if (this.endPoint.x.Equals(x) && this.endPoint.z.Equals(z)){
            return false;
        }
        return true;
    }
}

public class Tile{
    //public string name { get; set; }
    public Tile previousTile { get; set; }
    public GameObject tileObject { get; set; }
    public GameObject tower { get; set; }
    public GameObject ghostTower { get; set; }
    public bool isObstacle = true;
    public bool canPlaceTower = true;
    public int distanceFromRoot { get; set; }
    public int x { get; set; }
    public int z { get; set; }

    List<Tile> adjacentTileList = new List<Tile>();

    public Tile(GameObject tileObject, int x, int z){
        this.tileObject = tileObject;
        this.x = x;
        this.z = z;
    }

    public List<Tile> adjacentTiles{
        get{
            return adjacentTileList;
        }
    }

    public void isAdjacentTo(Tile p){
        adjacentTileList.Add(p);
    }
}