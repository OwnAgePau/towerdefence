using UnityEngine;
using System.Collections;

public class TowerRange : MonoBehaviour {

    public GameObject rangeSphere;

    void Start(){
        //this.rangeSphere = this.transform.FindChild("Range").gameObject;
    }

    public void SetTowerRange(float towerRange){
        float range = towerRange * 2;
        this.rangeSphere.transform.localScale = new Vector3(range, range, range);
    }

    public void SetRangeSphereActivity(bool active){
        this.rangeSphere.SetActive(active);
    }
}