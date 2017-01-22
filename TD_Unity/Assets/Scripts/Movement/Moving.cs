using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Moving : MonoBehaviour {

    //Points
    public List<GameObject> points = new List<GameObject>();
    public GameObject destination;
    public int startPoint = 0;
    public int currentPoint;
    public int endPoint;

    //Movement
    public int speed = 5;

	// Use this for initialization
	void Start () {
        this.currentPoint = this.startPoint;
        this.destination = this.points[this.startPoint];
        this.endPoint = this.points.Count;
    }
	
	// Update is called once per frame
	void Update () {
        // Upon reaching a node calculate distance to next node and determine speed in all directions (x & y)
        this.destination = this.points[this.currentPoint];
        this.Move();
	}

    void Move(){
        this.transform.LookAt(this.destination.transform);
        var step = speed * Time.deltaTime;
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.destination.transform.position, step);
    }

    void OnTriggerEnter(Collider col){
        if (col.gameObject.transform.position.x == destination.transform.position.x && col.gameObject.transform.position.y == destination.transform.position.y){
            if (this.currentPoint + 1 <= this.endPoint){
                this.currentPoint++;
            }
            else{
                this.currentPoint = this.startPoint;
            }
        }
    }
}