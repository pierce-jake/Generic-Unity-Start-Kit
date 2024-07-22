using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PromptPool))]
public class InteractLayout : MonoBehaviour
{

    public enum Layout
    {
        Stack, Horizontal, Circle
    }

    [SerializeField] private GameObject promptPrefab;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), fadeOutCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    [SerializeField] private float transitionSpeed = 1f;
    private Coroutine transitionRoutine;
    private Coroutine moveRoutine;
    public Layout layout;

    private PromptPool pool;
    private readonly List<Prompt> prompts = new();

    private void Awake()
    {
        pool = GetComponent<PromptPool>();
        pool.prefab = promptPrefab.GetComponent<Prompt>();
    }

    private void OnEnable()
    {
        InteractListener.OnInteractEnter += Enter;
        InteractListener.OnInteractSwitch += Switch;
        InteractListener.OnInteractExit += Exit;
    }

    private void OnDisable()
    {
        InteractListener.OnInteractEnter -= Enter;
        InteractListener.OnInteractSwitch -= Switch;
        InteractListener.OnInteractExit -= Exit;
    }

    // THIS IS TEMPORARY!!!
    private void Update()
    {
        foreach(Prompt prompt in prompts)
        {
            if(Input.GetKey(prompt.OptionData.requiredKey))
            {
                // Key pressed
                prompt.Fill += Time.deltaTime * (1f / prompt.OptionData.holdTime);
            } else if(prompt.OptionData.autoDecay)
                prompt.Fill -= Time.deltaTime * (1f / prompt.OptionData.decayTime);

            if(prompt.Fill >= 1f)
            {
                //Invoke Filled
            }
        }
    }

    public void Enter(Interactable interactable)
    {

        transform.position = GameManager.Instance.MainCamera.WorldToScreenPoint(interactable.transform.position);
        TrimPrompts(interactable.options);
        FadeIn();

        if(moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToPosition(interactable.transform.position, transitionSpeed));
    }

    public void Switch(Interactable interactable)
    {
        if(moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveToPosition(interactable.transform.position, transitionSpeed));
        TrimPrompts(interactable.options);
    }

    public void Exit(Interactable interactable)
    {
        FadeOut();
    }

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

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        transitionRoutine = StartCoroutine(FadeTransition(false));
    }

    private void EnablePrompts(bool enable)
    {
        for (int i = 0; i < prompts.Count; i++)
        {
            prompts[i].gameObject.SetActive(enable);
        }
    }

    private void TrimPrompts(params InteractOption[] options)
    {
        pool.Give(prompts.ToArray());
        prompts.Clear();
        prompts.AddRange(pool.GetAmount(options.Length));

        for(int i = 0; i < prompts.Count; i++)
        {
            prompts[i].OptionData = options[i];
        }
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

        if(!fadeIn)
            EnablePrompts(false);
    }

    private IEnumerator MoveToPosition(Vector3 position, float time)
    {
        float stepTime = 1f / time;

        while(true)
        {
            Vector3 newPos = GameManager.Instance.MainCamera.WorldToScreenPoint(position);
            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * stepTime);
            yield return new WaitForEndOfFrame();
        }
    }

}
