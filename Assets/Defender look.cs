using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defenderlook : MonoBehaviour
{
    public float amplitude = 10f;
    public float speed = 1f;
    public float offset = 0f;
    public bool isAttacker;
    public DataTracker ds;

    VisionScript vs;
    

    public bool isFiring = false;
    // Update is called once per frame

    private void Start()
    {
        vs = GetComponent<VisionScript>();
    }
    void FixedUpdate()
    {
        if(isFiring)
        {
            return;
        }
        
        transform.rotation = Quaternion.Euler(0, offset + Mathf.Sin(Time.time * speed) * amplitude, 0);
    }

    public IEnumerator turnTowards(float speed, Vector3 target,bool isFire = false, float dmg = 0, float accuracy = 0)
    {
        RaycastHit hit;
        while (!Physics.Raycast(transform.position,transform.forward,out hit,vs.distance,vs.targetMask))
        {
            Debug.Log("Raycast Missed");
            Vector3 dir = (target - transform.position).normalized;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, dir, speed, 0);
            transform.rotation = Quaternion.LookRotation(newDir);
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("LOOKING AT OPPONENT");

        if(isFire)
        {
            if (Random.Range(0f, 1f) < accuracy)
            {
                hit.collider.gameObject.SendMessage("TakeDamage", dmg);
            }
            if(isAttacker)
            {
                ds.attackerShots++;
            }
            else
            {
                ds.defenderShots++;
            }
        }
    }
}
