using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopUpText : MonoBehaviour {

    public Text textComponent;

    /*public Text textComponent;
    public Color textColor;
    public float currentAlpha = 1.0f;
    public float moveUpAmount = -10f;*/

    public float duration = 1f;
    private float currentDuration = 0f;

	// Use this for initialization
	void OnEnable () {
        this.currentDuration = this.duration;
	}
	
	// Update is called once per frame
	void Update () {
        if(this.currentDuration > 0f) {
            this.currentDuration -= Time.deltaTime;
        }
        else {
            this.gameObject.SetActive(false);
        }
	}
}
