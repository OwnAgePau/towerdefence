using UnityEngine;
using System.Collections;

public class NavmeshPathfinding : MonoBehaviour {

    public Transform[] points;
    public int destPoint = 0;
    private NavMeshAgent agent;
    private Enemy enemy;


    void Start(){
        this.agent = this.GetComponent<NavMeshAgent>();
        this.enemy = this.GetComponent<Enemy>();
        this.agent.autoBraking = false;
        this.GotoNextPoint();
    }


    void GotoNextPoint(){
        // Returns if no points have been set up
        if (this.points.Length == 0){
            return;
        }
        
        if(this.destPoint == this.points.Length){
            this.enemy.EnemyReachedVillage();
        }
        else{
            this.agent.destination = this.points[this.destPoint].position;
            this.destPoint++;
        }
        //destPoint = (destPoint + 1) % points.Length;
    }


    void Update(){
        if (this.agent.remainingDistance < 0.5f){
            this.GotoNextPoint();
        }
    }
}