using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

    public float timeUntillDestruction = 5f;
	
	// Update is called once per frame
	void Update () {
        if (this.timeUntillDestruction > 0f){
            this.timeUntillDestruction -= Time.deltaTime;
        }
        else{
            Destroy(this.gameObject);
        }
	}
}