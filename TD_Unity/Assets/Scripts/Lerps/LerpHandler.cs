using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LerpHandler : MonoBehaviour {

    public static LerpHandler instance;

    void Awake() {
        instance = this;
    }

    public IEnumerator LerpPositionFromObjectTo(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        float positionX = 0f;
        float positionY = 0f;
        float positionZ = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            positionX = curveValue * (wrapper.startPos.x - wrapper.endPos.x);
            positionY = curveValue * (wrapper.startPos.y - wrapper.endPos.y);
            positionZ = curveValue * (wrapper.startPos.z - wrapper.endPos.z);
            wrapper.objectToLerp.transform.position = new Vector3(positionX, positionY, positionZ) + wrapper.startPos;
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator LerpPositionPointToPoint(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            wrapper.objectToLerp.transform.position = Vector3.Lerp(wrapper.startPos, wrapper.endPos, curveValue);
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator LerpUIPositionPointToPoint(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            wrapper.rectTransform.anchoredPosition = Vector3.Lerp(wrapper.UI_StartPosition, wrapper.UI_EndPosition, curveValue);
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator LerpPositionToTargetWithOffset(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        if (wrapper.lerpToOrFrom.Equals(ToOrFrom.TO)) {
            wrapper.endPos = wrapper.target.transform.position + wrapper.targetOffset;
        }
        else {
            wrapper.startPos = wrapper.target.transform.position + wrapper.targetOffset;
        }
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            wrapper.objectToLerp.transform.position = Vector3.Lerp(wrapper.startPos, wrapper.endPos, curveValue);
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator LerpRotationVectorTo(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        Vector3 newCoords = wrapper.startRotation;
        if (wrapper.startRotation.x - wrapper.endRotation.x > 180) {
            newCoords.x -= 360;
        }
        if (wrapper.startRotation.y - wrapper.endRotation.y > 180) {
            newCoords.y -= 360;
        }
        if (wrapper.startRotation.z - wrapper.endRotation.z > 180) {
            newCoords.z -= 360;
        }
        wrapper.startRotation = newCoords;

        float curveValue = 0f;
        float rotationX = 0f;
        float rotationY = 0f;
        float rotationZ = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            rotationX = curveValue * (wrapper.startRotation.x - wrapper.endRotation.x);
            rotationY = curveValue * (wrapper.startRotation.y - wrapper.endRotation.y);
            rotationZ = curveValue * (wrapper.startRotation.z - wrapper.endRotation.z);
            wrapper.objectToLerp.transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ) + wrapper.endRotation;
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    // Lerp Position to a possibly Moving Target Transform + Offset
    public IEnumerator LerpPositionWithForce(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        Vector3 from = wrapper.objectToLerp.transform.position;
        Rigidbody rigidBody = wrapper.objectToLerp.GetComponent<Rigidbody>();
        float timerCur = 0f;
        float velX = 0f;
        float velY = 0f;
        float velZ = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            velX = wrapper.endPos.x != 0 ? wrapper.endPos.x / wrapper.duration : 0;
            velY = wrapper.endPos.y != 0 ? wrapper.endPos.y / wrapper.duration : 0;
            velZ = wrapper.endPos.z != 0 ? wrapper.endPos.z / wrapper.duration : 0;
            if (isBackwards) {
                rigidBody.velocity = new Vector3(-velX, -velY, -velZ);
            }
            else {
                rigidBody.velocity = new Vector3(velX, velY, velZ);
            }
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator LerpPositionWithForceCurve(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        Vector3 from = wrapper.objectToLerp.transform.position;
        Rigidbody rigidBody = wrapper.objectToLerp.GetComponent<Rigidbody>();
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        float velX = 0f;
        float velY = 0f;
        float velZ = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            velX = curveValue * wrapper.endPos.x;
            velY = curveValue * wrapper.endPos.y;
            velZ = curveValue * wrapper.endPos.z;
            rigidBody.velocity = new Vector3(velX, velY, velZ);
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator FadeImage(Lerper lerper, LerpWrapper wrapper, bool isBackwards, bool isMainLerp, Image forceUseImage = null) {
        float timerCur = 0f;
        Image image = wrapper.objectToLerp.GetComponentInChildren<Image>();
        if (forceUseImage != null) {
            image = forceUseImage;
        }
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        float alpha = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            alpha = curveValue * (wrapper.endAlpha - wrapper.startAlpha);
            image.color = new Color(image.color.r, image.color.g, image.color.b, wrapper.startAlpha + alpha);
            yield return new WaitForSeconds(0.01f);
        }
        if (isMainLerp) {
            lerper.FinishLerp(wrapper, isBackwards);
        }
    }

    public IEnumerator FadeImage(Lerper lerper, LerpWrapper wrapper, bool isBackwards, bool isMainLerp, Text forceUseText = null) {
        float timerCur = 0f;
        Text text = wrapper.objectToLerp.GetComponentInChildren<Text>();
        if (forceUseText != null) {
            text = forceUseText;
        }
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        float alpha = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            alpha = curveValue * (wrapper.endAlpha - wrapper.startAlpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, wrapper.startAlpha + alpha);
            yield return new WaitForSeconds(0.01f);
        }
        if (isMainLerp) {
            lerper.FinishLerp(wrapper, isBackwards);
        }
    }

    public IEnumerator FadeMeshMaterial(Lerper lerper, LerpWrapper wrapper, bool isBackwards, bool isMainLerp, Material forceMaterial = null, GameObject forceGameObject = null) {
        float timerCur = 0f;
        Material material = wrapper.objectToLerp.GetComponentInChildren<MeshRenderer>().materials[0];  
        if (forceMaterial != null) {
            material = forceMaterial;
        }
        Debug.Log(forceGameObject + ", " + material);
        Material tempMaterial = MaterialColorHelper.GetTempMaterial(wrapper, material, forceGameObject);
        Color usedColor = new Color();
        float alphaDif = 0f;
        AnimationCurve curve = wrapper.curve;
        usedColor = MaterialColorHelper.GetColor(material);
        alphaDif = wrapper.endAlpha - wrapper.startAlpha;
        float curveValue = 0f;
        float alpha = 0f;
        Color color = new Color();
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            alpha = curveValue * alphaDif;
            color = new Color(usedColor.r, usedColor.g, usedColor.b, wrapper.startAlpha + alpha);
            MaterialColorHelper.SetColor(tempMaterial, color);
            yield return new WaitForSeconds(0.01f);
        }
        if (isMainLerp) {
            lerper.FinishLerp(wrapper, isBackwards);
        }
    }

    public IEnumerator FadeOtherMaterial(Lerper lerper, LerpWrapper wrapper, bool isBackwards, bool isMainLerp, Material forceMaterial = null, GameObject forceGameObject = null) {
        float timerCur = 0f;
        float alphaDif = 0f;
        AnimationCurve curve = wrapper.curve;
        Material mat = wrapper.normalMaterial;
        if (forceMaterial != null) {
            mat = forceMaterial;
        }
        Material tempMaterial = MaterialColorHelper.GetTempMaterial(wrapper, mat, forceGameObject);
        Color startColor = MaterialColorHelper.GetColor(mat);
        alphaDif = wrapper.endAlpha - wrapper.startAlpha;
        float curveValue = 0f;
        float alpha = 0f;
        Color color = new Color();
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            alpha = curveValue * alphaDif;
            color = new Color(startColor.r, startColor.g, startColor.b, wrapper.startAlpha + alpha);
            MaterialColorHelper.SetColor(tempMaterial, color);
            yield return new WaitForSeconds(0.01f);
        }
        if (isMainLerp) {
            lerper.FinishLerp(wrapper, isBackwards);
        }
    }

    public IEnumerator Scale(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        float curveValue = 0f;
        float scaleX = 0f;
        float scaleY = 0f;
        float scaleZ = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            curveValue = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            scaleX = curveValue * (wrapper.endScale.x - wrapper.startScale.x);
            scaleY = curveValue * (wrapper.endScale.y - wrapper.startScale.y);
            scaleZ = curveValue * (wrapper.endScale.z - wrapper.startScale.z);
            wrapper.objectToLerp.transform.localScale = new Vector3(scaleX, scaleY, scaleZ) + wrapper.startScale;
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator HueShift(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        AnimationCurve curve = wrapper.curve;
        HSBColor hsbColor = new HSBColor();
        Color color = new Color();
        Material tempMaterial = MaterialColorHelper.GetTempMaterial(wrapper, wrapper.normalMaterial);
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            hsbColor = HSBColor.FromColor(wrapper.startHueColor);
            hsbColor.h = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            hsbColor.s = 1f;
            hsbColor.b = 1f;
            color = HSBColor.ToColor(hsbColor);
            MaterialColorHelper.SetColor(tempMaterial, color);
            yield return new WaitForSeconds(0.01f);
        }
        MaterialColorHelper.SetColor(tempMaterial, wrapper.startHueColor);
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator HueShiftText(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        Color usedColor = wrapper.text.color;
        AnimationCurve curve = wrapper.curve;
        HSBColor hsbColor = new HSBColor();
        Color color = new Color();
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            hsbColor = HSBColor.FromColor(usedColor);
            hsbColor.h = isBackwards ? curve.Evaluate(1 - (timerCur / wrapper.duration)) : curve.Evaluate(timerCur / wrapper.duration);
            hsbColor.s = 1f;
            hsbColor.b = 1f;
            color = HSBColor.ToColor(hsbColor);
            wrapper.text.color = color;
            yield return new WaitForSeconds(0.01f);
        }
        wrapper.text.color = usedColor;
        lerper.FinishLerp(wrapper, isBackwards);
    }

    public IEnumerator FillCircle(float targetFillAmount, AnimationCurve curve, float duration, Image image, Text text) {
        float currentFillAmount = 0f;
        float timerCur = 0f;
        while (timerCur <= duration) {
            timerCur += Time.deltaTime;
            currentFillAmount = curve.Evaluate(timerCur / duration) * targetFillAmount;
            image.fillAmount = currentFillAmount;
            text.text = (currentFillAmount * 100).ToString("0") + "%";
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator ColorShift(Lerper lerper, LerpWrapper wrapper, bool isBackwards) {
        float timerCur = 0f;
        Material tempMaterial = MaterialColorHelper.GetTempMaterial(wrapper, wrapper.normalMaterial);
        MaterialColorHelper.SetColor(tempMaterial, wrapper.startColor);
        HSBColor hsbStartColor = HSBColor.FromColor(wrapper.startColor);
        HSBColor hsbEndColor = HSBColor.FromColor(wrapper.endColor);
        HSBColor hsbResult = new HSBColor();
        Color color = new Color();
        float time = 0f;
        while (timerCur <= wrapper.duration) {
            timerCur += Time.deltaTime;
            time = isBackwards ? wrapper.curve.Evaluate(1 - (timerCur / wrapper.duration)) : wrapper.curve.Evaluate(timerCur / wrapper.duration);
            hsbResult = HSBColor.Lerp(hsbStartColor, hsbEndColor, time);
            color = HSBColor.ToColor(hsbResult);
            MaterialColorHelper.SetColor(tempMaterial, color);
            yield return new WaitForSeconds(0.01f);
        }
        lerper.FinishLerp(wrapper, isBackwards);
    }
}