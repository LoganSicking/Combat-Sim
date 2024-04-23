using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataTracker : MonoBehaviour
{
    public int numberOfRuns = 1;
    public bool _override;

    public int currentRun;

    public string weapon;
    public string win;
    public string stratType;
    public float duration;
    public int defenderShots = 0;
    public int attackerShots = 0;
    float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        if(PlayerPrefs.GetInt("MultipleRuns") != 1 || _override)
        {
            currentRun = numberOfRuns;
            PlayerPrefs.SetInt("MultipleRuns", 1);
            _override = false;
        }
        else
        {
            currentRun = PlayerPrefs.GetInt("CurrentRun");
        }

        Debug.Log("Run number " + currentRun);

    }

    public void dataLog()
    {
        duration = Time.time - startTime;
        Debug.Log("MATCH REPORT: \n"+
            "WEAPON: "+weapon+"\n"+
            "WIN: " + win + "\n" +
            "STRATEGY: " + stratType + "\n" +
            "DURATION: " + duration + "\n" +
            "ATTACKER SHOTS: " + attackerShots + "\n" +
            "DEFENDER SHOTS: " + defenderShots + "\n");
        File.AppendAllText("data.csv", string.Format("{0},{1},{2},{3},{4},{5}\n",weapon,win,stratType,duration,defenderShots,attackerShots));
    }
}
