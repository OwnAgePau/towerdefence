using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum LerpSmoothness{
    Smooth,
    Linear,
    SmoothDown,
    LinearSmooth
}

public class LerpHandler : MonoBehaviour{
    public static LerpHandler instance;

    public List<AnimationCurve> curves = new List<AnimationCurve>();

    /*public AnimationCurve SmoothCurve;
    public AnimationCurve LinearCurve;
    public AnimationCurve SmoothDownCurve;
    public AnimationCurve LinearSmoothCurve;*/

    void Awake(){
        instance = this;
    }

    public IEnumerator LerpPositionTo(float duration, Transform movable, Vector3 from, Vector3 to, AnimationCurve curve){
        float timerCur = 0f;
        while (timerCur <= duration){
            timerCur += Time.deltaTime;
            movable.position = Vector3.Lerp(from, to, curve.Evaluate(timerCur / duration));
            yield return new WaitForSeconds(0.01f);
        }
    }

    // Lerp Position to a possibly Moving Target Transform + Offset
    public IEnumerator LerpPositionTo(float duration, Transform movable, Transform target, Vector3 targetOffset, AnimationCurve curve){
        Vector3 from = movable.position;
        float timerCur = 0f;
        while (timerCur <= duration){
            timerCur += Time.deltaTime;
            movable.position = Vector3.Lerp(from, target.position + targetOffset, curve.Evaluate(timerCur / duration));
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator LerpRotationTo(float duration, Transform rotatable, Quaternion from, Quaternion to, AnimationCurve curve){
        float timerCur = 0f;
        while (timerCur <= duration){
            timerCur += Time.deltaTime;
            rotatable.rotation = Quaternion.Lerp(from, to, curve.Evaluate(timerCur / duration));
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator Scale(float duration, GameObject item, bool isScalingUp, AnimationCurve curve, bool isMenuOpen){
        float timerCur = 0f;
        Quaternion start = new Quaternion(0f, 0f, 0f, 0);
        Quaternion to = new Quaternion(1f, 1f, 1f, 0);
        while (timerCur <= duration){
            timerCur += Time.deltaTime;
            float newScale = curve.Evaluate(timerCur / duration);
            if (isScalingUp){
                item.transform.localScale = new Vector3(newScale, newScale, 1f);
            }
            else{
                item.transform.localScale = new Vector3(1f - newScale, 1f - newScale, 1f - newScale);
            }
            yield return new WaitForSeconds(0.01f);
        }
        TowerInfo.instance.isAnimatingMenu = false;
        TowerInfo.instance.isMenuOpen = isMenuOpen;
    }

    /* AnimationCurve GetCurveByEnum(LerpSmoothness smoothness){
        switch (smoothness){
            case LerpSmoothness.Linear:
                return LinearCurve;
            case LerpSmoothness.Smooth:
                return SmoothCurve;
            case LerpSmoothness.SmoothDown:
                return SmoothDownCurve;
            case LerpSmoothness.LinearSmooth:
                return LinearSmoothCurve;

            default:
                return SmoothCurve;
        }
    }*/
}