using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class BFS : MonoBehaviour {

    public static BFS instance;

    private GridPathfinding gpManager;

    private Tile currentQueuedTile;

    void Awake(){
        instance = this;
    }

	// Use this for initialization
	void Start () {
        this.gpManager = GridPathfinding.instance;
    }

    void Update(){
        /// Perhaps let the thread continously search routes
    }

    public void UpdateTiles(){
        Thread myNewThread = new Thread(() => {
            GridPathfinding.instance.grid = UpdateGrid(GridPathfinding.instance.grid, GridPathfinding.instance.width, GridPathfinding.instance.height);
        });
        myNewThread.Start();
        myNewThread.Join();
    }

    public Tile[][] UpdateGrid(Tile[][] grid, int width, int height) {
        for (int i = 0; i < width; i++){
            for (int j = 0; j < height; j++){
                Tile tile = grid[i][j];
                tile.ghostTower = SelectTower.instance.ghostTower;
                if (SelectTower.instance.CanPlaceTower(tile) && tile.tower == null && 
                    GridPathfinding.instance.checkPoint != tile && GridPathfinding.instance.startPoint != tile && 
                    GridPathfinding.instance.endPoint != tile){
                    tile.canPlaceTower = true;
                }
                else
                {
                    tile.canPlaceTower = false;
                }
                tile.ghostTower = null;
            }
        }
        return grid;
    }

    public void SearchRoute(Tile start, Tile checkpoint, Tile end){
        //List<Tile> route = new List<Tile>();
        Thread myNewThread = new Thread(() =>{
            this.SearchRoutes(start, checkpoint, end);
        });
        myNewThread.Start();
        myNewThread.Join();
        //return route;
    }

    // Method excecuted by thread
    public void SearchRoutes(Tile start, Tile checkpoint, Tile end){
        List<Tile> routeFromStartToScheck = this.SearchRouteTo(start, checkpoint);
        List<Tile> routeFromCheckToEnd = this.SearchRouteTo(checkpoint, end);
        this.gpManager.currentRoute.AddRange(routeFromStartToScheck);
        this.gpManager.currentRoute.AddRange(routeFromCheckToEnd);
    }

    public Tile BuildGrid(){
        for (int i = 0; i < this.gpManager.width; i++){
            for (int j = 0; j < this.gpManager.height; j++){
                Tile root = this.gpManager.grid[i][j];
                if (this.gpManager.CheckGridLocation(i + 1, j)) { root.isAdjacentTo(this.gpManager.grid[i + 1][j]); }
                if (this.gpManager.CheckGridLocation(i - 1, j)) { root.isAdjacentTo(this.gpManager.grid[i - 1][j]); }
                if (this.gpManager.CheckGridLocation(i, j + 1)) { root.isAdjacentTo(this.gpManager.grid[i][j + 1]); }
                if (this.gpManager.CheckGridLocation(i, j - 1)) { root.isAdjacentTo(this.gpManager.grid[i][j - 1]); }
            }
        }
        return this.gpManager.startPoint;
    }

    public void CalculateNewRoute(Tile start, Tile checkpoint, Tile end){
        //this.gpManager.RemoveColorsOldRoute();
        this.gpManager.currentRoute = new List<Tile>();
        // Extend to use more then 1 checkpoint

        this.SearchRoute(start, checkpoint, end);
        /*List<Tile> routeFromStartToScheck = this.SearchRoute(start, checkpoint); // was SearchRouteTo
        List<Tile> routeFromCheckToEnd = this.SearchRoute(checkpoint, end); // was SearchRouteTo
        this.gpManager.currentRoute.AddRange(routeFromStartToScheck);
        this.gpManager.currentRoute.AddRange(routeFromCheckToEnd);
        this.gpManager.AddColorsNewRoute();*/
    }

    public List<Tile> SearchRouteTo(Tile root, Tile target){
        // De root is verticy 0
        Queue<Tile> traverseOrder = new Queue<Tile>();

        Queue<Tile> Q = new Queue<Tile>(); // Dequeue voor de tiles die bezocht moeten worden
        HashSet<Tile> S = new HashSet<Tile>(); // 
        Q.Enqueue(root);
        S.Add(root);
        Tile targetTile = null;
        int distanceFromRoot = 0;
        root.distanceFromRoot = distanceFromRoot;

        while (Q.Count > 0 && targetTile == null){
            Tile t = Q.Dequeue();
            this.Highlight(t.tileObject);
            // Voeg de tile aan de bezochte tiles toe
            traverseOrder.Enqueue(t); 
            // Check of de tile de locatie heeft van de target
            if (t.x.Equals(target.x) && t.z.Equals(target.z)){ 
                targetTile = t;
            }
            // Voor alle adjacent tiles van 't' kijk welke nog niet bezocht zijn en voeg deze aan de queue toe
            foreach (Tile friend in t.adjacentTiles){ 
                if (!S.Contains(friend)){
                    if (friend.tower == null){
                        // De afstand van adjacent tiles is 1 hoger dan 't's afstand tot de root
                        friend.distanceFromRoot = t.distanceFromRoot + 1; 
                        friend.previousTile = t;
                        if (friend.ghostTower == null){
                            Q.Enqueue(friend);
                            S.Add(friend);
                        }
                    }
                }
            }
        }
        if(targetTile == null){
            return null;
        }
        Tile currentTile = targetTile;
        float distanceToRoot = currentTile.distanceFromRoot;
        List<Tile> route = new List<Tile>();
        route.Add(target);
        while (distanceToRoot != 0){
            currentTile = currentTile.previousTile;
            distanceToRoot = currentTile.distanceFromRoot;
            route.Add(currentTile);
        }
        route.Reverse();
        return route;
    }

    public void Traverse(Tile root){
        Queue<Tile> traverseOrder = new Queue<Tile>();
        Queue<Tile> Q = new Queue<Tile>();
        HashSet<Tile> S = new HashSet<Tile>();
        Q.Enqueue(root);
        S.Add(root);

        while (Q.Count > 0){
            Tile t = Q.Dequeue();
            traverseOrder.Enqueue(t);
            foreach (Tile tile in t.adjacentTiles){
                if (!S.Contains(tile)){
                    Q.Enqueue(tile);
                    S.Add(tile);
                }
            }
        }

        /*while (traverseOrder.Count > 0){
            Tile t = traverseOrder.Dequeue();
        }*/
    }

    private void Highlight(GameObject target){
        /*if (this.currentQueuedTile != null){ // Unselect old target if there is one
            GameObject oldTarget = this.currentQueuedTile.tileObject.transform.FindChild("Bottom").gameObject;
            oldTarget.GetComponent<MeshRenderer>().material = this.gpManager.yellow;
        }
        this.currentQueuedTile = this.gpManager.GetTile(target);
        GameObject newTarget = this.currentQueuedTile.tileObject.transform.FindChild("Bottom").gameObject;
        newTarget.GetComponent<MeshRenderer>().material = this.gpManager.green;*/
    }
}