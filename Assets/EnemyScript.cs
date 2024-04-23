using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyScript : MonoBehaviour
{
    Seeker s;
    public Transform advanceTarget;
    CharacterController controller;
    Path path;

    public SupervisorAI sai;

    public GameObject[] allies;
    Transform[] cover;

    public float speed = 1f;
    public float movementThreshold = 0.1f;

    bool isClimbing = false;
    public bool isHighGround = false;

    int currentWaypoint = 0;
    public string state = "ADVANCE";

    public float peakbackTime = 3f;

    public float awaitTimeout = 10f;

    public WeaponScript ws;

    Vector3 homePos;
    // Start is called before the first frame update
    void Start()
    {
        cover = sai.cover;
        s = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        advance();
    }

    void advance()
    {
        s.StartPath(transform.position, advanceTarget.position, initPath);
    }

    public void peek()
    {
        homePos = transform.position;
        s.StartPath(transform.position, sai.lastPosition, initPath);
        state = "PEAKING";
    }

    public void rush()
    {
        s.StartPath(transform.position, sai.lastPosition, initPath);
        state = "RUSH";
    }

    public void moveToCover(Vector3 t)
    {
        path = null;
        s.StartPath(transform.position, t, initPath);
        state = "COVER";
    }

    private void initPath(Path p)
    {
        path = p;
        currentWaypoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint == path.vectorPath.Count && state == "COVER" && !isClimbing)
        {
            state = "AWAIT";
            StartCoroutine(awaitCooldown());
        }

        if (currentWaypoint == path.vectorPath.Count && state == "COVER" && isClimbing)
        {
            Debug.Log("Awaiting climb");
            state = "CLIMB";
        }

        if (currentWaypoint == path.vectorPath.Count && state == "PEAKINGBACK")
        {
            state = "AWAIT";
        }

        Vector3 dir = path.vectorPath[currentWaypoint] - transform.position;

        controller.SimpleMove(dir.normalized * speed);

        if (dir.magnitude < movementThreshold)
        {
            currentWaypoint++;
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        Ladder l = collision.gameObject.GetComponent<Ladder>();
        if(l != null)
        {
            isClimbing = true;
            Debug.Log("Climbing now");
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        Ladder l = collision.gameObject.GetComponent<Ladder>();
        if (l != null)
        {
            isClimbing = false;
        }
    }

    public void DefenderFound(Vector3 t)
    {
        sai.lastPosition = t;
        Debug.Log("Defender Spotted!");
        ws.StartCoroutine(ws.fireShot(t));
        foreach(GameObject g in allies)
        {
            if(g == null)
            {
                continue;
            }
            if(!GameObject.ReferenceEquals(g,gameObject))
            {
                if(g.GetComponent<EnemyScript>().state != "PEAKING" && g.GetComponent<EnemyScript>().state != "PEAKINGBACK" && g.GetComponent<EnemyScript>().state != "RUSH" && g.GetComponent<EnemyScript>().state != "AWAIT")
                {
                    g.SendMessage("EnemyFoundInner", t);
                }
                /**Defenderlook dl = g.GetComponentInChildren<Defenderlook>();
                if(dl.isFiring)
                {
                    continue;
                }
                dl.isFiring = true;
                dl.StartCoroutine(dl.turnTowards(g.GetComponent<WeaponScript>().turnSpeed,t));**/
            }
            else
            {
                g.SendMessage("EnemyFoundInner", t);
            }
        }

    }

    public void EnemyFoundInner(Vector3 defender)
    {
        if(state == "PEAKING")
        {
            s.StartPath(transform.position, homePos, initPath);
            state = "PEAKINGBACK";
            StartCoroutine(peakbackCooldown());
            return;
        }
        if (state != "COVER" && state != "PEAKINGBACK" && state != "RUSH" && state != "AWAIT")
        {
            
            Vector3 target = cover.OrderBy(t => (t.position - transform.position).magnitude).First().position;
            s.StartPath(transform.position, target, initPath);
            state = "COVER";
        }
    }

    IEnumerator peakbackCooldown()
    {
        yield return new WaitForSeconds(peakbackTime);
        Vector3 target = cover.OrderBy(t => (t.position - transform.position).magnitude).First().position;
        s.StartPath(transform.position, target, initPath);
        state = "COVER";

    }

    IEnumerator awaitCooldown()
    {
        yield return new WaitForSeconds(awaitTimeout);
        if(state == "AWAIT")
        {
            sai.evalStrat();
        }
    }
}
