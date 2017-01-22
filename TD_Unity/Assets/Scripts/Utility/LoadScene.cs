using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour {

    public bool onAwake = false;
    public string sceneName = "UserInterface";

    public Image loadingBarFill;

	// Use this for initialization
	void Awake () {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
	}

    void Start() {
        if (this.onAwake) {
            this.GoToScene(this.sceneName);
        }
    }
	
	// Update is called once per frame
	public void GoToScene(string name) {
        StartCoroutine(UtilFunctions.LoadLevelAsync(name, 0.5f, this.loadingBarFill));
    }
}