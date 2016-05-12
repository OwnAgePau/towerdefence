using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuSelector : MonoBehaviour {

    [Header("Animation Objects")]
    public GameObject[] menuOptions = new GameObject[5];
    private Vector3[] startLocations = new Vector3[5];

    [Header("Animation Values")]
    public float toMoveOffset = 50f;
    public float toMoveDuration = 0.25f;
    public float toMoveBackDuration = 0.1f;
    public float alphaValue = 40f;

    private GameObject selectedOption;
    private int selectedIndex = 0;

    private Coroutine moveRoutine;
    private Coroutine fadeRoutine;

	// Use this for initialization
	void Start () {
        for(int i = 0; i < this.menuOptions.Length; i++){
            this.startLocations[i] = this.menuOptions[i].transform.position;
        }
        this.SelectMenuOption();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // If down arrow is pressed select next item in the list
            this.DeselectMenuOption();
            this.selectedIndex = this.selectedIndex += 1;
            if (this.selectedIndex >= this.menuOptions.Length)
            {
                this.selectedIndex = 0;
            }
            this.SelectMenuOption();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // If up arrow is pressed select previous item in the list
            this.DeselectMenuOption();
            this.selectedIndex = this.selectedIndex -= 1;
            if (this.selectedIndex < 0)
            {
                this.selectedIndex = this.menuOptions.Length - 1;
            }
            this.SelectMenuOption();
            //this.DeselectMenuOptions();
        }
        // If enter is pressed, select item in the list
    }

    public void HoverMenuOption(int index){
        this.DeselectMenuOption();
        this.selectedIndex = index;
        this.SelectMenuOption();
    }

    public void ClickMenuOption(int index){
        // Open Menu Option
        // And activate the code
    }

    public void SelectMenuOption(){
        this.selectedOption = this.menuOptions[this.selectedIndex];
        // Make the gameobject move to its new position : Slightly to the right
        Vector3 target = this.startLocations[this.selectedIndex];
        target.x += this.toMoveOffset;
        // TO DO: Return Instance of coroutine, save this locally and use this to check wether to deselect or select
        // TO DO: Search for a way of disabling all coroutines on current object.
        this.moveRoutine = StartCoroutine(LerpHandler.instance.LerpPositionTo(this.toMoveDuration, this.selectedOption.transform, this.selectedOption.transform.position, target, LerpHandler.instance.curves[2]));
        // Get the highlight from the gameObjects child
        this.fadeRoutine = StartCoroutine(LerpHandler.instance.Fade(this.toMoveDuration, this.selectedOption, this.alphaValue, LerpHandler.instance.curves[3]));
        // Fade in the background of the text while sliding to the right (use the alpha value of the image)

    }

    public void DeselectMenuOption(){
        Vector3 target = this.startLocations[this.selectedIndex];
        GameObject menuItem = this.menuOptions[this.selectedIndex];
        StartCoroutine(LerpHandler.instance.LerpPositionTo(this.toMoveBackDuration, menuItem.transform, menuItem.transform.position, target, LerpHandler.instance.curves[2]));
        // Get the highlight from the gameObjects child
        StopCoroutine(this.moveRoutine);
        StopCoroutine(this.fadeRoutine);
        StartCoroutine(LerpHandler.instance.Fade(this.toMoveDuration, this.selectedOption, this.alphaValue, LerpHandler.instance.curves[5]));
    }
}