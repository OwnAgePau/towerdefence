using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Achievements : MonoBehaviour {

    public static Achievements instance;

    public int totalVillagers = 20;

    void Awake(){
        instance = this;
    }

    private int firstDayID = 48488; // First Day - Kill a 1.000 enemies [x]
    private int firestarterID = 48489; // The Firestarter - Have 20 fire towers in one game [x]
    private int allrdounderID = 48493; // Allrounder - Build at least one tower of every type in one game. [x]
    private int villageHeroID = 48494; // Village Hero - Have a total power of 9.000 or more in one game. [x]
    private int fullHouseID = 48490; // Full House - Upgrade a tower of every type to level 5 or higher in one game. [x]
    private int arenasChampionID = 48495; // Arena's Champion - Have a total power of 50.000 or more in one game. [x]
    private int trueSurviorID = 48491; // True Survivor - Have the 200th villager enter the village in one game. [x]
    private int ironWillID = 48492; // Iron Will - Kill a 1.000.000 enemies in one game.

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(Player.instance.enemiesKilled > 1000){
            // First Day achievement achieved!
            this.UnlockTrophy(this.firstDayID);
        }
        else if(Player.instance.enemiesKilled > 1000000){
            // Iron Will achievement achieved!
            this.UnlockTrophy(this.ironWillID);
        }

	    if(Player.instance.power > 9000){
            // Village Hero achievement achieved!
            this.UnlockTrophy(this.villageHeroID);
        }
        else if(Player.instance.power > 50000){
            // Arena's Champion achievement achieved
            this.UnlockTrophy(this.arenasChampionID);
        }

        if(this.totalVillagers > 200){
            // True Survivor achievement achieved!
            this.UnlockTrophy(this.trueSurviorID);
        }

        int fireTowerCount = 0;
        int mageTowerCount = 0;
        int iceTowerCount = 0;
        int poisonTowerCount = 0;
        int demonTowerCount = 0;
        int earthTowerCount = 0;
        bool fireUpgraded = false;
        bool mageUpgraded = false;
        bool iceUpgraded = false;
        bool poisonUpgraded = false;
        bool demonUpgraded = false;
        bool earthUpgraded = false;
        List<Tower> towers = TowerManager.instance.GetAllTowers();
        if (towers.Count > 0){
            foreach (Tower tower in towers){
                if (tower.type.Equals(DamageType.FIRE)){
                    fireTowerCount++;
                    if (tower.towerLevel > 4){
                        fireUpgraded = true;
                    }
                }
                if (tower.type.Equals(DamageType.DEMON)){
                    demonTowerCount++;
                    if (tower.towerLevel > 4){
                        demonUpgraded = true;
                    }
                }
                if (tower.type.Equals(DamageType.EARTH)){
                    earthTowerCount++;
                    if (tower.towerLevel > 4){
                        earthUpgraded = true;
                    }
                }
                if (tower.type.Equals(DamageType.ICE)){
                    iceTowerCount++;
                    if (tower.towerLevel > 4){
                        iceUpgraded = true;
                    }
                }
                if (tower.type.Equals(DamageType.POISON)){
                    poisonTowerCount++;
                    if (tower.towerLevel > 4){
                        poisonUpgraded = true;
                    }
                }
                if (tower.type.Equals(DamageType.MAGE)){
                    mageTowerCount++;
                    if (tower.towerLevel > 4){
                        mageUpgraded = true;
                    }
                }
            }
        }
        if (fireTowerCount > 20){
            // Firestarter achievement achieved!
            this.UnlockTrophy(this.firestarterID);
        }

        if(fireTowerCount > 0 && mageTowerCount > 0 && poisonTowerCount > 0 && 
            iceTowerCount > 0 && earthTowerCount > 0 && demonTowerCount > 0){
            // Allrounder achievement achieved!
            this.UnlockTrophy(this.allrdounderID);
        }

        if (fireUpgraded && mageUpgraded && poisonUpgraded && iceUpgraded && earthUpgraded && demonUpgraded) {
            // Full house achievement achieved!
            this.UnlockTrophy(this.fullHouseID);
        }
	}

    /*public List<Tower> GetAllTowers(){
        GameObject[] towerObjects = GameObject.FindGameObjectsWithTag("tower");
        List<Tower> towers = new List<Tower>();
        foreach(GameObject towerObject in towerObjects){
            Tower towerScript = towerObject.GetComponent<Tower>();
            towers.Add(towerScript);
        }
        return towers;
    }*/

    public void UnlockTrophy(int trophyID){
        if(GameJolt.UI.Manager.Instance != null){
            int loggedIn = PlayerPrefs.GetInt("LoggedIn");
            if (loggedIn.Equals(1)){
                GameJolt.API.Trophies.Get(trophyID, (GameJolt.API.Objects.Trophy trophy) => {
                    if (trophy != null && !trophy.Unlocked){
                        GameJolt.API.Trophies.Unlock(trophyID, (bool success) => {
                            if (success){
                                Debug.Log("Trophy succesfully achieved! : " + trophyID);
                            }
                            else
                            {
                                Debug.Log("Something went wrong");
                            }
                        });
                    }
                });
            }
        }
    }
}
