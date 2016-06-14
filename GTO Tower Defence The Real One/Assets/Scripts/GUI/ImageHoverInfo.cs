using UnityEngine;
using System.Collections;

public class ImageHoverInfo : MonoBehaviour {

    public string info;

    public string extraInfo;

    public void SetTextOn(){
        ImageHover.instance.SetTextOn(this.info + this.extraInfo, this.transform.position);
    }
}
