using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour {

    public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    public bool isPlaying = true;

    void Start() {
        /*for(int i = 0; i < this.transform.childCount; i++){
            GameObject child = this.transform.GetChild(i).gameObject;
            this.particleSystems.Add(child.GetComponent<ParticleSystem>());
        }*/
    }

    public void PlayParticles(){
        foreach (ParticleSystem partSystem in this.particleSystems){
            if (!partSystem.isPlaying){
                partSystem.gameObject.SetActive(true);
            }
        }
        isPlaying = true;
    }

    public void StopParticles(){
        foreach (ParticleSystem partSystem in this.particleSystems){
            if (partSystem.isPlaying){
                partSystem.gameObject.SetActive(false);
            }

        }
        isPlaying = false;
    }
}