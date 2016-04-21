using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class API_Facebook : MonoBehaviour {

    public static API_Facebook instance;

    void Awake(){
        instance = this;
    }

	// Use this for initialization
	void Start () {
        //FB.Init();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Login(){
        List<string> permissions = new List<string>(){ "public_profile", "email", "user_friends"};
        //FacebookDelegate<IResult> fbDelegate = new FacebookDelegate<IResult>(LoginComplete);
        //FB.LogInWithReadPermissions(permissions, LoginComplete);
    }

    /*public void LoginComplete(IResult result) {
        Debug.Log("Login Result : " + result);
    }*/
}
