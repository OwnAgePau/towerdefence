using UnityEngine;

public class OnEnableAudioTrigger : MonoBehaviour {

    public AudioScript sound;

    public bool fadeIn = false;

	void OnEnable() {
        if (fadeIn) {
            sound.FadeIn();
        }
        else {
            sound.PlaySound();
        }
    }
}