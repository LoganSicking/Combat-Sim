using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    private float health;
    public Image healthBar;
    public SupervisorAI sai;
    public TMP_Text victoryText;
    public DataTracker ds;
    float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            healthBar.fillAmount = health / maxHealth;
            if(health <= 0)
            {
                if(sai != null)
                {
                    sai.numberOfAttackers--;
                    if(sai.numberOfAttackers == 0)
                    {
                        victoryText.text = "DEFENDER WINS!";
                        victoryText.gameObject.SetActive(true);
                        ds.win = "D";
                        ds.dataLog();
                        if (ds.currentRun != 1)
                        {
                            PlayerPrefs.SetInt("CurrentRun", ds.currentRun - 1);
                            SceneManager.LoadScene(0);
                        }
                        else
                        {
                            PlayerPrefs.SetInt("MultipleRuns", 0);
                        }
                    }
                }
                else
                {
                    victoryText.text = "ATTACKERS WIN!";
                    victoryText.gameObject.SetActive(true);
                    ds.win = "A";
                    ds.dataLog();
                    if (ds.currentRun != 1)
                    {
                        PlayerPrefs.SetInt("CurrentRun", ds.currentRun - 1);
                        SceneManager.LoadScene(0);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("MultipleRuns", 0);
                    }
                }
                
                
                gameObject.SetActive(false);
            }
        }
    }

    public float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        Health = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        Health -= amount;
        Debug.Log("Taken " + amount);
    }
}
