using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class PersistenceData : MonoBehaviour {

    public static PersistenceData instance;
    public Controls controls;
    public string mainMenuName;
    public bool isSaved = false;

    public void Awake(){
        instance = this;
    }

    public void LoadGame(){
        int gameState = PlayerPrefs.GetInt("GameState");
        if (gameState.Equals(1)){
            this.Load();
        }
    }

    public void RemoveSaveGame(){
    }

    public void UploadHighscore(){
        int highScore = GUIcontroller.instance.GetHighscore();
        string scoreText = "Highscore : " + highScore; // A string representing the score to be shown on the website.
        int tableID = 123206; // Set it to 0 for main highscore table.
        string extraData = ""; // This will not be shown on the website. You can store any information.
        string guestName = "Guest";
        Debug.Log(scoreText);
        int loggedIn = PlayerPrefs.GetInt("LoggedIn");
        if (loggedIn.Equals(0)) {
            GameJolt.API.Scores.Add(highScore, scoreText, guestName, tableID, extraData, (bool success) => {
                Debug.Log(string.Format("Score Add {0}.", success ? "Successful" : "Failed"));
            });
        }
        else{
            GameJolt.API.Scores.Add(highScore, scoreText, tableID, extraData, (bool success) => {
                Debug.Log(string.Format("Score Add {0}.", success ? "Successful" : "Failed"));
            });
        }
    }

    public void ExitToMainMenu(){
        controls.PauseMenu(false);
        this.ExitGame();
    }

    public void ExitGame(){
        PlayerPrefs.SetInt("GameState", 0);
        //PlayerPrefs.SetInt("LoggedIn", 1);
        //controls.PauseGame();
        Application.LoadLevel(mainMenuName);
    }

    public void Save(){
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        PlayerData playerData = this.CreatePlayerData();
        bf.Serialize(file, playerData);
        file.Close();
        
        file = File.Create(Application.persistentDataPath + "/towersInfo.dat");
        TowersData towersData = PersistenceTowers.instance.CreateTowersData();
        bf.Serialize(file, towersData);
        file.Close();        

        file = File.Create(Application.persistentDataPath + "/enemiesInfo.dat");
        EnemiesData enemiesData = PersistenceEnemies.instance.CreateEnemiesData();
        bf.Serialize(file, enemiesData);
        file.Close();

        file = File.Create(Application.persistentDataPath + "/bulletInfo.dat");
        BulletsData bulletData = PersistenceTowers.instance.CreateBulletsData();
        bf.Serialize(file, bulletData);
        file.Close();

        file = File.Create(Application.persistentDataPath + "/gridInfo.dat");
        GridData gridData = this.CreateGridData();
        bf.Serialize(file, gridData);
        file.Close();
        this.isSaved = true;
    }

    public void Load(){
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat")){
            file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            this.LoadPlayerData(data);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/towersInfo.dat")){
            file = File.Open(Application.persistentDataPath + "/towersInfo.dat", FileMode.Open);
            TowersData data = (TowersData)bf.Deserialize(file);
            PersistenceTowers.instance.LoadTowersData(data);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/enemiesInfo.dat")){
            file = File.Open(Application.persistentDataPath + "/enemiesInfo.dat", FileMode.Open);
            EnemiesData data = (EnemiesData)bf.Deserialize(file);
            PersistenceEnemies.instance.LoadEnemiesData(data);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/bulletInfo.dat")){
            file = File.Open(Application.persistentDataPath + "/bulletInfo.dat", FileMode.Open);
            BulletsData data = (BulletsData)bf.Deserialize(file);
            PersistenceTowers.instance.LoadBulletsData(data);
            file.Close();
        }
        if (File.Exists(Application.persistentDataPath + "/gridInfo.dat")){
            file = File.Open(Application.persistentDataPath + "/gridInfo.dat", FileMode.Open);
            GridData data = (GridData)bf.Deserialize(file);
            this.LoadGridData(data);
            file.Close();
        }
    }

    public PlayerData CreatePlayerData(){
        Player p = Player.instance;
        return new PlayerData(p.villagers, p.aspirePoints, p.power, p.score, p.enemiesKilled, p.lives, p.enemyHealth, p.enemyScore);
    }

    public void LoadPlayerData(PlayerData data){
        Player p = Player.instance;
        p.villagers = data.villagers;
        p.aspirePoints = data.aspirePoints;
        p.power = data.power;
        p.score = data.score;
        p.enemiesKilled = data.enemiesKilled;
        p.lives = data.lives;
        p.enemyHealth = data.enemyHealth;
        p.enemyScore = data.enemyScore;
    }

    public GridData CreateGridData(){
        return new GridData(GridPathfinding.instance.grid);
    }

    public void LoadGridData(GridData data){
        GridPathfinding pathfinding = GridPathfinding.instance;
        for (int i = 0; i < data.gridFieldsData.Length; i++){
            for(int j = 0; j < data.gridFieldsData[i].Length; j++){
                GridFieldData fieldData = data.gridFieldsData[i][j];
                Tile tile = pathfinding.grid[i][j];
                tile.x = fieldData.xPos;
                tile.z = fieldData.zPos;
                tile.isObstacle = fieldData.isObstacle;
            }
        }
    }
}