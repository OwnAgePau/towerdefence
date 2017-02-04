using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Lerper : MonoBehaviour{

    public LerpWrapper lerpWrapper;

    public bool isLoaded = false;

    void Awake(){
        if (this.lerpWrapper != null) {
            if (this.lerpWrapper.lerpType.Equals(LerpType.Position)) {
                if (this.lerpWrapper.lerpPositionType.Equals(LerpPositionType.Force)) {
                    this.lerpWrapper.rigid = this.lerpWrapper.objectToLerp.GetComponent<Rigidbody>();
                }
            }
            this.lerpWrapper.Init();

            RectTransform rectTransform = this.GetComponent<RectTransform>();
            if(rectTransform != null) {
                this.lerpWrapper.rectTransform = rectTransform;
                if (this.lerpWrapper.UI_StartPosition.Equals(Vector2.zero) && this.lerpWrapper.UI_EndPosition.Equals(Vector2.zero)) {
                    if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                        this.lerpWrapper.UI_StartPosition = this.lerpWrapper.rectTransform.anchoredPosition;
                        this.lerpWrapper.UI_EndPosition = this.lerpWrapper.UI_Position;
                    }
                    else {
                        this.lerpWrapper.UI_StartPosition = this.lerpWrapper.UI_Position;
                        this.lerpWrapper.UI_EndPosition = this.lerpWrapper.rectTransform.anchoredPosition;
                    }
                }
            }

            if(this.lerpWrapper.startScale.Equals(Vector3.zero) && this.lerpWrapper.endScale.Equals(Vector3.zero)) {
                if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                    this.lerpWrapper.startScale = this.lerpWrapper.objectToLerp.transform.localScale;
                    this.lerpWrapper.endScale = this.lerpWrapper.scale;
                }
                else {
                    this.lerpWrapper.startScale = this.lerpWrapper.scale;
                    this.lerpWrapper.endScale = this.lerpWrapper.objectToLerp.transform.localScale;
                }
            }

            if (this.lerpWrapper.startPos.Equals(Vector3.zero) && this.lerpWrapper.endPos.Equals(Vector3.zero)) {
                if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                    this.lerpWrapper.startPos = this.lerpWrapper.objectToLerp.transform.position;
                    this.lerpWrapper.endPos = this.lerpWrapper.position;
                }
                else {
                    this.lerpWrapper.startPos = this.lerpWrapper.position;
                    this.lerpWrapper.endPos = this.lerpWrapper.objectToLerp.transform.position;
                }
            }

            if (this.lerpWrapper.startRotation.Equals(Vector3.zero) && this.lerpWrapper.endRotation.Equals(Vector3.zero)) {
                if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                    this.lerpWrapper.startRotation = this.lerpWrapper.rotation;
                    this.lerpWrapper.endRotation = this.lerpWrapper.objectToLerp.transform.localEulerAngles;
                }
                else {
                    this.lerpWrapper.startRotation = this.lerpWrapper.objectToLerp.transform.localEulerAngles;
                    this.lerpWrapper.endRotation = this.lerpWrapper.rotation;
                }
            }

            // Set starting color

            // Alpha

            // alpha = Text, Image or Material alpha
            // - TO
            // - FROM

            // Get the color from the component
            Color componentColor = new Color();
            if (this.lerpWrapper.lerpChildMaterials.Count > 0) {
                componentColor = MaterialColorHelper.GetColor(this.lerpWrapper.lerpChildMaterials[0]);
            }
            else if (this.lerpWrapper.lerpChildImages.Count > 0) {
                componentColor = this.lerpWrapper.lerpChildImages[0].color;
            }
            else if (this.lerpWrapper.lerpChildTexts.Count > 0) {
                componentColor = this.lerpWrapper.lerpChildTexts[0].color;
            }

            // Set the start and end alpha values or the start and end colours
            if (this.lerpWrapper.lerpColorType.Equals(LerpColorType.Fade)) {
                // Uses alpha values
                if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                    this.lerpWrapper.startAlpha = componentColor.a;
                    this.lerpWrapper.endAlpha = this.lerpWrapper.alpha;
                }
                else {
                    this.lerpWrapper.startAlpha = this.lerpWrapper.alpha;
                    this.lerpWrapper.endAlpha = componentColor.a;
                }
            }
            else {
                // Uses Color
                if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                    this.lerpWrapper.startColor = componentColor;
                    this.lerpWrapper.endColor = this.lerpWrapper.color;
                }
                else {
                    this.lerpWrapper.startColor = this.lerpWrapper.color;
                    this.lerpWrapper.endColor = componentColor;
                }
            }

            // Color





            /*if (this.lerpWrapper.lerpChildMaterials.Count > 0) {
                if (this.lerpWrapper.startAlpha == 0f && this.lerpWrapper.endAlpha == 0f) {
                    if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                        Debug.Log(this.lerpWrapper.lerpChildMaterials[0]);
                        this.lerpWrapper.startAlpha = MaterialColorHelper.GetColor(this.lerpWrapper.lerpChildMaterials[0]).a;
                        this.lerpWrapper.endAlpha = this.lerpWrapper.alpha;
                    }
                    else {
                        this.lerpWrapper.startAlpha = this.lerpWrapper.alpha;
                        this.lerpWrapper.endAlpha = MaterialColorHelper.GetColor(this.lerpWrapper.lerpChildMaterials[0]).a;
                    }
                }

                if (this.lerpWrapper.startColor.Equals(new Color(0, 0, 0, 0)) && this.lerpWrapper.endColor.Equals(new Color(0, 0, 0, 0))) {
                    if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                        this.lerpWrapper.startColor = MaterialColorHelper.GetColor(this.lerpWrapper.lerpChildMaterials[0]);
                        this.lerpWrapper.endColor = this.lerpWrapper.color;
                    }
                    else {
                        this.lerpWrapper.startColor = this.lerpWrapper.color;
                        this.lerpWrapper.endColor = MaterialColorHelper.GetColor(this.lerpWrapper.lerpChildMaterials[0]);
                    }
                }
            }

            if(this.lerpWrapper.lerpChildImages.Count > 0) {
                if (this.lerpWrapper.startAlpha == 0f && this.lerpWrapper.endAlpha == 0f) {
                    if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                        this.lerpWrapper.startAlpha = this.lerpWrapper.lerpChildImages[0].color.a;
                        this.lerpWrapper.endAlpha = this.lerpWrapper.alpha;
                    }
                    else {
                        this.lerpWrapper.startAlpha = this.lerpWrapper.alpha;
                        this.lerpWrapper.endAlpha = this.lerpWrapper.lerpChildImages[0].color.a;
                    }
                }

                if (this.lerpWrapper.startColor.Equals(new Color(0, 0, 0, 0)) && this.lerpWrapper.endColor.Equals(new Color(0, 0, 0, 0))) {
                    if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                        this.lerpWrapper.startColor = this.lerpWrapper.lerpChildImages[0].color;
                        this.lerpWrapper.endColor = this.lerpWrapper.color;
                    }
                    else {
                        this.lerpWrapper.startColor = this.lerpWrapper.color;
                        this.lerpWrapper.endColor = this.lerpWrapper.lerpChildImages[0].color;
                    }
                }
            }

            if (this.lerpWrapper.lerpChildTexts.Count > 0) {
                if (this.lerpWrapper.startAlpha == 0f && this.lerpWrapper.endAlpha == 0f) {
                    if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                        this.lerpWrapper.startAlpha = this.lerpWrapper.lerpChildTexts[0].color.a;
                        this.lerpWrapper.endAlpha = this.lerpWrapper.alpha;
                    }
                    else {
                        this.lerpWrapper.startAlpha = this.lerpWrapper.alpha;
                        this.lerpWrapper.endAlpha = this.lerpWrapper.lerpChildTexts[0].color.a;
                    }
                }

                if (this.lerpWrapper.startColor.Equals(new Color(0, 0, 0, 0)) && this.lerpWrapper.endColor.Equals(new Color(0, 0, 0, 0))) {
                    if (this.lerpWrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
                        this.lerpWrapper.startColor = this.lerpWrapper.lerpChildTexts[0].color;
                        this.lerpWrapper.endColor = this.lerpWrapper.color;
                    }
                    else {
                        this.lerpWrapper.startColor = this.lerpWrapper.color;
                        this.lerpWrapper.endColor = this.lerpWrapper.lerpChildTexts[0].color;
                    }
                }
            }*/
        }
        // Save the data of the lerper in a wrapper to be used in a multiple lerper. Eventually in a new project use the Lerpwrapper data instead of the lerper data.
        this.isLoaded = true;
    }

    void OnEnable(){
        if (this.lerpWrapper != null) {
            if (this.lerpWrapper.startOnEnable) {
                this.DoLerp();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="toLerp">When starting the lerp an alternative lerp can be chosen (using all of its data), useful when applying a lerp to multiple targets</param>
    /// <returns></returns>
    public void DoLerp(bool withDelay = true, bool isBackwards = false, LerpWrapper wrapper = null){
        if (wrapper != null){
            this.lerpWrapper = wrapper;
        }
        LerpWrapper currentWrapper = this.lerpWrapper;
        //Debug.Log(this.lerpWrapper + ", " + this.lerpWrapper.lerps);
        /*if (this.lerpWrapper.lerps != null){
            StopAllCoroutines();
            //StopCoroutine(this.lerpWrapper.lerps);
        }*/
        if (currentWrapper.lerps == null) {
            currentWrapper.lerps = new List<Coroutine>();
        }
        // This starts the lerp and when it is finished it calls the 2e lerper
        if (!currentWrapper.objectToLerp.activeInHierarchy) {
            return;
        }
        if(!LerpHandler.instance){
            return;
        }
        currentWrapper.isDone = false;
        currentWrapper.currentLoops = 0;
        if(currentWrapper.startDelay > 0f && withDelay) {
            this.StartCoroutine(this.LerpWithDelay(isBackwards, currentWrapper));
        }
        else {
            this.StartLerp(isBackwards, currentWrapper);
        }    
    }

    void DoLoop(bool withDelay = true, bool isBackwards = false, LerpWrapper wrapper = null) {
        if(wrapper == null) {
            wrapper = this.lerpWrapper;
        }
        if (wrapper.lerps != null) {
            //StopAllCoroutines();
        }
        if (!wrapper.objectToLerp.activeInHierarchy) {
            return;
        }
        if (!LerpHandler.instance) {
            return;
        }
        wrapper.isDone = false;
        if (wrapper.startDelay > 0f && withDelay) {
            this.StartCoroutine(this.LerpWithDelay(isBackwards, wrapper));
        }
        else {
            this.StartLerp(isBackwards, wrapper);
        }
    }

    IEnumerator LerpWithDelay(bool isBackwards, LerpWrapper wrapper) {
        float curDelay = 0f;
        while(curDelay < wrapper.startDelay) {
            curDelay += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        this.StartLerp(isBackwards, wrapper);
    }


    void StartLerp(bool isBackwards, LerpWrapper wrapper) {
        wrapper.currentLoops += 1;
        switch (wrapper.lerpType) {
            case LerpType.Color:
                this.DoColorLerp(isBackwards, wrapper);
                break;
            case LerpType.Position:
                this.DoPositionLerp(isBackwards, wrapper);
                break;
            case LerpType.Rotation:
                this.DoRotationLerp(isBackwards, wrapper);
                break;
            case LerpType.Scale:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.Scale(this, wrapper, isBackwards)));
                break;
        }
    }

    /// <summary>
    /// Different lerps that modify the color of the object
    /// </summary>
    void DoColorLerp(bool isBackwards, LerpWrapper wrapper) {
        switch (wrapper.lerpColorType) {
            case LerpColorType.ColorShift:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.ColorShift(this, wrapper, isBackwards)));
                break;
            case LerpColorType.Fade:
                this.DoFadeLerp(isBackwards, wrapper);
                break;
            case LerpColorType.HueShift:
                this.DoHueShiftLerp(isBackwards, wrapper);
                break;
        }
    }

    /// <summary>
    /// Different lerps that modify the darkness of the color of the object
    /// </summary>
    void DoFadeLerp(bool isBackwards, LerpWrapper wrapper) {
        switch (wrapper.lerpMaterialType) {
            case LerpMaterialType.Mesh:
                for (int i = 0; i < wrapper.lerpChildMaterials.Count; i++) {
                    wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.FadeMeshMaterial(this, wrapper, isBackwards, wrapper.lerpChildMaterials[0])));
                }
                break;
            case LerpMaterialType.Image:
                for (int i = 0; i < wrapper.lerpChildImages.Count; i++) {
                    wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.FadeImage(this, wrapper, isBackwards, wrapper.lerpChildImages[0])));
                }
                break;
            case LerpMaterialType.Particle:
                for (int i = 0; i < wrapper.lerpChildMaterials.Count; i++) {
                    wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.FadeOtherMaterial(this, wrapper, isBackwards, wrapper.lerpChildMaterials[0])));
                }
                break;
            case LerpMaterialType.Other:
                for (int i = 0; i < wrapper.lerpChildMaterials.Count; i++) {
                    wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.FadeOtherMaterial(this, wrapper, isBackwards, wrapper.lerpChildMaterials[0])));
                }
                break;
            case LerpMaterialType.Text:
                for(int i = 0; i < wrapper.lerpChildTexts.Count;i ++) {
                    wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.FadeImage(this, wrapper, isBackwards, wrapper.lerpChildTexts[i])));
                }
                break;
        }
    }

    /// <summary>
    /// Different lerps that modify hue of the object
    /// </summary>
    void DoHueShiftLerp(bool isBackwards, LerpWrapper wrapper) {
        switch (wrapper.lerpHueShiftType) {
            case LerpHueShiftType.Material:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.HueShift(this, wrapper, isBackwards)));
                break;
            case LerpHueShiftType.Text:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.HueShiftText(this, wrapper, isBackwards)));
                break;
        }
    }

    /// <summary>
    /// Different lerps that modify the position of the object
    /// </summary>
    void DoPositionLerp(bool isBackwards, LerpWrapper wrapper) {
        switch (wrapper.lerpPositionType) {
            case LerpPositionType.Force:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpPositionWithForce(this, wrapper, isBackwards)));
                break;
            case LerpPositionType.ForceWithCurve:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpPositionWithForceCurve(this, wrapper, isBackwards)));
                break;
            case LerpPositionType.PointToPoint:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpPositionPointToPoint(this, wrapper, isBackwards)));
                break;
            case LerpPositionType.Point:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpPositionFromObjectTo(this, wrapper, isBackwards)));
                break;
            case LerpPositionType.ObjectWithOffset:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpPositionToTargetWithOffset(this, wrapper, isBackwards)));
                break;
            case LerpPositionType.UI_PointToPoint:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpUIPositionPointToPoint(this, wrapper, isBackwards)));
                break;
            case LerpPositionType.UI_Point:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpUIPositionPointToPoint(this, wrapper, isBackwards)));
                break;
        }
    }

    /// <summary>
    /// Different lerps that modify the rotation of the object
    /// </summary>
    void DoRotationLerp(bool isBackwards, LerpWrapper wrapper) {
        switch (wrapper.lerpRelative) {
            case LerpRelative.NotRelative:
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpRotationVectorTo(this, wrapper, isBackwards)));
                break;
            case LerpRelative.Relative:
                //this.lerpWrapper.startRotation = this.lerpWrapper.objectToLerp.transform.localEulerAngles;
                wrapper.lerps.Add(StartCoroutine(LerpHandler.instance.LerpRotationVectorTo(this, wrapper, isBackwards)));
                break;
        }
    }

    public bool StopLerp(){
        this.lerpWrapper.isDone = true;
        bool isLerping = false;
        if(this.lerpWrapper.lerps != null){
            this.StopAllCoroutines();
            //this.StopCoroutine(this.lerpWrapper.lerps);
            if (this.lerpWrapper.lerpType.Equals(LerpType.Position)) {
                if (this.lerpWrapper.lerpPositionType.Equals(LerpPositionType.Force)) {
                    this.lerpWrapper.rigid.velocity = new Vector3(0, 0, 0);
                }
            }
            else if (this.lerpWrapper.lerpType.Equals(LerpType.Color)) {
                if (this.lerpWrapper.lerpColorType.Equals(LerpColorType.HueShift)) {
                    if (this.lerpWrapper.lerpHueShiftType.Equals(LerpHueShiftType.Material) && this.lerpWrapper.normalMaterial != null) {
                        if (this.lerpWrapper.normalMaterial.color != null)
                        {
                            this.lerpWrapper.normalMaterial.color = this.lerpWrapper.startHueColor;
                        }
                        else {
                            this.lerpWrapper.normalMaterial.SetColor("_TintColor", this.lerpWrapper.startHueColor);
                        }
                    }
                }
            }
            isLerping = true;
            //this.lerpWrapper.lerps = null;
            if(this.lerpWrapper.goToAfterFinished != null){
                this.lerpWrapper.goToAfterFinished.StopLerp();
            }
        }
        return isLerping;
    }

    public void FinishLerp(LerpWrapper wrapper, bool isBackwards){
        //this.StopAllCoroutines();
        wrapper.isDone = true;
        if(wrapper.goToAfterFinished != null){
            wrapper.goToAfterFinished.DoLerp();
        }
        else if(wrapper.currentLoops < wrapper.amountOfLoops) {
            this.DoLoop(wrapper.loopWithDelay, isBackwards, wrapper);
        }
        else if(wrapper.amountOfLoops == -1) {
            this.DoLoop(wrapper.loopWithDelay, isBackwards, wrapper);
        }
        if(wrapper.normalMaterial != null){
            // This stuff needs to be done better, finishing up lerpers should not be this dirty and should be handled by the lerphandler, the lerper should only handle what lerp is going on
            if (wrapper.lerpType.Equals(LerpType.Color)) {
                if (wrapper.lerpColorType.Equals(LerpColorType.Fade)) {
                    switch (wrapper.lerpMaterialType) {
                        case LerpMaterialType.Mesh:
                            wrapper.objectToLerp.GetComponent<MeshRenderer>().material = wrapper.normalMaterial;
                            break;
                        case LerpMaterialType.Particle:
                            wrapper.normalMaterial.SetColor("_TintColor", wrapper.startColor);
                            break;
                    }
                }
            }
        }
    }
}

// Should split this up in main and sub groups (like Rotate, Position, Scale, Visual Effects (fade, hue) as possible main groups)
public enum LerpType{
    Rotation,
    Position,
    Color,
    Scale
}

public enum LerpRelative {
    Relative,
    NotRelative
}

public enum LerpPositionType {
    None,
    ObjectWithOffset,
    Point,
    PointToPoint,
    Force,
    ForceWithCurve,
    UI_PointToPoint,
    UI_Point
}

public enum LerpColorType {
    None,
    Fade,
    HueShift,
    ColorShift
}

public enum LerpHueShiftType {
    None,
    Text,
    Material
}

public enum LerpMaterialType {
    None,
    Image,
    Particle,
    Mesh,
    Other,
    Text
}

public enum ToOrFrom {
    TO,
    FROM
}

[System.Serializable]
public class LerpWrapper{

    public LerpWrapper(float duration, AnimationCurve curve, bool startOnAwake, bool startOnEnable, float startDelay, LerpType lerpType, LerpPositionType lerpPositionType,
        LerpColorType lerpColorType, LerpHueShiftType lerpHueShiftType, LerpMaterialType lerpMaterialType, LerpRelative lerpRelative, ToOrFrom lerpToOrFrom, 
        GameObject objectToLerp, GameObject target, Vector3 targetOffset, Vector3 position, Vector3 startPos, Vector3 endPos, Vector3 rotation, Vector3 startRotation, Vector3 endRotation,
        Vector3 scale, Vector3 startScale, Vector3 endScale, Material tempMaterial, Material normalMaterial, bool resetOnFinish, Color color, Color startColor, Color endColor, Color startHueColor,
        Text text, float alpha, float startAlpha, float endAlpha, bool applyToAllchildren, Lerper goToAfterFinished, int amountOfLoops, bool loopWithDelay, RectTransform rectTransform, 
        Vector2 UI_Position, Vector2 UI_StartPosition, Vector2 UI_EndPosition) {
        this.duration = duration;
        this.curve = curve;
        this.startOnAwake = startOnAwake;
        this.startOnEnable = startOnEnable;
        this.startDelay = startDelay;
        this.lerpType = lerpType;
        this.lerpPositionType = lerpPositionType;
        this.lerpColorType = lerpColorType;
        this.lerpHueShiftType = lerpHueShiftType;
        this.lerpMaterialType = lerpMaterialType;
        this.lerpRelative = lerpRelative;
        this.lerpToOrFrom = lerpToOrFrom;
        this.objectToLerp = objectToLerp;
        this.target = target;
        this.targetOffset = targetOffset;
        this.position = position;
        this.startPos = startPos;
        this.endPos = endPos;
        this.rotation = rotation;
        this.startRotation = startRotation;
        this.endRotation = endRotation;
        this.scale = scale;
        this.startScale = startScale;
        this.endScale = endScale;
        this.tempMaterial = tempMaterial;
        this.normalMaterial = normalMaterial;
        this.resetOnFinish = resetOnFinish;
        this.color = color;
        this.startColor = startColor;
        this.endColor = endColor;
        this.startHueColor = startHueColor;
        this.text = text;
        this.alpha = alpha;
        this.startAlpha = startAlpha;
        this.endAlpha = endAlpha;
        this.applyToChildren = applyToAllchildren;
        this.goToAfterFinished = goToAfterFinished;
        this.amountOfLoops = amountOfLoops;
        this.loopWithDelay = loopWithDelay;
        this.rectTransform = rectTransform;
        this.UI_Position = UI_Position;
        this.UI_StartPosition = UI_StartPosition;
        this.UI_EndPosition = UI_EndPosition;
        lerpChildMaterials = new List<Material>();
        lerpChildImages = new List<Image>();
        lerpChildTexts = new List<Text>();
        lerps = new List<Coroutine>();
        Init();
    }

    public List<Material> lerpChildMaterials;
    public List<Image> lerpChildImages;
    public List<Text> lerpChildTexts;

    public float duration;
    public AnimationCurve curve;
    public bool startOnAwake;
    public bool startOnEnable;
    public float startDelay;

    // Lerp Type fields
    public LerpType lerpType;
    public LerpPositionType lerpPositionType;
    public LerpColorType lerpColorType;
    public LerpHueShiftType lerpHueShiftType;
    public LerpMaterialType lerpMaterialType;
    public LerpRelative lerpRelative;

    public ToOrFrom lerpToOrFrom;

    public GameObject objectToLerp;

    public GameObject target;
    public Vector3 targetOffset;

    public Vector3 position;
    public Vector3 startPos;
    public Vector3 endPos;

    public Vector3 rotation;
    public Vector3 startRotation;
    public Vector3 endRotation;

    public Vector3 scale;
    public Vector3 startScale;
    public Vector3 endScale;

    public Material tempMaterial;
    public Material normalMaterial;

    public bool resetOnFinish;

    public Color color;
    public Color startColor;
    public Color endColor;
    
    public Color startHueColor;
    public Text text;

    [Range(0f, 1f)]
    public float alpha;
    [Range(0f, 1f)]
    public float startAlpha;
    [Range(0f, 1f)]
    public float endAlpha;

    public bool applyToChildren;

    // Looping & Sequences
    public Lerper goToAfterFinished;
    public int amountOfLoops;
    public int currentLoops;
    public bool loopWithDelay;

    public RectTransform rectTransform;
    public Vector2 UI_Position;
    public Vector2 UI_StartPosition;
    public Vector2 UI_EndPosition;

    [HideInInspector]
    public List<Coroutine> lerps = new List<Coroutine>();

    [HideInInspector]
    public Coroutine nextLerp;

    [HideInInspector]
    public bool isDone;

    public Rigidbody rigid;

    public void Init() {
        this.CheckImageChilds();
    }

    void CheckImageChilds() {
        if (this.lerpType.Equals(LerpType.Color)) {
            if (this.lerpMaterialType.Equals(LerpMaterialType.Image)) {
                this.lerpChildImages.Add(this.objectToLerp.GetComponent<Image>());
                if (this.applyToChildren) {
                    this.lerpChildImages.AddRange(this.objectToLerp.transform.GetChildImages());
                }
            }
            else if (this.lerpMaterialType.Equals(LerpMaterialType.Text))  {
                this.lerpChildTexts.Add(this.objectToLerp.GetComponent<Text>());
                if (this.applyToChildren)
                {
                    this.lerpChildTexts.AddRange(this.objectToLerp.transform.GetChildTexts());
                }
            }
            else {
                if (this.normalMaterial != null) {
                    this.lerpChildMaterials.Add(this.normalMaterial);
                }
                else {
                    this.lerpChildMaterials.Add(this.objectToLerp.GetComponent<MeshRenderer>().materials[0]);
                }
                if (this.applyToChildren) {
                    this.lerpChildMaterials.AddRange(this.objectToLerp.transform.GetChildMaterials());
                }
            }
        }
    }
}