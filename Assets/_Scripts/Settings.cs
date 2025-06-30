using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [Header("Wave system")]
    public EnemyManager enemyManager;
    public Difficulty difficulty;
    public Factions enemyFaction;
    public Factions playerFaction;


    [Header("Money")]
    public int money;
    public TextMeshProUGUI moneyVisual;

    public float sellPercentage = 0.7f;

    [Header("Health")]
    public int health;
    public TextMeshProUGUI healthVisual;
    public VisualFaction heartIcon;
    
    [Header("Speed up")]
    public bool isSpeeding;
    public float speedValue = 3f;
    public Image speedBackground;
    public ColorFactions[] factionColors;
    public Image speedIcon;
    public Sprite[] icons;

    void Start()
    {
        speedIcon.sprite = icons[0];
        for(int i = 0; i < factionColors.Length; i++)
        {
            if(playerFaction == factionColors[i].faction)
            {
                speedBackground.color = factionColors[i].color;
                heartIcon.ChangeSprite(i);
                break;
            }
        }
        UpdateVisual();
        healthVisual.text = health.ToString();
    }

    public void UpdateVisual()
    {
        moneyVisual.text = "$" + money.ToString();
    }

    public void CheckHealth()
    {
        health = Mathf.Max(0, health);
        healthVisual.text = health.ToString();
        if(health <= 0)
        {

        }
    }

    public void SpeedUp()
    {
        if(enemyManager.enemyParent.childCount == 0 && enemyManager.currentWave < enemyManager.waveWeight.Length)
        {
            speedIcon.sprite = icons[1];
            isSpeeding = false;
            enemyManager.StartWave();
            return;
        }

        isSpeeding = !isSpeeding;

        Time.timeScale = isSpeeding ? speedValue : 1f;
        speedIcon.sprite = isSpeeding ? icons[2] : icons[1];
    }

    public void SetNormalSpeed()
    {
        isSpeeding = false;
        Time.timeScale = 1f;
        speedIcon.sprite = icons[0];
    }
}