using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveInfoDisplayer : MonoBehaviour {

    public Image currentWave;
    public Image nextWave;
    public Transform seperatorStartPoint;
    public Transform seperatorEndPoint;
    public GameObject seperator;
    public Text currentWaveType;
    public Text nextWaveType;
    public Text waveNr;
    public string waveText = "Wave : ";

	// Use this for initialization
	void Start () {
	
	}

    void Update(){
        // Update the Size of the images according to the times of the wave to come 
        this.UpdateWaveSeperator();
        this.UpdateWaveTypeText();
        this.waveNr.text = this.waveText + Waves.instance.totalWaveNr.ToString();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        // Update the colors of the images according to the damageTypes of the waves to come   
        DamageType currentDamageType = Waves.instance.currentWaveDmgType;
        DamageType nextDamageType = Waves.instance.nextWaveDmgType;
        EnemyTypeColor currentColor = Player.instance.enemyTypeColor.Find(x => x.enemyType == currentDamageType);
        EnemyTypeColor nextColor = Player.instance.enemyTypeColor.Find(x => x.enemyType == nextDamageType);
        this.currentWave.color = new Color(currentColor.color.r, currentColor.color.g, currentColor.color.b);
        this.nextWave.color = new Color(nextColor.color.r, nextColor.color.g, nextColor.color.b);
    }

    private void UpdateWaveSeperator(){
        float full = Waves.instance.GetTotalWaveTime();
        float current = Waves.instance.GetCurrentWaveTime();
        float percentage = current / full;
        this.currentWave.fillAmount = percentage;
        float waveBarLength = this.seperatorStartPoint.position.x - this.seperatorEndPoint.position.x;
        float xPosition = this.seperatorEndPoint.position.x + (waveBarLength * percentage);
        this.seperator.transform.position = new Vector3(xPosition, this.seperator.transform.position.y);
        if(percentage < 0.5f){
            this.nextWaveType.gameObject.SetActive(true);
            this.currentWaveType.gameObject.SetActive(false);
        }
        else{
            this.nextWaveType.gameObject.SetActive(false);
            this.currentWaveType.gameObject.SetActive(true);
        }
    }

    private void UpdateWaveTypeText(){
        int currentWaveNr = Waves.instance.currentWaveIndex;
        this.currentWaveType.text = Waves.instance.waveOrder[currentWaveNr].type.ToString();
        if (currentWaveNr + 1 >= Waves.instance.waveOrder.Length){
            currentWaveNr = -1;
        }
        this.nextWaveType.text = Waves.instance.waveOrder[currentWaveNr + 1].type.ToString();
    }
}