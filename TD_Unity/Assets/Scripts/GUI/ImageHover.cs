using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageHover : MonoBehaviour {

    public static ImageHover instance;

    public Text infoPopUp;

    public float yOffset = 10; // Make this scale with the UI, so that it comes out correctly

    void Awake(){
        instance = this;
    }

    public void SetTextOn(string text, Vector3 newPosition){
        /*this.infoPopUp.text = text;
        // Not sure if activation has to change, as the text being empty should be enough to not show it
        //this.infoPopUp.gameObject.SetActive(true);
        Vector3 pos = new Vector3(newPosition.x, this.infoPopUp.transform.position.y, newPosition.z);
        this.infoPopUp.gameObject.transform.position = pos;*/
    }

    // WE MIGHT HAVE TO USE THE GETCOMPONENT HOVER, BECAUSE IT ACTUALLY WORKS THAT WAY.

    public void SetTextOff(){
        //this.infoPopUp.text = "";
        // Not sure if activation has to change, as the text being empty should be enough to not show it
        //this.infoPopUp.gameObject.SetActive(false);
    }


    void Update(){
        // Raycasting to get hoverImage info
        var pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);
        if (raycastResults.Count > 0){
            if (raycastResults[0].gameObject.tag.Equals("ImageHover")){
                ImageHoverInfo hoverInfo = raycastResults[0].gameObject.GetComponent<ImageHoverInfo>();
                this.infoPopUp.text = hoverInfo.info + hoverInfo.extraInfo;
                Vector3 imagePos = raycastResults[0].gameObject.transform.position;
                Vector3 pos = new Vector3(imagePos.x, this.infoPopUp.transform.position.y, imagePos.z);
                this.infoPopUp.gameObject.transform.position = pos;
                this.infoPopUp.gameObject.SetActive(true);
            }
            else{
                this.infoPopUp.text = "";
                this.infoPopUp.gameObject.SetActive(false);
            }
        }
    }
}