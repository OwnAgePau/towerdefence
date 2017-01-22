using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EditorGridInstantiate : MonoBehaviour {

    public GameObject lookAtPoint;
    public bool isInstantiated = false;

    public Tile[][] grid;

    public GridPathfinding bfs;

    void Update(){
        //transform.LookAt(lookAtPoint.transform.position);
        if (!this.isInstantiated){
            this.InstantiateGrid();
            this.isInstantiated = true;
        }
        if (grid != null){
            Debug.Log(grid.Length);
        }
    }
    // Use this for initialization
    void Start () {
	    
	}

    void InstantiateGrid(){
        this.grid = new Tile[this.bfs.width][];
        for (int i = 0; i < this.bfs.width; i++){
            this.grid[i] = new Tile[this.bfs.height];
            for (int j = 0; j < this.bfs.height; j++){
                float x = this.bfs.startPosX + (i * this.bfs.tileSize);
                float z = this.bfs.startPosZ + (j * this.bfs.tileSize);
                Quaternion rotation = new Quaternion(0, 0, 0, 0);
                GameObject tile = (GameObject)Instantiate(this.bfs.gridObject, new Vector3(x, 0, z), rotation);
                tile.transform.parent = this.bfs.gridParent.transform;
                tile.name = "Tile x : " + i + ", z : " + j;
                tile.GetComponent<TileScript>().x = i;
                tile.GetComponent<TileScript>().z = j;
                GameObject tileBottom = tile.transform.FindChild("Bottom").gameObject;
                //tileBottom.GetComponent<MeshRenderer>().material = this.bfs.yellow;
                this.grid[i][j] = new Tile(tile, i, j);
            }
        }
    }
}
