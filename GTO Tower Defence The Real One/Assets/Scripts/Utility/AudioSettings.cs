using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using MovementEffects;

[ExecuteInEditMode]
public class AudioSettings : SingletonMonobehaviour<AudioSettings> {

    public AudioSource audioMusic;
    public AudioSource audioUI;

    public AudioMixerGroup gameMixerGroup;
    public AudioMixerGroup uiMixerGroup;
    public AudioMixerGroup musicMixerGroup;
    public AudioMixerGroup sfxMixerGroup;

    public bool isSoundOn = true;
    public bool isMusicOn = true;

    public int valueSoundOff = -80;
    public int valueSoundOn = 0;

    void Awake() {
        instance = this;
        if (audioUI != null)
            audioUI.outputAudioMixerGroup = uiMixerGroup;
        if (audioMusic != null)
            audioMusic.outputAudioMixerGroup = musicMixerGroup;
    }

    void Start() {
        if (ES2.Exists("isSoundOn") && !GameSettings.instance.isReset) {
            this.isSoundOn = ES2.Load<bool>("isSoundOn");
            this.SetSFXVolume(this.isSoundOn);
        }
        else {
            isSoundOn = true;    
        }

        if (ES2.Exists("isMusicOn") && !GameSettings.instance.isReset) {
            this.isMusicOn = ES2.Load<bool>("isMusicOn");
            this.SetMusicVolume(this.isMusicOn);
        }
        else {
            isMusicOn = true;          
        }
        this.SetSFXVolume(this.isSoundOn);
        this.SetMusicVolume(this.isMusicOn);
    }

    public void ToggleSound(bool isOn) {
        this.isSoundOn = isOn;
        this.FadeSFXVolume(this.isSoundOn);
        ES2.Save<bool>(this.isSoundOn, "isSoundOn");
    }

    public void ToggleMusic(bool isOn) {
        this.isMusicOn = isOn;
        this.FadeMusicVolume(this.isMusicOn);
        ES2.Save<bool>(this.isMusicOn, "isMusicOn");
    }

    void SetSFXVolume(bool isOn) {
        this.sfxMixerGroup.audioMixer.SetFloat("SFXVolume", isOn ? 0 : -80);
    }

    void SetMusicVolume(bool isOn) {
        this.musicMixerGroup.audioMixer.SetFloat("MusicVolume", isOn ? 0 : -80);
    }

    void FadeSFXVolume(bool isOn) {
        float from = isOn ? this.valueSoundOff : this.valueSoundOn;
        float to = isOn ? this.valueSoundOn : this.valueSoundOff;
        Timing.RunCoroutine(this.FadeMixerVolume(from, to, 0.5f, "SFXVolume", this.sfxMixerGroup));
    }

    void FadeMusicVolume(bool isOn) {
        float from = isOn ? this.valueSoundOff : this.valueSoundOn;
        float to = isOn ? this.valueSoundOn : this.valueSoundOff;
        Timing.RunCoroutine(this.FadeMixerVolume(from, to, 0.5f, "MusicVolume", this.musicMixerGroup));
    }

    IEnumerator<float> FadeMixerVolume(float from, float to, float duration, string volumeName, AudioMixerGroup mixer) {
        float curTime = 0f;
        float difference = to - from;
        float currentVolume = 0f;
        float newVolume = 0f;
        float clamped = 0f;
        while (curTime < duration) {
            curTime += Time.deltaTime;
            currentVolume = (curTime / duration) * difference;
            newVolume = (int)(currentVolume + from);
            clamped = Mathf.Clamp(newVolume, this.valueSoundOff, this.valueSoundOn);
            mixer.audioMixer.SetFloat(volumeName, clamped);
            yield return Timing.WaitForSeconds(0.01f);
        }
    }
}