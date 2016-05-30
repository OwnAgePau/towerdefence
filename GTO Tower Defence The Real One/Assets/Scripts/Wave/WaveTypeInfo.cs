using UnityEngine;
using System.Collections;

public class WaveTypeInfo : MonoBehaviour {

    public int amountOfEnemies;
    public WaveType type;

    public float healthBonusFactor;
    public float speedBonusFactor;
    public float scoreBonusFactor;

    public float enemySpawnInterval;
}

// TO DO : Add extension to allow different wave types like FAST, HORDE & BOSS. To change the health, speed or amount of enemies
// Do this by using both DamageType enum and a new enum called WaveType to determine what enemies and how many
public enum WaveType
{
    NORMAL,
    FAST,
    STRONG,
    HORDE,
    BOSS
}
