using UnityEngine;
using System.Collections;

public class SpawnCube : MonoBehaviour {

    public GameObject cube;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f)){
            if (hit.collider.gameObject != null){
                Debug.DrawLine(ray.origin, hit.point);
                if (hit.collider.gameObject.layer == 12){
                    // Is grid tile
                    if (Input.GetButtonDown("Fire1")){
                        Debug.Log("Plaats kubus!");
                        this.PlaceTower(hit.point);
                    }
                }
            }
        }
    }

    void PlaceTower(Vector3 hit){
        Vector3 position = new Vector3(hit.x, hit.y + 0.5f, hit.z);
        Instantiate(this.cube, position, this.transform.rotation);
    }
}
