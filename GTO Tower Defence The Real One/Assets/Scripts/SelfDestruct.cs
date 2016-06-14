using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {

    public float timeUntillDestruction = 5f;
    public float destructionTime = 3f;
	
	// Update is called once per frame
	void Update () {
        if (this.timeUntillDestruction > 0f){
            this.timeUntillDestruction -= Time.deltaTime;
        }
        else{
            // Change this to go back to the explosionHandler
            ExplosionHandler.instance.AddActiveExplosionToPool(this.gameObject);
            this.timeUntillDestruction = this.destructionTime;
            //Destroy(this.gameObject);
        }
	}
}