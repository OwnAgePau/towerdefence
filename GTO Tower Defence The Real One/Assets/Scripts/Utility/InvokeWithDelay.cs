using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class InvokeWithDelay : MonoBehaviour{

    public UnityEvent eventToInvoke;

    [HideInInspector]
    public float duration = 1;

    // Use this for initialization
    void OnEnable(){
        if(!GameSettings.instance.isFirstLaunch)
            Invoke("init", duration);
    }

    // Update is called once per frame
    void init(){
        eventToInvoke.Invoke();
        this.enabled = false;
    }
}