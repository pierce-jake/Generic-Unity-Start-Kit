using UnityEngine.Events;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float detectionDistance = 2.0f;
    public InteractOption[] options;

    void OnEnable()
    {
        InteractListener.interactables.Add(this);
    }

    void OnDisable()
    {
        InteractListener.interactables.Remove(this);
    }
}

[System.Serializable]
public class InteractOption
{
    public string label = "Take";
    public KeyCode requiredKey;
    public float holdTime = 1.0f;
    public float decayTime = 1.0f;
    public bool autoDecay = true;
    public UnityEvent onInteract;
}
