using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Waves : MonoBehaviour {
    public static Waves instance;

    public GameObject startPoint;
    
    public float breakTime = 10.0f; // Time between waves (a break)
    public float currentBreakTime = 0f;
    public bool isBreak = false;
    public GameObject enemyParent;

    // TO DO: Create a pool of enemies to use, and re-use when they are dead.

    [Header("Wave Info")]
    // Interval between each enemy
    public float enemyInterval = 1.0f;
    // Time before the next enemy spawns
    public float timeTillNextEnemy = 1.0f;
    // The amount of enemies before the stats get an upgrade
    public int upgradeEnemies = 5;
    public int enemiesLeft = 0;
    public DamageType currentWaveDmgType;
    public DamageType nextWaveDmgType;
    public int currentWaveIndex;
    public int totalWaveNr = 0;
    private WaveTypeInfo currentWaveType;
    public WaveTypeInfo[] waveOrder;
    private float totalWaveTime;
    private float currentWaveTime;

    public float GetTotalWaveTime(){
        return this.totalWaveTime;
    }
    public float GetCurrentWaveTime(){
        return this.currentWaveTime;
    }

    [System.Serializable]
    public struct EnemyListObject{
        public DamageType enemyType;
        public string name;

        public EnemyListObject(DamageType key, string name){
            this.enemyType = key;
            this.name = name;
        }
    }
    [Header("SpawnChance")]
    public List<EnemyListObject> enemyList = new List<EnemyListObject>();

    [System.Serializable]
    public struct EnemyTypeChance{
        public DamageType enemyType;
        [Range(0f, 1f)]
        public float chance;

        public EnemyTypeChance(DamageType key, float f){
            this.enemyType = key;
            this.chance = f;
        }
        
        public void SetChance(float chance){
            this.chance = chance;
        }   
    }
    public List<EnemyTypeChance> enemySpawnChance = new List<EnemyTypeChance>();
    private Dictionary<DamageType, float> spawnChance = new Dictionary<DamageType, float>(); // This one shouldn't be needed as you allready have a struct containing the combination

    public GameObject enemy;
    //public string enemyName;

    void Awake(){
        instance = this;
    }

    // Use this for initialization
    void Start () {
        foreach (EnemyTypeChance resDmgType in this.enemySpawnChance){
            this.spawnChance.Add(resDmgType.enemyType, resDmgType.chance);
        }
        this.UpgradeEnemies();
        this.SetWaveType();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        // Check if there is a break
        if (!isBreak) {
            if (this.enemyInterval <= 0.0f){
                this.SpawnEnemies();
                this.SetWaveType();
                this.enemyInterval = this.timeTillNextEnemy;
            }
            else{
                this.enemyInterval -= Time.deltaTime;
            }
            if(this.currentWaveTime > 0.0f){
                this.currentWaveTime -= 1 * Time.deltaTime;
            }
        }
        else{
            if(this.currentBreakTime <= 0.0f){
                this.isBreak = false;
            }
            else{
                this.currentBreakTime -= Time.deltaTime;
            }
        }
    }

    void SetWaveType(){
        if(this.currentWaveType == null || enemiesLeft <= 0){
            this.currentWaveIndex++;
            this.totalWaveNr++;
            if (this.currentWaveIndex >= this.waveOrder.Length){
                this.currentWaveIndex = 0;
            }
            this.RecalculateEnemyTypes();
            this.currentWaveType = this.waveOrder[this.currentWaveIndex];
            this.isBreak = true;
            this.currentBreakTime = this.breakTime;
            this.enemiesLeft = this.currentWaveType.amountOfEnemies;
            this.timeTillNextEnemy = this.currentWaveType.enemySpawnInterval;
            this.totalWaveTime = this.currentWaveType.enemySpawnInterval * this.enemiesLeft + this.breakTime;
            this.currentWaveTime = this.totalWaveTime;
        }
    }

    void SpawnEnemies(){
        GameObject enemyPrefab = this.GetEnemyPrefabToSpawn(currentWaveDmgType);
        GameObject enemy = (GameObject)Instantiate(enemyPrefab, this.startPoint.transform.position, this.startPoint.transform.rotation);
        enemy.name = enemyPrefab.name;
        enemy.transform.parent = this.enemyParent.transform;
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (this.currentWaveType.type.Equals(WaveType.BOSS)){
            // Increase the size of a boss
            Vector3 scale = this.enemy.transform.localScale;
            enemy.transform.localScale = new Vector3(scale.x * 2, scale.y * 2, scale.z * 2);
        }
        enemyScript.fullHealth = (int)((enemyScript.health + Player.instance.enemyHealth) * this.currentWaveType.healthBonusFactor);
        enemyScript.health = (int)(enemyScript.fullHealth);
        enemyScript.currentSpeed = enemyScript.speed * this.currentWaveType.speedBonusFactor;
        enemyScript.score += (int)(Player.instance.enemyScore * this.currentWaveType.scoreBonusFactor);
        this.UpgradeEnemies();
        this.enemiesLeft--;
    }

    void UpgradeEnemies(){
        this.upgradeEnemies--;
        if (this.upgradeEnemies <= 0){
            Player.instance.IncreaseEnemyHealth();
            this.upgradeEnemies = 5;
        }
    }

    void RecalculateEnemyTypes(){
        if (this.nextWaveDmgType.Equals(DamageType.NONE)){
            // Set current and future wave
            this.CalculateEnemyTypeChances();
            this.currentWaveDmgType = this.nextWaveDmgType;
        }
        else{
            this.currentWaveDmgType = this.nextWaveDmgType;
            this.CalculateEnemyTypeChances();
        }
    }

    void CalculateEnemyTypeChances(){
        // Step 0 : Calculate amount of towers per damage type
        // Step 1 : Calculate amount of damage per damage type
        // Step 2 : Find out what percentage of total towers are a certain type for each type
        // Step 3 : Find out what percentage of total damage of a certain type is for each type
        // Step 4 : Use both percentages to create a spawn chance
        // Step 5 : Use percentages to determine what type of enemy to spawn next
        // Step 6 : After spawning several enemies of that type, go to step 0! STILL TO IMPLEMENT!!!
        // STEP 6.1: Display the information on which type is currently spawning! And which type is next! (STILL TO IMPLEMENT)

        GameObject[] towers = GameObject.FindGameObjectsWithTag("tower");
        float totalTowers = towers.Length;
        float totalDamage = 0;
        Dictionary<DamageType, float> damagePerType = new Dictionary<DamageType, float>();
        Dictionary<DamageType, float> towerPercentage = new Dictionary<DamageType, float>();
        Dictionary<DamageType, float> damagePercentage = new Dictionary<DamageType, float>();
        
        if (totalTowers > 0){
            // Get tower type, damage and percentage of total
            // Step 0 & 1 & 2
            foreach (EnemyTypeChance enemySpawnChance in this.enemySpawnChance){
                DamageType type = enemySpawnChance.enemyType;
                float amountOfTowers = this.GetAmountOfTowers(towers, type);
                int totalDamageOfType = this.GetDamageOfTowers(towers, type);
                totalDamage += totalDamageOfType;
                damagePerType.Add(type, totalDamageOfType);
                float towerPercentageOfType = 0.0f;
                if (amountOfTowers != 0){
                    towerPercentageOfType = (float)(amountOfTowers / totalTowers);
                }
                towerPercentage.Add(type, towerPercentageOfType);
            }
            // Step 3
            foreach (DamageType key in damagePerType.Keys){
                float damageOfType = 0;
                damagePerType.TryGetValue(key, out damageOfType);
                if (totalDamage.Equals(0)){
                    totalDamage = 1;
                }
                float damagePercentageOfType = damageOfType / totalDamage;
                damagePercentage.Add(key, damagePercentageOfType);
            }

            // Step 4
            this.CalculateChancePerType(towerPercentage, damagePercentage);
            // Step 5
            this.nextWaveDmgType = this.GetEnemyTypeToSpawn();
        }
        else{
            this.nextWaveDmgType = DamageType.MAGE;
        }
    }

    private void CalculateChancePerType(Dictionary<DamageType, float> towerPercentage, Dictionary<DamageType, float> damagePercentage){
        foreach (DamageType key in towerPercentage.Keys){
            float enemySpawnChance = 0.0f;
            float towerPercentageOfType = 0.0f; // toren 40% kans
            towerPercentage.TryGetValue(key, out towerPercentageOfType);
            float damagePercentageOfType = 0.0f; // Damage 60% van de totale damage
            damagePercentage.TryGetValue(key, out damagePercentageOfType);
            if (damagePercentageOfType > 0.5f){ // 0.40 * 0.60 = 0.24
                // Decrease spawn chance of enemy      
                enemySpawnChance = towerPercentageOfType * damagePercentageOfType;
            }
            else{ // 0.40 * 1.60 = 0.64
                // Increase spawn chance of enemy
                float damagePercentageIncreased = damagePercentageOfType + 1;
                enemySpawnChance = towerPercentageOfType * damagePercentageIncreased;
            }
            if(enemySpawnChance < 0.1f){
                enemySpawnChance = 0.1f;
            }
            this.spawnChance[key] = enemySpawnChance;
            for(int i = 0; i < this.enemySpawnChance.Count; i++){
                if (this.enemySpawnChance[i].enemyType.Equals(key)){
                    this.enemySpawnChance[i] = new EnemyTypeChance(key, this.spawnChance[key]);
                }
            }
        }
    }

    public DamageType GetEnemyTypeToSpawn() {
        List<DamageType> damageTypes = new List<DamageType>();
        foreach (EnemyTypeChance typeChance in this.enemySpawnChance){
            float chanceValue = typeChance.chance * 10;
            for (int i = 0; i < chanceValue; i++){
                damageTypes.Add(typeChance.enemyType);
            }
        }
        int rng = Random.Range(0, damageTypes.Count);
        DamageType rngType = damageTypes[rng];
        return rngType;
    }

    public GameObject GetEnemyPrefabToSpawn(DamageType dmgType){
        List<EnemyListObject> enemiesOfType = new List<EnemyListObject>();
        foreach (EnemyListObject enemyListObject in this.enemyList){
            if (enemyListObject.enemyType.Equals(dmgType)){
                enemiesOfType.Add(enemyListObject);
            }
        }
        int rng = Random.Range(0, enemiesOfType.Count);
        GameObject prefab = Resources.Load(enemiesOfType[rng].name) as GameObject;
        return prefab;
    }

    int GetAmountOfTowers(GameObject[] towers, DamageType type){
        int count = 0;
        foreach (GameObject tower in towers){
            Tower towerScript = tower.GetComponent<Tower>();
            if(towerScript != null){
                if (towerScript.type.Equals(type)){
                    count++;
                }
            }  
        }  
        return count;
    }

    int GetDamageOfTowers(GameObject[] towers, DamageType type) {
        int damage = 0;
        foreach (GameObject tower in towers){
            Tower towerScript = tower.GetComponent<Tower>();
            if (towerScript != null){
                if (towerScript.type.Equals(type)){
                    damage += towerScript.damage;
                }
            }
        }
        return damage;
    }
}