using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour{

    public AudioSource source;
    public AudioClip[] clips;
    public float minInterval;
    public float maxInterval;
    public float interval;
    public float timeBeforeStarting;

    [Range(0.0F, 1.0F)]
    public float minVolume;
    [Range(0.0F, 1.0F)]
    public float maxVolume;
    
    [Range(0.5F, 2.0F)]
    public float minPitch = 1;
    [Range(0.5F, 2.0F)]
    public float maxPitch = 1;

    public bool randomize = false;
    public bool loop = false;
    public bool isPlaying = false;
    public bool playOnLoadScene = false;

    public float timeTillNextShot = -1f;

    public float timeSongInitiated;

    // Use this for initialization
    void Start(){

    }

    // Update is called once per frame
    void Update(){
        if (this.playOnLoadScene){
            this.isPlaying = true;
            this.playOnLoadScene = false;
        }
        if (this.isPlaying){
            if (this.timeBeforeStarting < 0.0f){
                if (!this.loop){
                    AudioClip clip = this.ChooseClipAndPlay();
                    if (this.randomize){
                        this.source.volume = Random.Range(this.minVolume, this.maxVolume);
                        this.source.pitch = Random.Range(this.minPitch, this.maxPitch);
                    }
                    this.source.PlayOneShot(clip);
                }
                else{
                    // Loop the sound
                    if (this.timeTillNextShot < 0.0f){
                        AudioClip clip = this.ChooseClipAndPlay();
                        if (this.randomize){
                            this.source.volume = Random.Range(this.minVolume, this.maxVolume);
                            this.source.pitch = Random.Range(this.minPitch, this.maxPitch);
                        }
                        this.source.PlayOneShot(clip);
                        if (this.maxInterval > 0){
                            this.timeTillNextShot = Random.Range(this.minInterval, this.maxInterval);
                        }
                        else{
                            this.timeTillNextShot = clip.length;
                        }
                    }
                    else{
                        this.timeTillNextShot -= Time.deltaTime;
                    }
                }
            }
            else{
                this.timeBeforeStarting -= Time.deltaTime;
            }
        }
    }

    public AudioClip ChooseClipAndPlay(){
        AudioClip clipToPlay = this.clips[0];
        if (clips.Length > 1){
            // Choose a random clip
            int random = Random.Range(0, clips.Length);
            clipToPlay = clips[random];
        }
        if (!this.loop){
            this.isPlaying = false;
        }

        return clipToPlay;
    }

    public void PlaySound(){
        this.isPlaying = true;
    }

    public void StopPlaying(){
        this.isPlaying = false;
    }

    public void SetTimeSongStarted(float time){
        this.timeSongInitiated = time;
    }

    public float GetTimeLeft(){
        float timeElapsed = Time.time - this.timeSongInitiated;
        float clipLength = this.clips[0].length;
        float timeLeft = clipLength - timeElapsed;
        return timeLeft;
    }
}
