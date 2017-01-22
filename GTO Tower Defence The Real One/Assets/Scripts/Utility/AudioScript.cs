using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

public class AudioScript : MonoBehaviour{

    public AudioSource source;
    public AudioClip[] clips;

    [Header("Pitch")]
    public bool randomizePitch = false;
    [Range(0.5F, 2.0F)]
    public float minPitch = 1;
    [Range(0.5F, 2.0F)]
    public float maxPitch = 1;

    [Header("Randomize")]
    public bool useNonRepeatOrder;
    private AudioClip[] randomList;
    private AudioClip lastRandomClip;

    [Header("Looping")]
    public bool loop = false;
    public float minInterval;
    public float maxInterval;
    public float interval;

    [Header("Start Delay")]
    public float timeBeforeStarting;
    public bool playOnLoadScene = false;

    [HideInInspector]
    public bool isPlaying = false;

    [HideInInspector]
    public float timeTillNextShot = -1f;

    [HideInInspector]
    public float timeSongInitiated;

    [Header("Sample start")]
    public bool isUsingSampleStartPos;
    public int sampleStart;

    private float normalVolume;

    private AudioClip clip;
    private AudioClip tempClip;
    private List<AudioClip> tempAudioClips;

    void Start() {
        this.normalVolume = this.source.volume;
        this.tempAudioClips = new List<AudioClip>();
    }

    // Update is called once per frame
    void Update(){
        if (this.playOnLoadScene) {
            this.isPlaying = true;
            this.playOnLoadScene = false;
        }
        if (this.isPlaying) {
            if (this.timeBeforeStarting <= 0f){
                if (!this.loop){
                    this.clip = this.ChooseClipToPlay();
                    if (this.randomizePitch) {
                        this.source.pitch = Random.Range(this.minPitch, this.maxPitch);
                    }
                    this.PlayClip(clip);
                }
                else{
                    // Loop the sound
                    bool flagEndOfClip = false;
                    if(this.source.clip != null) {
                        flagEndOfClip = this.source.timeSamples > this.source.clip.length * this.source.clip.frequency;
                    }
                    if (this.timeTillNextShot < 0f || flagEndOfClip) {
                        this.clip = this.ChooseClipToPlay();
                        if (this.randomizePitch) {
                            this.source.pitch = Random.Range(this.minPitch, this.maxPitch);
                        }
                        this.PlayClip(this.clip);
                        if (this.maxInterval > 0) {
                            this.timeTillNextShot = Random.Range(this.minInterval, this.maxInterval);
                        }
                        else{
                            //int samples = this.clip.samples - sampleStart;
                            float secondsSkipped = 0f;
                            if(sampleStart > 0) {
                                secondsSkipped = (float)sampleStart / (float)this.clip.frequency;
                            }
                            this.timeTillNextShot = this.clip.length - secondsSkipped;
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

    public AudioClip ChooseClipToPlay(){
        this.tempClip = this.clips[0];
        if (clips.Length > 1){
            // Choose a random clip
            if (useNonRepeatOrder) {
                this.tempAudioClips.Clear();
                foreach (AudioClip clip in this.clips){
                    if (clip != this.lastRandomClip) {
                        this.tempAudioClips.Add(clip);
                    }
                }
                this.tempClip = this.tempAudioClips[Random.Range(0, this.tempAudioClips.Count)];
            }
            else {
                this.tempClip = clips[Random.Range(0, clips.Length)];
            }
        }
        if (!this.loop){
            this.isPlaying = false;
        }
        this.lastRandomClip = this.tempClip;
        return this.tempClip;
    }

    void PlayClip(AudioClip clip) {
        if (this.isUsingSampleStartPos) {
            this.source.clip = clip;
            this.source.timeSamples = this.sampleStart;
            Debug.Log(this.source.timeSamples);
            this.source.Play();
        }
        else {
            this.source.PlayOneShot(clip);
        }
    }

    public bool IsPlaying() {
        return this.isPlaying;
    }

    public void PlaySound() {
        if (this.source.volume < 1) {
            this.source.volume = 1;
        }
        this.isPlaying = true;
    }

    public void PlaySound(float seconds) {
        this.sampleStart = (int)(seconds * this.clips[0].frequency);
        this.PlaySound();
    }

    public void StopPlaying(){
        this.isPlaying = false;
        this.timeTillNextShot = 0f;
        this.source.Stop();
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

    public float GetSoundLength() {
        return this.clips[0].length;
    }

    /// <summary>
    /// Fade out the current clip, also stops the audiosource from playing this clip
    /// </summary>
    public void FadeOut(float duration = 1f) {
        // Also stop playing the sound after fade out is done
        // Regulate volume
        Timing.RunCoroutine(FadeSound(this.normalVolume, 0, duration));
        this.Invoke("StopPlaying", duration);
    }

    /// <summary>
    /// Fade in a new clip, also starts playing the clip
    /// </summary>
    public void FadeIn(float duration = 0.5f, int timeSamples = 0) {
        this.sampleStart = timeSamples;
        // Do the fade in
        Timing.RunCoroutine(FadeSound(0, this.normalVolume, duration));
        this.PlaySound();
    }

    IEnumerator<float> FadeSound(float from, float to, float duration){
        float curTime = 0f;
        float difference = to - from;
        float currentVolume = 0f;
        while (curTime < duration) {
            curTime += Time.deltaTime;
            currentVolume = (curTime / duration) * difference;
            this.source.volume = currentVolume + from;
            yield return Timing.WaitForSeconds(0.01f);
        }
    }
}