using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICore : MonoBehaviour
{

    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private float viewDistance = 10f;

    void Start()
    {
        
    }
    void Update()
    {

        if (!Input.GetMouseButton(0))
            return;


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        //Gizmos.DrawLine(transform.position, )
    }

}
