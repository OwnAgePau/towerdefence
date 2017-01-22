using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class API_Gamejolt : MonoBehaviour {

    public Button loginButton;
    public Button leaderBoardsButton;
    public Button trophiesButton;

	// Use this for initialization
	void Start () {
        loginButton.onClick.AddListener(this.Login);
        leaderBoardsButton.onClick.AddListener(this.ShowLeaderBoards);
        trophiesButton.onClick.AddListener(this.ShowTrophies);
    }
	
	// Update is called once per frame
	void Update () {
	}

    public void Login(){
        GameJolt.UI.Manager.Instance.ShowSignIn();
    }

    public void ShowLeaderBoards(){
        GameJolt.UI.Manager.Instance.ShowLeaderboards();
    }

    public void ShowTrophies(){
        GameJolt.UI.Manager.Instance.ShowTrophies();
    }
}
