using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractListener : MonoBehaviour
{
    public static List<Interactable> interactables = new();
    [SerializeField] private float listenRange = Mathf.Infinity;
    [SerializeField] private UnityEvent onInteractEnter;
    [SerializeField] private UnityEvent onInteractSwitch;
    [SerializeField] private UnityEvent onInteractExit;
    private Interactable lastInteractable;

    public delegate void InteractEvent(Interactable interactable);
    public static event InteractEvent OnInteractEnter;
    public static event InteractEvent OnInteractSwitch;
    public static event InteractEvent OnInteractExit;

    private void Update()
    {
        Interactable currentInteractable = GetNearestInteractable(listenRange);

        if (currentInteractable == lastInteractable)
            return;

        if (!lastInteractable)
        {
            onInteractEnter.Invoke();
            OnInteractEnter?.Invoke(currentInteractable);
        }
        else if (!currentInteractable)
        {
            onInteractExit.Invoke();
            OnInteractExit?.Invoke(currentInteractable);
        }
        else
        {
            onInteractSwitch.Invoke();
            OnInteractSwitch?.Invoke(currentInteractable);
        }

        lastInteractable = currentInteractable;
    }

    public Interactable GetNearestInteractable(float range = Mathf.Infinity)
    {
        Interactable closestInteractable = null;
        float closestDistance = range;

        foreach (Interactable interactable in interactables)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance >= closestDistance)
                continue;

            closestDistance = distance;
            closestInteractable = interactable;
        }

        return closestInteractable;
    }

}