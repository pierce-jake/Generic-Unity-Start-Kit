using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{

    public float viewRange;
    [Range(0, 360)]
    public float viewAngle;


    public LayerMask targetMask;
    public LayerMask obstacleMask;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    private void Start()
    {
        StartCoroutine(FindTargetsInView(0.25f));
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool globalAngle = true)
    {
        angleInDegrees += globalAngle ? 0f : transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInRange = Physics.OverlapSphere(transform.position, viewRange);

        foreach(Collider target in targetsInRange)
        {
            Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) > viewAngle / 2)
                continue;

            float targetDist = Vector3.Distance(transform.position, target.transform.position);
            if (Physics.Raycast(transform.position, dirToTarget, targetDist, obstacleMask))
                continue;

            visibleTargets.Add(target.transform);
        }

    }

    private IEnumerator FindTargetsInView(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

}
