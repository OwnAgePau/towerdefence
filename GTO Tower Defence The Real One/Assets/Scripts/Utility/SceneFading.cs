using UnityEngine;
using System.Collections;

public class SceneFading : SingletonMonobehaviour<SceneFading> {

    public Texture2D fadeOutTexture;
    public float fadeSpeed = 0.3f;

    private int drawDepth = -1000;
    private float alpha = 1.0f;
    private int fadeDir = -1;
    private bool isFading = false;

    void Awake(){
        instance = this;
    }

	void OnGUI(){
        if (this.isFading){
            alpha += fadeDir * fadeSpeed * Time.deltaTime;
            Debug.Log(alpha);
            alpha = Mathf.Clamp01(alpha);

            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
            GUI.depth = drawDepth;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
        }
    }

    public float BeginFade(int direction){
        this.isFading = true;
        fadeDir = direction;
        return fadeSpeed;
    }

    public void Fade(int direction){
        this.isFading = false;
        fadeDir = direction;
    }
}