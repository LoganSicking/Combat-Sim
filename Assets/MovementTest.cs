using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System;
using static UnityEngine.GraphicsBuffer;
using System.Linq;

public class MovementTest : MonoBehaviour
{
    public Transform[] wayPoints;
    public Transform[] cover;

    public float patrolWait;

    Path path;
    int currentWaypoint;
    int currentPatrolWaypoint = 0;
    public float movementThreshold = 0.01f;
    public float speed = 1f;
    CharacterController controller;
    Seeker s;
    public WeaponScript ws;
    public DataTracker ds;

    public string state = "PATROL";
    // Start is called before the first frame update
    void Start()
    {
        s = GetComponent<Seeker>();
        ds.weapon = ws.weaponName;

        controller = GetComponent<CharacterController>();

        patrol();
    }

    void patrol()
    {
        s.StartPath(transform.position, wayPoints[currentPatrolWaypoint].position, initPath);
    }

    private void initPath(Path p)
    {
        path = p;
        currentWaypoint = 0;
    }



    // Update is called once per frame
    void Update()
    {
        if(path == null)
        {
            return;
        }

        if(currentWaypoint == path.vectorPath.Count && state == "COVER")
        {
            StartCoroutine(patrolCooldown());
            state = "SHOOTONSIGHT";
        }
        Vector3 dir = path.vectorPath[currentWaypoint] - transform.position;

        controller.SimpleMove(dir.normalized * speed);

        if(dir.magnitude < movementThreshold)
        {
            currentWaypoint++;
        }

        if((wayPoints[currentPatrolWaypoint].position-transform.position).magnitude < movementThreshold && state == "PATROL")
        {
            currentPatrolWaypoint = (currentPatrolWaypoint + 1) % wayPoints.Length;
            patrol();
        }
     }

    public void EnemyFound(Vector3 t)
    {
        ws.StartCoroutine(ws.fireShot(t));
        if(state == "PATROL")
        {
            Vector3 target = cover.OrderBy(t => (t.position - transform.position).magnitude).First().position;
            s.StartPath(transform.position, target, initPath);
            state = "COVER";
        }
    }

    IEnumerator patrolCooldown()
    {
        yield return new WaitForSeconds(patrolWait);
        state = "PATROL";
        patrol();

    }
}
