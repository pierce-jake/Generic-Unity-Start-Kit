using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class TargetTracker : MonoBehaviour
{

    public Transform target;
    public Vector3 lastSpottedLocation;

    public delegate void TargetTrackEvent();
    public event TargetTrackEvent OnTargetSpotted;
    public event TargetTrackEvent OnTargetBlocked;
    public event TargetTrackEvent OnLastSightBlocked;
    public event TargetTrackEvent OnTargetLost;

    private FieldOfView fov;

    private void Start()
    {
        fov = GetComponent<FieldOfView>();
    }

    public void DeterminTargetVisibility()
    {

        if (target && !fov.visibleTargets.Contains(target))
        {
            lastSpottedLocation = target.position;
            target = null;
            if (fov.visibleTargets.Count > 0)
                target = fov.visibleTargets[Random.Range(0, fov.visibleTargets.Count)];

            return;
        }

        if (target)
        {
            //Shoot The Fucker!

        }
        else if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, Mathf.Infinity))
        {
            //Sight Blocked
        }

        else if (!Physics.Raycast(transform.position, lastSpottedLocation - transform.position, Vector3.Distance(lastSpottedLocation, transform.position), fov.obstacleMask))
        {
            //Cautious Pathing (Can't see the target at last known location)
        } else
        {
            //Wander Around Area
        }

    }

    public void SetWanderPoint()
    {

    }

    public void NotifyListeners()
    {

    }

}
