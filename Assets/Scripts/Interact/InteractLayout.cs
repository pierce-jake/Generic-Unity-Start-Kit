using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractLayout : MonoBehaviour
{

    public enum Layout
    {
        Stack, Horizontal, Circle
    }

    [SerializeField] private GameObject promptPrefab;
    [SerializeField] private AnimationCurve fadeInCurve, fadeOutCurve;
    private Coroutine transitionRoutine;
    public Layout layout;

    private List<GameObject> prompts = new();

    public void FadeIn()
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(FadeTransition(true));
    }

    public void FadeOut()
    {
        if(transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(FadeTransition(false));
    }

    private IEnumerator FadeTransition(bool fadeIn)
    {
        float t = 0f;

        while(t < 1f)
        {
            transform.localScale = fadeIn ? Vector3.one * fadeInCurve.Evaluate(t) : Vector3.one * fadeOutCurve.Evaluate(t);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
    }

}
