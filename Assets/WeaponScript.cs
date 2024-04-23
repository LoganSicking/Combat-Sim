using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public Defenderlook look;

    public string weaponName;

    public float turnSpeed;
    public float damage;
    public float roundsPerSecond;
    public float accuracy;

    bool canFire = true;

    public IEnumerator fireShot(Vector3 t)
    {
        if (look.isFiring)
        {
            yield break;
        }
        look.isFiring = true;
        Debug.Log("Shot firing");
        
        yield return look.StartCoroutine(look.turnTowards(turnSpeed, t, canFire, damage, accuracy));
        if(canFire)
        {
            canFire = false;
            StartCoroutine(fireCoolDown());
        }
        look.isFiring = false;
    }

    IEnumerator fireCoolDown()
    {
        yield return new WaitForSeconds(1 / roundsPerSecond);
        canFire = true;
    }
}
