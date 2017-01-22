using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

    public AudioScript powerupPickupSound;
    public AudioScript boosterSound;
    public AudioScript boosterEndSound;
    public AudioScript flatWallsSound;
    public AudioScript flatWallsEndSound;
    public AudioScript invincibilitySound;
    public AudioScript currencyPickupSound;
    public AudioScript currencyPickupSoundPitched;
    public AudioScript scoreIndicatorBreachSound;
    public AudioScript deathSound;
    public AudioScript waterDeathSound;
    public AudioScript slideSound;
    public AudioScript touchWallSound;
    public AudioScript jumpSound;
    public AudioScript reviveSound; 

    public static AudioScript PowerupPickupSound;
    public static AudioScript BoosterSound;
    public static AudioScript BoosterEndSound;
    public static AudioScript FlatWallsSound;
    public static AudioScript FlatWallsEndSound;
    public static AudioScript InvincibilitySound;
    public static AudioScript CurrencyPickupSound;
    public static AudioScript CurrencyPickupSoundPitched;
    public static AudioScript ScoreIndicatorBreachSound;
    public static AudioScript DeathSound;
    public static AudioScript WaterDeathSound;
    public static AudioScript SlideSound;
    public static AudioScript TouchWallSound;
    public static AudioScript JumpSound;
    public static AudioScript ReviveSound;

    void Awake() {
        PowerupPickupSound = this.powerupPickupSound;
        BoosterSound = this.boosterSound;
        BoosterEndSound = this.boosterEndSound;
        FlatWallsSound = this.flatWallsSound;
        FlatWallsEndSound = this.flatWallsEndSound;
        InvincibilitySound = this.invincibilitySound;
        CurrencyPickupSound = this.currencyPickupSound;
        CurrencyPickupSoundPitched = this.currencyPickupSoundPitched;
        ScoreIndicatorBreachSound = this.scoreIndicatorBreachSound;
        DeathSound = this.deathSound;
        WaterDeathSound = this.waterDeathSound;
        SlideSound = this.slideSound;
        TouchWallSound = this.touchWallSound;
        JumpSound = this.jumpSound;
        ReviveSound = this.reviveSound;
    }
}