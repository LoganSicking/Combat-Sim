using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupervisorAI : MonoBehaviour
{
    public Vector3 lastPosition;
    public int numberOfAttackers;

    public WeaponScript defenderWeapon;
    public EnemyScript[] attackers;

    public Transform[] cover;
    public Ladder[] ladders;
    public Transform[] highCovers;
    public Transform[] riskyCovers;
    public float climbTime;

    public DataTracker ds;

    bool hasPlan = false;
    private void Update()
    {
        foreach(EnemyScript e in attackers)
        {
            if(e.state != "AWAIT")
            {
                return;
            }
        }

        if(!hasPlan)
        {
            hasPlan = true;
            Debug.Log("AWAITING PLAN!");
            evalStrat();
        }
    }

    public void evalStrat()
    {
        if (defenderWeapon.weaponName == "SNIPER")
        {
            int choice = Random.Range(0, 6);
            switch (choice)
            {
                case 0:
                    rush();
                    break;
                case 1:
                    coverAdvance();
                    break;
                case 2:
                    spreadOut();
                    break;
                case 3:
                    highGround();
                    break;
                case 4:
                    spreadOutHigh();
                    break;
                case 5:
                    riskyRush();
                    break;
            }

        }
        else if (defenderWeapon.weaponName == "SHOTGUN")
        {
            int choice = Random.Range(0, 6);
            switch (choice)
            {
                case 0:
                    flank();
                    break;
                case 1:
                    divide();
                    break;
                case 2:
                    decoy();
                    break;
                case 3:
                    highGround();
                    break;
                case 4:
                    spreadOutHigh();
                    break;
                case 5:
                    riskyRush();
                    break;
            }
        }
        else if (defenderWeapon.weaponName == "MACHINEGUN")
        {
            int choice = Random.Range(0, 6);
            switch (choice)
            {
                case 0:
                    flank();
                    break;
                case 1:
                    divide();
                    break;
                case 2:
                    decoy();
                    break;
                case 3:
                    rush();
                    break;
                case 4:
                    coverAdvance();
                    break;
                case 5:
                    spreadOut();
                    break;
            }
        }
        else
        {
            choose:
            int choice = Random.Range(0, 9);
            if(choice != 8)
            {
                goto choose;
            }
            switch (choice)
            {
                case 0:
                    flank();
                    break;
                case 1:
                    divide();
                    break;
                case 2:
                    decoy();
                    break;
                case 3:
                    rush();
                    break;
                case 4:
                    coverAdvance();
                    break;
                case 5:
                    spreadOut();
                    break;
                case 6:
                    highGround();
                    break;
                case 7:
                    spreadOutHigh();
                    break;
                case 8:
                    riskyRush();
                    break;
            }
        }
    }
    void riskyRush()
    {
        ds.stratType = "D";
        List<int> coverIndex = Enumerable.Range(0, riskyCovers.Length).ToList();
        int high1 = Random.Range(0, attackers.Length);
        int high2;
        do
        {
            high2 = Random.Range(0, attackers.Length);
        } while (high2 == high1);

        for (int i = 0; i < attackers.Length; i++)
        {
            EnemyScript e = attackers[i];
            if (i == high1)
            {
                e.moveToCover(ladders[0].transform.position);
                StartCoroutine(awaitClimb(e, ladders[0], highCovers[0]));
            }
            else if (i == high2)
            {
                e.moveToCover(ladders[1].transform.position);
                StartCoroutine(awaitClimb(e, ladders[1], highCovers[1]));
            }
            else
            {
                int chosenPoint = coverIndex[Random.Range(0, riskyCovers.Length)];
                e.moveToCover(riskyCovers[chosenPoint].position);
                coverIndex.Remove(chosenPoint);
                StartCoroutine(awaitPeek(e));
            }
        }
    }
    void spreadOutHigh()
    {
        ds.stratType = "D";
        List<int> coverIndex = Enumerable.Range(0, cover.Length).ToList();
        int high1 = Random.Range(0, attackers.Length);
        int high2;
        do
        {
            high2 = Random.Range(0, attackers.Length);
        } while (high2 == high1);

        for (int i = 0; i < attackers.Length; i++)
        {
            EnemyScript e = attackers[i];
            if (i == high1)
            {
                e.moveToCover(ladders[0].transform.position);
                StartCoroutine(awaitClimb(e, ladders[0], highCovers[0]));
            }
            else if (i == high2)
            {
                e.moveToCover(ladders[1].transform.position);
                StartCoroutine(awaitClimb(e, ladders[1], highCovers[1]));
            }
            else
            {
                int chosenPoint = coverIndex[Random.Range(0, coverIndex.Count)];
                e.moveToCover(cover[chosenPoint].position);
                coverIndex.Remove(chosenPoint);
                StartCoroutine(awaitPeek(e));
            }
        }
    }

    void highGround()
    {
        ds.stratType = "D";
        int high1 = Random.Range(0, attackers.Length);
        int high2;
        do
        {
            high2 = Random.Range(0, attackers.Length);
        } while (high2 == high1);

        for (int i = 0; i < attackers.Length; i++)
        {
            EnemyScript e = attackers[i];
            if (i == high1)
            {
                e.moveToCover(ladders[0].transform.position);
                StartCoroutine(awaitClimb(e, ladders[0], highCovers[0]));
            }
            else if(i == high2)
            {
                e.moveToCover(ladders[1].transform.position);
                StartCoroutine(awaitClimb(e, ladders[1], highCovers[1]));
            }
            else
            {
                e.peek();
                StartCoroutine(awaitPeek(e));
            }
        }
    }

    void decoy()
    {
        ds.stratType = "S";
        List<Transform> flanks = getFlanks();
        int rush1 = Random.Range(0, attackers.Length);

        for (int i = 0; i < attackers.Length; i++)
        {
            EnemyScript e = attackers[i];
            if (i == rush1)
            {

                e.moveToCover(flanks[0].position);
            }
            else
            {
                e.moveToCover(flanks[1].position);
            }
        }

        StartCoroutine(waitFlank(new int[] { rush1}));
    }

    void divide()
    {
        ds.stratType = "S";
        List<Transform> flanks = getFlanks();
        int rush1 = Random.Range(0, attackers.Length);
        int rush2;
        do
        {
            rush2 = Random.Range(0, attackers.Length);
        } while (rush2 == rush1);

        for (int i = 0; i < attackers.Length; i++)
        {
            EnemyScript e = attackers[i];
            if (i == rush1 || i == rush2)
            {

                e.moveToCover(flanks[0].position);
            }
            else
            {
                e.moveToCover(flanks[1].position);
            }
        }

        StartCoroutine(waitFlank(new int[] { 0,1,2,3 }));
    }

    void flank()
    {
        ds.stratType = "S";
        List<Transform> flanks = getFlanks();
        int rush1 = Random.Range(0, attackers.Length);
        int rush2;
        do
        {
            rush2 = Random.Range(0, attackers.Length);
        } while (rush2 == rush1);

        for (int i = 0; i < attackers.Length; i++)
        {
            EnemyScript e = attackers[i];
            if (i == rush1 || i == rush2)
            {
                
                e.moveToCover(flanks[0].position);
            }
            else
            {
                e.moveToCover(flanks[1].position);
            }
        }

        StartCoroutine(waitFlank(new int[] { rush1, rush2 }));
    }
    void rush()
    {
        ds.stratType = "A";
        foreach (EnemyScript e in attackers)
        {
            e.rush();
            Debug.Log("RUSH");
        }
    }

    void coverAdvance()
    {
        ds.stratType = "A";
        int rush1 = Random.Range(0, attackers.Length);
        int rush2 = 0;
        do
        {
            rush2 = Random.Range(0, attackers.Length);
        } while (rush2 == rush1);

        for(int i=0;i<attackers.Length;i++)
        {
            if(i == rush1 || i == rush2)
            {
                attackers[i].rush();
            }
            else
            {
                attackers[i].peek();
                StartCoroutine(awaitPeek(attackers[i]));
            }
        }
    }

    void spreadOut()
    {
        ds.stratType = "A";
        List<int> coverIndex = Enumerable.Range(0, cover.Length).ToList();
        foreach(EnemyScript e in attackers)
        {
            int chosenPoint = coverIndex[Random.Range(0, coverIndex.Count)];
            e.moveToCover(cover[chosenPoint].position);
            coverIndex.Remove(chosenPoint);
        }
        StartCoroutine(awaitRush());
    }

    IEnumerator awaitClimb(EnemyScript e, Ladder l,Transform t)
    {
        while (true)
        {
            yield return null;
            if(e.gameObject == null)
            {
                yield break;
            }
            if (e.state == "CLIMB")
            {
                yield return new WaitForSeconds(climbTime);
                e.transform.position += l.offset;
                e.isHighGround = true;
                break;
            }
        }
        yield return new WaitForSeconds(climbTime);
        e.moveToCover(t.position);
        StartCoroutine(awaitPeek(e));
    }

    IEnumerator awaitRush()
    {
        bool done;
        do
        {
            done = true;
            yield return null;
            foreach (EnemyScript e in attackers)
            {
                if(e.gameObject == null)
                {
                    continue;
                }
                if (e.state != "AWAIT")
                {
                    done = false;
                }
            }
        } while (!done);

        rush();
    }

    IEnumerator awaitPeek(EnemyScript e)
    {
        while(true)
        {
            yield return null;
            if (e.gameObject == null)
            {
                yield break;
            }
            if (e.state == "AWAIT")
            {
                e.peek();
            }
        }
    }

    IEnumerator waitFlank(int[] rushers)
    {
        yield return new WaitForSeconds(1f);
        bool done;
        do
        {
            done = true;
            yield return null;
            foreach (EnemyScript e in attackers)
            {
                Debug.Log(e.state);
                if (e.state != "AWAIT")
                {
                    done = false;
                }
            }
        } while (!done);

        Debug.Log("FLANKING");
        for (int i = 0; i < attackers.Length; i++)
        {
            if (rushers.Contains(i))
            {
                attackers[i].rush();
            }
            else
            {
                attackers[i].peek();
                StartCoroutine(awaitPeek(attackers[i]));
            }
        }
    }

    List<Transform> getFlanks()
    {
        Transform[] orderedCover = cover.OrderBy(x => Vector3.SignedAngle(Vector3.forward,(x.position-lastPosition).normalized,Vector3.up)).ToArray();
        return new List<Transform> { orderedCover.First(), orderedCover.Last() };
    }
}
