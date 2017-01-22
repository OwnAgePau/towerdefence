using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class UtilFunctions {

    public delegate void LoadComplete();
    public static event LoadComplete OnLoadComplete;

    public static int GetRandomFromOptions(int[] options){
        return options[Random.Range(0, options.Length)];
    }

    public static Vector3 GetRandomFromOptions(Vector3[] options){
        return options[Random.Range(0, options.Length)];
    }

    public static void Shuffle<T>(this IList<T> list){
        int n = list.Count;
        while (n > 1){
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static float frac(float n){
        return n - Mathf.Floor(n);
    }

    public static IEnumerator<float> LoadLevelAsync(string sceneName, float minWaitTime, Image progressImage){

        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        ao.allowSceneActivation = false; // Put this to false if you don't want it to load
        float curWaitTime = 0f;
        System.GC.Collect();
        while (ao.progress < 0.9f || curWaitTime < minWaitTime){
            curWaitTime += 0.01f;
            progressImage.rectTransform.offsetMax = new Vector3(ao.progress * 1501f - 1501f, progressImage.rectTransform.offsetMax.y);
            yield return MovementEffects.Timing.WaitForSeconds(0.01f);
        }
        progressImage.rectTransform.offsetMax = new Vector3(0, progressImage.rectTransform.offsetMax.y);
        ao.allowSceneActivation = true;
        if(OnLoadComplete != null) {
            OnLoadComplete.Invoke();
        }       
    }
}