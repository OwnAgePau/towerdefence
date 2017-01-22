using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

    // You might at some point change this camera movement, because it uses some form of teleportation!
    private int speed = 20;

    [Header("Camera Move Borders")]
    public Transform northernBorder; // + z
    public Transform southernBorder; // - z
    public Transform westernBorder; // - x
    public Transform easternBorder; // + x

    private float x;
    private float y;
    private float z;
    private float movingInput = -1;

    [Header("Zoom")]
    public int zoom = 0;
    public int maxZoom = 10;
    private float startY;
    public float zoomDistance = 10f;

    // Use this for initialization
    void Start () {
        this.startY = this.transform.position.y;
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

        float axis = Input.GetAxis("Mouse ScrollWheel");
        if(axis > 0f){
            // Scrolling up - zooming in
            // You should have a certain amount of strength of this axis per zoomed in position so you can easily scroll to any of the positions and then 
            //Debug.Log(axis);
            int amountOfZooms = (int)(axis * 10);
            this.ZoomIn(amountOfZooms);
        }
        else if(axis < 0f){
            // Scrolling down - zooming out
            int amountOfZooms = -(int)(axis * 10);
            this.ZoomOut(amountOfZooms);
        }
    }

    public void CameraMoveLeft(){
        if (this.transform.position.x >= this.westernBorder.position.x){
            this.transform.position = new Vector3(this.x - (this.speed * Time.deltaTime), this.y, this.z);
            this.movingInput = 0;
        }
    }

    public void CameraMoveRight(){
        if(this.transform.position.x <= this.easternBorder.position.x){
            this.transform.position = new Vector3(this.x + (this.speed * Time.deltaTime), this.y, this.z);
            this.movingInput = 1;
        }
    }

    public void CameraMoveUp(){
        if(this.transform.position.z <= this.northernBorder.position.z){
            this.transform.position = new Vector3(this.x, this.y, this.z + (this.speed * Time.deltaTime));
            this.movingInput = 2;
        }
    }

    public void CameraMoveDown(){
        if (this.transform.position.z >= this.southernBorder.position.z){ 
            this.transform.position = new Vector3(this.x, this.y, this.z - (this.speed * Time.deltaTime));
            this.movingInput = 3;
        }
    }

    public void CameraStopMoving(){
        this.movingInput = -1;
    }

    public void ZoomIn(int amountOfZooms){
        int dif = 0;
        if (this.zoom + amountOfZooms >= this.maxZoom){
            dif = this.maxZoom - this.zoom;
            this.zoom = this.maxZoom;
        }
        else{
            this.zoom = this.zoom + amountOfZooms;
            dif = amountOfZooms;
        }
        this.Zoom(dif, -this.zoomDistance);
    }

    public void ZoomOut(int amountOfZooms){
        int dif = 0;
        if(this.zoom - amountOfZooms <= 0){
            dif = this.zoom;
            this.zoom = 0;
        }
        else{
            this.zoom = this.zoom - amountOfZooms;
            dif = amountOfZooms;
        }
        this.Zoom(dif, this.zoomDistance);
    }

    public void Zoom(int zooms, float zoomDistance){
        for (int i = 0; i < zooms; i++){
            Vector3 newPosition = new Vector3(this.transform.position.x, this.transform.position.y + zoomDistance, this.transform.position.z);
            this.transform.position = newPosition;
        }
    }
}