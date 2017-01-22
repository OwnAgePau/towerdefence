using UnityEngine;
using System.Collections;

public class BulletParticle : MonoBehaviour {

    public Vector3 lookAt;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.LookAt(this.lookAt);
	}
}
