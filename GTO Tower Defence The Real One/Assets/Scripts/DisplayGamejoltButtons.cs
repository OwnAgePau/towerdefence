using UnityEngine;
using System.Collections;

public class DisplayGamejoltButtons : MonoBehaviour {

    public GameObject trophiesButton;
    public GameObject highScoresButton;
    public GameObject loginButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        bool isSignedIn = GameJolt.API.Manager.Instance.CurrentUser != null;
        if (isSignedIn){
            //GameJolt.API.Manager.Instance.CurrentUser.SignOut();
            trophiesButton.SetActive(true);
            highScoresButton.SetActive(true);
            loginButton.SetActive(false);
            PlayerPrefs.SetInt("LoggedIn", 1);
        }
        else{
            trophiesButton.SetActive(false);
            highScoresButton.SetActive(false);
            loginButton.SetActive(true);
        }
    }
}
