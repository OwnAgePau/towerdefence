using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TowerManager : MonoBehaviour {

    public static TowerManager instance;

    void Awake(){
        instance = this;
    }

    private List<Tower> towers = new List<Tower>();

    public void AddTower(Tower tower){
        this.towers.Add(tower);
        Player.instance.AddPower(tower.damage);
    }

    public void RemoveTower(Tower tower){
        if (this.towers.Contains(tower)){
            this.towers.Remove(tower);
            Player.instance.RemovePower(tower.damage);
        }
    }

    public List<Tower> GetAllTowers(){
        return this.towers;
    }

    public void RemoveEnemyFromTower(GameObject enemy){
        foreach(Tower tower in this.towers){
            tower.RemoveEnemy(enemy);
        }
    }
}