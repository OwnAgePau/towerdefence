using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MovementEffects;

public class AudioBackgroundController : MonoBehaviour {

    public static AudioBackgroundController instance;

    [Header("UI")]
    public AudioScript ambientUI;

    public static AudioScript AmbientUI;

    [Header("Gameplay")]
    public AudioScript drums;
    public AudioScript melodies;
    
    public static AudioScript Drums;
    public static AudioScript Melodies;

    public float[] queuePointsInMs;
    private List<int> queuePointsInSamples = new List<int>();
    private int lastChosenQueuePoint = -1;

    [Header("Hertz values")]
    public float lowValueUI;
    public float highValueUI;
    public float toGameHigh;

    [Header("Durations")]
    public float fadeInUIMs;
    public float fadeInGameMs;
    public float uiLoopDuration;
    public float lerpToGameLength;

    private bool isLooping;
    private bool isGoingUp;
    private float currentLoopTime;
    private float hertzValue;

    void Awake() {
        instance = this;
        Drums = this.drums;
        Melodies = this.melodies;
        AmbientUI = this.ambientUI;
        for(int i = 0; i < this.queuePointsInMs.Length; i++) {
            this.queuePointsInSamples.Add((int)(this.queuePointsInMs[i] * Drums.clips[0].frequency));
        }
    }

    void Update() {
        if (this.isLooping) {
            if(this.currentLoopTime < this.uiLoopDuration) {
                this.currentLoopTime += Time.deltaTime;
            }
            else {
                this.currentLoopTime = 0f;
                if (this.isGoingUp) {
                    SetCutoffFreq(this.highValueUI, this.lowValueUI, this.uiLoopDuration);
                }
                else {
                    SetCutoffFreq(this.lowValueUI, this.highValueUI, this.uiLoopDuration);
                }
                this.isGoingUp = !this.isGoingUp;
                AmbientUI.source.outputAudioMixerGroup.audioMixer.GetFloat("AmbientCutoffFreq", out this.hertzValue);
            }
        }
    }

    public void ToUI() {
        Drums.FadeOut(this.fadeInUIMs);
        Melodies.FadeOut(this.fadeInUIMs);
        AmbientUI.FadeIn(this.fadeInUIMs);
        this.StartCutoffFreqLoop();
    }

    public void ToGamePlay() {
        List<int> chosenQueuePoints = new List<int>();
        for(int i = 0; i < this.queuePointsInSamples.Count; i++) {
            if(i != this.lastChosenQueuePoint) {
                chosenQueuePoints.Add(i);
            }
        }
        this.lastChosenQueuePoint = chosenQueuePoints[Random.Range(0, chosenQueuePoints.Count)];
        int sampleStart = this.queuePointsInSamples[this.lastChosenQueuePoint];
        Drums.FadeIn(this.fadeInGameMs, sampleStart);
        Melodies.FadeIn(this.fadeInGameMs, sampleStart);
        AmbientUI.FadeOut(this.fadeInGameMs);
        this.StopCutoffFreqLoop();
        this.SetCutoffFreq(this.hertzValue, this.toGameHigh, this.lerpToGameLength);
    }

    public void StopGamePlayMusic() {
        Drums.FadeOut(this.fadeInUIMs);
        Melodies.FadeOut(this.fadeInUIMs);
    }

    public void StartGamePlayMusic() {
        List<int> chosenQueuePoints = new List<int>();
        for (int i = 0; i < this.queuePointsInSamples.Count; i++) {
            if (i != this.lastChosenQueuePoint) {
                chosenQueuePoints.Add(i);
            }
        }
        this.lastChosenQueuePoint = chosenQueuePoints[Random.Range(0, chosenQueuePoints.Count)];
        int sampleStart = this.queuePointsInSamples[this.lastChosenQueuePoint];
        Drums.FadeIn(this.fadeInGameMs, sampleStart);
        Melodies.FadeIn(this.fadeInGameMs, sampleStart);
    }

    public void StartUIAmbient() {
        AmbientUI.FadeIn(this.fadeInUIMs);
        this.StartCutoffFreqLoop();
    }

    public void SetCutoffFreq(float from, float to, float duration) {
        Timing.RunCoroutine(LerpCutoffFreq(from, to, duration));
    }

    private void StartCutoffFreqLoop() {
        this.isLooping = true;
        this.currentLoopTime = this.uiLoopDuration;
    }

    private void StopCutoffFreqLoop() {
        this.isLooping = false;
    }

    static IEnumerator<float> LerpCutoffFreq(float from, float to, float duration) {
        float curDuration = 0f;
        float currentDifference = 0f;
        float difference = to - from;
        while(curDuration < duration) {
            curDuration += Time.deltaTime;
            currentDifference = (curDuration / duration) * difference;
            AmbientUI.source.outputAudioMixerGroup.audioMixer.SetFloat("AmbientCutoffFreq", currentDifference + from);
            yield return Timing.WaitForSeconds(0.01f);
        }
    }
}