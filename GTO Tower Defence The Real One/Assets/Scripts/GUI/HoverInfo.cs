using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HoverInfo : MonoBehaviour {

    public Tower tower;

    public int aspireCost;
    public string nameToShow;
    public bool isDisabled;
    public bool isButton; // Veranderen naar button object

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (tower != null) {
            this.aspireCost = tower.aspireCost;
            this.nameToShow = tower.gameObject.name;
            this.SetButtonState();
        }
        else if (this.isButton){
            this.SetButtonState();
            this.nameToShow = this.gameObject.name;
        }
        else{
            this.nameToShow = this.gameObject.name;
        }
	}

    private void SetButtonState(){
        Image image = this.gameObject.GetComponent<Image>();
        Button button = this.gameObject.GetComponent<Button>();
        if (Player.instance.aspirePoints < this.aspireCost){
            image.color = new Color(0.5f, 0.001f, 0.001f);
            this.isDisabled = true;
            button.enabled = false;
        }
        else{
            image.color = new Color(1f, 1f, 1f);
            this.isDisabled = false;
            button.enabled = true;
        }
    }
}
