﻿using UnityEngine;
using System.Collections;
using System.Threading;

public class PlaceTower : MonoBehaviour{

    public static PlaceTower instance;
    public SelectTower towerSelection;

    private GridPathfinding gpManager;
    private BFS bfs;

    public GameObject parent;

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start(){
        gpManager = GridPathfinding.instance;
        bfs = BFS.instance;
    }

    // Update is called once per frame
    void Update(){

    }

    public void UpdateGridVisuals(){
        Thread myNewThread = new Thread(() =>{
            SelectTower.instance.ShowAllGridSpots();
        });
        myNewThread.Start();
        myNewThread.Join();
    }

    public void PlaceTowerOnGrid(GameObject tower){
        //towerSelection.canPlaceTower
        if (towerSelection.canPlaceTower && Player.instance.CheckVillagers() && this.HasEnoughPoints(tower)){
            Tile tile = towerSelection.hoveringTile;
            if (tile != null){
                float x = this.gpManager.startPosX + (tile.x * this.gpManager.tileSize);
                float z = this.gpManager.startPosZ + (tile.z * this.gpManager.tileSize);
                if (this.gpManager.grid[tile.x][tile.z].tower == null){
                    this.SubstractTowerCost(tower);
                    this.PlaceNewTower(tower, tile, x, 0.5f, z);
                    // Visual effect on the tile - Could possibly be removed
                    /*GameObject child = this.gpManager.grid[tile.x][tile.z].tileObject.transform.FindChild("Bottom").gameObject;
                    child.GetComponent<MeshRenderer>().material = this.gpManager.blue;*/
                    towerSelection.ghostTower.transform.position = new Vector3(999, 999, 999);
                    SelectTower.instance.ShowAllGridSpots();
                }
            }
            else{
                Debug.Log("Found tower, can't place another!");
                // Staat al een toren
            }
        }
    }

    public bool HasEnoughPoints(GameObject tower){
        Tower towerScript = tower.GetComponent<Tower>();
        return Player.instance.CheckAspirePoints(towerScript.aspireCost);
    }

    public GameObject PlaceOnGrid(GameObject tower, Tile tile, float x, float y, float z){
        // Tower place 
        // new Vector3(x, tile.tileObject.transform.position.y + y, z)
        // SelectTower.instance.hoveringTile.tileObject.transform.position
        GameObject towerOnGrid = (GameObject)Instantiate(tower, new Vector3(x, tile.tileObject.transform.position.y + y, z), tower.transform.rotation);
        towerOnGrid.transform.parent = parent.transform;
        towerOnGrid.name = tower.name;
        //Tower towerScript = tower.GetComponent<Tower>();
        this.gpManager.grid[tile.x][tile.z].tower = towerOnGrid;
        return towerOnGrid;
    }

    public GameObject PlaceNewTower(GameObject tower, Tile tile, float x, float y, float z){
        GameObject newTower = this.PlaceOnGrid(tower, tile, x, y, z);
        this.bfs.UpdateTiles();
        /// Place below method in a thread for each enemy seperately
        this.bfs.CalculateNewRoute(this.gpManager.startPoint, this.gpManager.checkPoint, this.gpManager.endPoint);
        return newTower;
    }

    public void SubstractTowerCost(GameObject tower){
        Tower towerScript = tower.GetComponent<Tower>();
        Player.instance.RemoveAspirePoints(towerScript.aspireCost);
        Player.instance.RemoveVillager();
    }
}