using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VisionScript : MonoBehaviour
{
    public float distance;
    public float angle;

    public float coneDensity;
    public Color coneColor;

    public GameObject parent;

    public Transform targetTransform;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public UnityEvent onSpotted;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, distance, targetMask);

        foreach(Collider c in inRange)
        {
            Vector3 target = c.transform.position;
            Vector3 dir = (target - transform.position).normalized;
            if(Vector3.Angle(transform.forward,dir) <= angle)
            {
                float distFromTarget = Vector3.Distance(transform.position, target);
                if (!Physics.Raycast(transform.position,dir,distFromTarget,obstacleMask))
                {
                    Debug.Log("Target Seen!");
                    targetTransform = c.transform;
                    onSpotted.Invoke();
                }
            }
        }
    }

    public void sendToParent(string msg)
    {
        parent.SendMessage(msg);
    }

    public void sendToParentInfo(string msg)
    {
        parent.SendMessage(msg,targetTransform.position);
        targetTransform = null;
    }
}
