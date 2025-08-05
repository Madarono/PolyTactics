using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WaveResources : MonoBehaviour, IDataPersistence
{
    public static WaveResources Instance {get; private set;}

    [Header("Resources")]
    public int coins;
    public int grain;
    public int steel;
    public int oil;
    public int uranium;

    public bool hasWon;
    public bool finishedBattle;
    bool hasSaved = false;
    bool showUI = false;

    [Header("Visuals")]
    public GameObject window;
    public TextMeshProUGUI header;
    public TextMeshProUGUI coinsVisual;
    public TextMeshProUGUI grainVisual;
    public TextMeshProUGUI steelVisual;
    public TextMeshProUGUI oilVisual;
    public TextMeshProUGUI uraniumVisual;

    [Header("Transitions")]
    public GameObject enterTransition;
    public float enterDuration = 0.2f;
    public GameObject leaveTransition;
    public float leaveDuration = 1.5f;

    [Header("Debug")]
    public bool saveToData = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        window.SetActive(false);
        StartCoroutine(EnterTransition());
    }

    public void LoadData(GameData data)
    {
        this.coins = data.coins;
        this.grain = data.grain;
        this.steel = data.steel;
        this.oil = data.oil;
        this.uranium = data.uranium;
    }

    public void SaveData(GameData data)
    {
        if(finishedBattle && !hasSaved && showUI)
        {
            data._coins += this.coins;
            data._grain += this.grain;
            data._steel += this.steel;
            data._oil += this.oil;
            data._uranium += this.uranium;
            data.hasWon = this.hasWon;
            data.makeWarsHappen = true; 
            hasSaved = true; //To only save once after winning
        }
        else if(finishedBattle && !hasSaved && !showUI)
        {
            data.hasWon = false;
            data.makeWarsHappen = true; 
            hasSaved = true;
        }

        if(saveToData)
        {
            data.coins = this.coins;
            data.grain = this.grain;
            data.steel = this.steel;
            data.oil = this.oil;
            data.uranium = this.uranium;
        }
    }

    public void FinishedBattle(bool won, bool showUI)
    {
        Time.timeScale = 0f;
        hasWon = won;
        finishedBattle = true;
        this.showUI = showUI;
        if(!this.showUI)
        {
            ReturnToWorldMap();    
            return;
        }

        window.SetActive(true);
        header.text = hasWon ? "Battle Won!" : "Battle Lost..";
        if(hasWon)
        {
            StartCoroutine(IncreaseByTime(coins, coinsVisual));
            StartCoroutine(IncreaseByTime(grain, grainVisual));
            StartCoroutine(IncreaseByTime(steel, steelVisual));
            StartCoroutine(IncreaseByTime(oil, oilVisual));
            StartCoroutine(IncreaseByTime(uranium, uraniumVisual));
            return;
        }

        StartCoroutine(IncreaseByTime(Mathf.FloorToInt((float)coins / 5), coinsVisual));
        StartCoroutine(IncreaseByTime(Mathf.FloorToInt((float)grain / 5), grainVisual));
        StartCoroutine(IncreaseByTime(Mathf.FloorToInt((float)steel / 5), steelVisual));
        StartCoroutine(IncreaseByTime(Mathf.FloorToInt((float)oil / 5), oilVisual));
        StartCoroutine(IncreaseByTime(Mathf.FloorToInt((float)uranium / 5), uraniumVisual));
    
    }

    private IEnumerator IncreaseByTime(int amount, TextMeshProUGUI visual)
    {
        int maxAmount = amount;
        float currentAmount = 0;
        if(amount >= 1)
        {
            currentAmount = 1;
        }
        float multipler = 0;

        while(currentAmount < maxAmount)
        {
            multipler += Time.unscaledDeltaTime * 10;

            currentAmount += Time.unscaledDeltaTime * multipler;
            int intAmount = Mathf.FloorToInt(currentAmount);
            intAmount = Mathf.Clamp(intAmount, 0, maxAmount);
            visual.text = intAmount.ToString();
            yield return null;
        }

        visual.text = maxAmount.ToString();
        yield break;
    }

    public void ReturnToWorldMap()
    {
        StartCoroutine(LeaveToWorldMap());
    }

    IEnumerator LeaveToWorldMap()
    {
        leaveTransition.SetActive(true);
        yield return new WaitForSecondsRealtime(leaveDuration);
        DataPersistenceManager.instance.SaveGame();
        Time.timeScale = 1f;
        SceneManager.LoadScene("WorldMap");
    }

    IEnumerator EnterTransition()
    {
        enterTransition.SetActive(true);
        yield return new WaitForSecondsRealtime(enterDuration);
        enterTransition.SetActive(false);
    }
}
