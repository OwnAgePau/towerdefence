using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MultipleLerper : MonoBehaviour {

    [Header("Objects to Lerp")]
    public GameObject[] objectsToLerp;

    [Header("Lerp To Multiply")]
    public Lerper lerp;

    private Dictionary<GameObject, LerpWrapper> lerps = new Dictionary<GameObject, LerpWrapper>();

    private bool[] objectsEnabled;

    private bool isLoaded = false;

    void Awake(){
        this.objectsEnabled = new bool[this.objectsToLerp.Length];
    }

    void OnEnable(){
        if (this.lerp.lerpWrapper.startOnEnable){
            this.LerpAllObjects();
        }
    }

    void OnDisable() {
        for (int i = 0; i < this.objectsEnabled.Length; i++) {
            this.objectsEnabled[i] = false;
        }
        this.StopLerps();
    }

    void FixedUpdate() {
        if (this.lerp.isLoaded && !this.isLoaded) {
            this.CreateLerps();
            this.isLoaded = true;
        }
        if (this.lerp.lerpWrapper.startOnEnable) {
            for (int i = 0; i < this.objectsToLerp.Length; i++) {
                this.CheckEnabled(i);
            }
        }
    }

    void CheckEnabled(int index) {
        GameObject go = this.objectsToLerp[index];
        bool isActive = this.objectsEnabled[index];
        if (go.activeSelf) {
            if (!isActive) {
                // It seems this frame this object has been activated do the OnEnable
                this.objectsEnabled[index] = true;
                this.LerpObject(go);
            }
        }
        else {
            if (isActive) {
                // It seems this frame this object has been deactivated so only set to be disabled
                this.objectsEnabled[index] = false;
            }
        }
    }

    void CreateLerps(){
        for(int i = 0; i < this.objectsToLerp.Length; i++){
            GameObject objectToLerp = this.objectsToLerp[i];
            // Copy all the values from all the fields from the standard lerp
            //LerpWrapper newLerp = this.lerp.lerpWrapper;
            LerpWrapper newLerp = new LerpWrapper(this.lerp.lerpWrapper.duration, this.lerp.lerpWrapper.curve, this.lerp.lerpWrapper.startOnAwake, this.lerp.lerpWrapper.startOnEnable, this.lerp.lerpWrapper.startDelay, this.lerp.lerpWrapper.lerpType,
                this.lerp.lerpWrapper.lerpPositionType, this.lerp.lerpWrapper.lerpColorType, this.lerp.lerpWrapper.lerpHueShiftType, this.lerp.lerpWrapper.lerpMaterialType, this.lerp.lerpWrapper.lerpRelative, this.lerp.lerpWrapper.lerpToOrFrom,
                objectToLerp, this.lerp.lerpWrapper.target, this.lerp.lerpWrapper.targetOffset, this.lerp.lerpWrapper.position, this.lerp.lerpWrapper.startPos, this.lerp.lerpWrapper.endPos, this.lerp.lerpWrapper.rotation,
                this.lerp.lerpWrapper.startRotation, this.lerp.lerpWrapper.endRotation, this.lerp.lerpWrapper.scale, this.lerp.lerpWrapper.startScale, this.lerp.lerpWrapper.endScale, this.lerp.lerpWrapper.tempMaterial, this.lerp.lerpWrapper.normalMaterial,
                this.lerp.lerpWrapper.resetOnFinish, this.lerp.lerpWrapper.color, this.lerp.lerpWrapper.startColor, this.lerp.lerpWrapper.endColor, this.lerp.lerpWrapper.startHueColor, 
                this.lerp.lerpWrapper.text == null ? null : objectToLerp.GetComponent<Text>(), this.lerp.lerpWrapper.alpha, this.lerp.lerpWrapper.startAlpha, this.lerp.lerpWrapper.endAlpha, 
                this.lerp.lerpWrapper.applyToChildren, this.lerp.lerpWrapper.goToAfterFinished, this.lerp.lerpWrapper.amountOfLoops, this.lerp.lerpWrapper.loopWithDelay, 
                this.lerp.lerpWrapper.rectTransform == null ? null : objectToLerp.GetComponent<RectTransform>(), this.lerp.lerpWrapper.UI_Position, this.lerp.lerpWrapper.UI_StartPosition, this.lerp.lerpWrapper.UI_EndPosition, 
                objectToLerp.GetComponent<MeshRenderer>(), objectToLerp.GetComponent<ParticleSystemRenderer>());
            this.objectsEnabled[i] = false;
            this.lerps.Add(objectToLerp, newLerp);     
        }
    }

    void LerpAllObjects(){
        foreach (KeyValuePair<GameObject, LerpWrapper> keyValuePair in this.lerps){
            this.lerp.DoLerp(true, false, keyValuePair.Value);
        }
    }

    LerpWrapper GetLerp(GameObject objectToLerp){
        foreach(KeyValuePair<GameObject, LerpWrapper> keyValuePair in this.lerps){
            if (keyValuePair.Key.Equals(objectToLerp)){
                return keyValuePair.Value;
            }
        }
        return null;
    }

    public void LerpObjectBackwards(GameObject objectToLerp) {
        LerpWrapper lerpWrapper = this.GetLerp(objectToLerp);
        this.lerp.DoLerp(true, true, lerpWrapper);
    }

    public void LerpObject(GameObject objectToLerp){
        LerpWrapper lerpWrapper = this.GetLerp(objectToLerp);
        this.lerp.DoLerp(true, false, lerpWrapper);
    }

    public void LerpObject(int listID){
        if(listID < this.objectsToLerp.Length){
            GameObject objectToLerp = this.objectsToLerp[listID];
            this.LerpObject(objectToLerp);
        }
        else{
            UnityEngine.Debug.LogWarning("Multi Lerp can not be done for ID : " + listID + ", as there are only " + this.objectsToLerp.Length + " objects in the list!");
        }
    }

    public void StopLerps() {
        this.lerp.StopLerp();
    }
}