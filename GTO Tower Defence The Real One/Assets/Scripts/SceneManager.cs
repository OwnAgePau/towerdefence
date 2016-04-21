using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {

    public string sceneName;
    public Slider loadingBar;
    public bool isPressed = false;

    void Start(){
        this.isPressed = false;
    }

    public void NewGame(){
        PlayerPrefs.SetInt("GameState", 0);
        PlayerPrefs.Save();
        //Application.LoadLevel(sceneName);
        if (!this.isPressed){
            this.isPressed = true;
            loadingBar.gameObject.SetActive(true);
            StartCoroutine(LoadLevelAsync(sceneName));
        }
    }

    public void LoadGame(){
        PlayerPrefs.SetInt("GameState", 1);
        PlayerPrefs.Save();
        //Application.LoadLevel(sceneName);
        if (!this.isPressed){
            this.isPressed = true;
            loadingBar.gameObject.SetActive(true);
            StartCoroutine(LoadLevelAsync(sceneName));
        }
    }

    private IEnumerator LoadLevelAsync(string sceneName){
        AsyncOperation ao = Application.LoadLevelAsync(sceneName);
        ao.allowSceneActivation = true; // Put this to false if you don't want it to load
        while (!ao.isDone){
            loadingBar.value = ao.progress;
            yield return new WaitForSeconds(0.001f);
        }
        loadingBar.value = ao.progress;
    }
}
