using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMovement : MonoBehaviour {

    //Points
    public List<Tile> route = new List<Tile>();

    public Enemy enemyScript;
    public GameObject destination;
    public int startPoint = 0;
    private int currentPoint;
    public int endPoint;
    public bool hasReachedEnding = false;

    // Use this for initialization
    void Start(){
        this.enemyScript = this.GetComponent<Enemy>();
    }

    public int GetCurrentPoint(){
        return this.currentPoint;
    }

    public void SetCurrentPoint(int currentPoint){
        this.currentPoint = currentPoint;
    }

    public void SetRoute(List<Tile> route){
        this.route = route;
        this.destination = this.route[this.startPoint].tileObject;
        this.endPoint = this.route.Count - 1;
    }

    // Update is called once per frame
    void Update(){
        if (!this.enemyScript.isDead){
            List<Tile> currentRoute = new List<Tile>();
            currentRoute = GridPathfinding.instance.GetRoute();
            
            if (currentRoute != null){
                this.SetRoute(currentRoute);
            }

            // Upon reaching a node calculate distance to next node and determine speed in all directions (x & y)
            if (this.destination != null){
                this.destination = this.route[this.currentPoint].tileObject;
                this.Move();
                this.CheckCloseToTarget();
            }
        }
    }

    void Move(){
        if (this.enemyScript.currentSpeed > 0.0f){
            this.transform.LookAt(this.destination.transform);
            var step = this.enemyScript.currentSpeed * Time.deltaTime;
            this.transform.position = Vector3.MoveTowards(this.transform.position, this.destination.transform.position, step);
        }
    }

    void CheckCloseToTarget(){
        float distance = Vector3.Distance(this.transform.position, this.destination.transform.position);
        if(distance < 2.5f){
            if (this.currentPoint + 1 <= this.endPoint){
                this.currentPoint++;
            }
            else{
                // Reach the end... Die!!!!
                if (!this.hasReachedEnding){
                    Player.instance.LooseLife();
                    enemyScript.health = 0;
                    this.hasReachedEnding = true;
                }
            }
        }
    }
}