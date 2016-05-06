using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ImageHover : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);
        if (raycastResults.Count > 0){ 
            if (raycastResults[0].gameObject.tag.Equals("Image")){
                HoverInfo info = raycastResults[0].gameObject.GetComponent<HoverInfo>();
                GUIcontroller.instance.isHovering = true;
                GUIcontroller.instance.hoverText = info.nameToShow;
                if (info.aspireCost > 0){
                    GUIcontroller.instance.isHoverAspireCost = true;
                    GUIcontroller.instance.hoverAspireText = info.aspireCost.ToString();
                    GUIcontroller.instance.hoverUpgradeCostText.text = info.aspireCost.ToString();
                }
                else{
                    GUIcontroller.instance.isHoverAspireCost = false;
                }
            }
            else{
                GUIcontroller.instance.isHoverAspireCost = false;
                GUIcontroller.instance.isHovering = false;
                GUIcontroller.instance.hoverText = "";
                GUIcontroller.instance.hoverAspireText = "";
            }
        }  
    }
}