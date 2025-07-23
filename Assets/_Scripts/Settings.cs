using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour, IDataPersistence
{
    public static Settings Instance { get; private set; }

    [Header("Wave system")]
    public SoundManager soundManager;
    public TowerManager towerManager;
    public EnemyManager enemyManager;
    public Difficulty difficulty;
    public Factions enemyFaction;
    public Factions playerFaction;

    [Header("FPS")]
    public TextMeshProUGUI fpsVisual;
    private float deltaTime = 0.0f;


    [Header("Money")]
    public int money;
    public TextMeshProUGUI moneyVisual;

    public float sellPercentage = 0.7f;

    [Header("Health")]
    public int health;
    public TextMeshProUGUI healthVisual;
    public VisualFaction heartIcon;

    [Header("Wave Visual")]
    public GameObject waveWindow;
    public TextMeshProUGUI waveVisual;
    public float waveAnimation = 0.5f;
    
    [Header("Speed up")]
    public bool isSpeeding;
    public float speedValue = 3f;
    public Image speedBackground;
    public Image[] backgrounds;
    public ColorFactions[] factionColors;
    public Image speedIcon;
    public Sprite[] icons;

    [Header("For debug purposes")]
    public bool saveToData = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.difficulty = data.difficulty;
        this.playerFaction = data.playerFaction;
        this.enemyFaction = data.enemyFaction;
        enemyManager.InitiateStart();
        InitiateStart();
    }

    public void SaveData(GameData data)
    {
        if(saveToData)
        {
            data.difficulty = this.difficulty;
            data.playerFaction = this.playerFaction;
            data.enemyFaction = this.enemyFaction;
        }
    }

    void InitiateStart()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        speedIcon.sprite = icons[0];
        for(int i = 0; i < factionColors.Length; i++)
        {
            if(playerFaction == factionColors[i].faction)
            {
                speedBackground.color = factionColors[i].color;
                foreach(Image img in backgrounds)
                {
                    img.color = factionColors[i].color;
                }
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

    public void ShowWave()
    {
        int waveCount = WaveRandomizer.Instance.waveCount;
        waveVisual.text = "Wave: " + enemyManager.currentWave.ToString() + "/" + waveCount.ToString();
    }

    public void SpeedUp()
    {
        if(enemyManager.enemiesLeft <= 0 && enemyManager.spawnLeft <= 0 && enemyManager.currentWave < enemyManager.waveWeight.Length) //Starting wave
        {
            speedIcon.sprite = icons[1];
            isSpeeding = false;
            enemyManager.StartWave();
            ShowWave();
            soundManager.PlayClip(soundManager.beginWave, 1f);
            return;
        }

        isSpeeding = !isSpeeding;

        soundManager.PlayClip(soundManager.fastForward, 1f);
        Time.timeScale = isSpeeding ? speedValue : 1f;
        speedIcon.sprite = isSpeeding ? icons[2] : icons[1];
    }

    public void SetNormalSpeed()
    {
        isSpeeding = false;
        soundManager.PlayClip(soundManager.endOfRound, 1f);
        Time.timeScale = 1f;
        speedIcon.sprite = icons[0];
        towerManager.EndOfRoundChecks();
    }
}