using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour {

    public Text textComponent;
    public Color textColor;
    public float currentAlpha = 1.0f;
    public float moveUpAmount = -10f;

	// Use this for initialization
	void Start () {
        this.textComponent = this.GetComponent<Text>();
        Vector3 target = new Vector3(this.transform.position.x, this.transform.position.y + moveUpAmount, this.transform.position.z);
        //this.StartCoroutine(LerpFromTable(this.transform.gameObject, target));
	}
	
	// Update is called once per frame
	void Update () {
        //LerpHandler.instance.Fade(0.5, this.)
	    if(this.currentAlpha > 0){
            //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + (moveUpAmount * Time.deltaTime), this.transform.position.z);
            this.currentAlpha -= Time.deltaTime * 1f;
            this.textColor.a = this.currentAlpha;
            this.textComponent.color = this.textColor;     
            if(this.currentAlpha < 0.5){
                Destroy(this.gameObject);
            }       
        }
	}

    /*private IEnumerator LerpFromTable(GameObject obj, Vector3 target){
        yield return StartCoroutine(LerpHandler.instance.LerpPositionTo(1f, obj.transform, obj.transform.position, target, LerpHandler.instance.curves[1]));
    }*/
}
