using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    private int speed = 20;

    private float x;
    private float y;
    private float z;
    private float movingInput = -1;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.x = this.transform.position.x;
        this.y = this.transform.position.y;
        this.z = this.transform.position.z;
        if (Input.GetKey(KeyCode.W) || this.movingInput.Equals(2)){
            this.CameraMoveUp();
        }
        if (Input.GetKey(KeyCode.A) || this.movingInput.Equals(0)){
            this.CameraMoveLeft();
        }
        if (Input.GetKey(KeyCode.S) || this.movingInput.Equals(3)){
            this.CameraMoveDown();
        }
        if (Input.GetKey(KeyCode.D) || this.movingInput.Equals(1)){
            this.CameraMoveRight();
        }

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.D)){
            this.movingInput = -1;
        }
    }

    public void CameraMoveLeft(){
        this.transform.position = new Vector3(this.x - (this.speed * Time.deltaTime), this.y, this.z);
        this.movingInput = 0;
    }

    public void CameraMoveRight(){
        this.transform.position = new Vector3(this.x + (this.speed * Time.deltaTime), this.y, this.z);
        this.movingInput = 1;
    }

    public void CameraMoveUp(){
        this.transform.position = new Vector3(this.x, this.y, this.z + (this.speed * Time.deltaTime));
        this.movingInput = 2;
    }

    public void CameraMoveDown(){
        this.transform.position = new Vector3(this.x, this.y, this.z - (this.speed * Time.deltaTime));
        this.movingInput = 3;
    }

    public void CameraStopMoving(){
        this.movingInput = -1;
    }
}