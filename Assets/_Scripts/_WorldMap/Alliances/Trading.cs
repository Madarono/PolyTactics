using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TradeVisual
{
    public Factions faction;
    public GameObject visual;
    public TextMeshProUGUI value;
    public Button button;
    public Image buttonImage;
    public TextMeshProUGUI buttonVisual;
}

[System.Serializable]
public class TradeRequirement
{
    public int[] requirement = new int[4];
    public int minReq;
    public int maxReq;
}

[System.Serializable]
public class InviteSlot
{
    public GameObject visual;
    public TextMeshProUGUI value;
}

public class Trading : MonoBehaviour, IDataPersistence
{
    public static Trading Instance {get; private set;}
    private Trust trust;
    private Relationships relationships;
    private ResourcesStorage storage;
    private SoundManager sound;
    
    public GameObject tradingWindow;
    public Animator windowAnim;
    public float animationDuration;

    public GameObject inviteWindow;
    public Animator inviteAnim;

    [Header("Visual - Choose Trade")]
    public Factions playerFaction;
    public TradeVisual[] visuals;
    public Color[] buttonStates;
    public Color[] textStates;
    public int inviteIndex = -1;
    
    [Header("Visual - Invite")]
    public TradeRequirement[] inviteReq;
    public TextMeshProUGUI[] visualsInvite;
    public Slider[] slidersInvite;
    
    [Header("Slots Visual")]
    [Tooltip("This is the storage that the player is willing to give")]
    public InviteSlot[] inviteSlots;
    public InviteSlot[] otherSlots;

    public TextMeshProUGUI probability;
    public int chance;
    public float cooldownSound = 0.25f;

    [Header("Trade Accepted")]
    public GameObject checkTradeButton;
    public GameObject checkWindow;
    public Animator checkAnim;

    public string[] acceptanceWords;
    public string[] declinedWords;
    public string replaceWord;
    public int relationshipIncrease = 20;
    public TextMeshProUGUI infoVisual;
    public TextMeshProUGUI headerVisual;
    public TextMeshProUGUI relationshipVisual;
    public Factions otherFaction;
    
    private int[] inviteValues = new int[4];
    private int[] inviteMax = new int[4];
    private int[] currentReq = new int[4];
    private int finalPoints = 0;
    private int reqIndex;
    private string factionName;
    private bool sentInvite = false;
    private bool canSound = true;
    private bool hasRecieved = false;
    private bool recievedLetter = false;
    private bool hasAccepted = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.playerFaction = data.playerFaction;
        this.reqIndex = data.reqIndex;
        this.chance = data.inviteChance;
        Refresh();
        hasRecieved = true;
    }

    public void SaveData(GameData data)
    {    
        if(hasRecieved && (sentInvite || recievedLetter))
        {
            data.reqIndex = this.reqIndex;
            data.inviteChance = this.chance;
        }
    }

    void Refresh()
    {
        checkTradeButton.SetActive(false);
        if(reqIndex == -1)
        {
            return;
        }

        foreach(var faction in visuals)
        {
            faction.button.interactable = false;
            faction.button.gameObject.SetActive(false);
        }

        visuals[reqIndex].buttonImage.color = buttonStates[1];
        visuals[reqIndex].buttonVisual.color = textStates[1];
        visuals[reqIndex].buttonVisual.text = "Sent!";
        visuals[reqIndex].button.gameObject.SetActive(true);
    }

    void Start()
    {
        tradingWindow.SetActive(false);
        inviteWindow.SetActive(false);
        checkWindow.SetActive(false);
        trust = Trust.Instance;
        relationships = Relationships.Instance;
        storage = ResourcesStorage.Instance;
        sound = SoundManager.Instance;
        foreach(var req in inviteReq)
        {
            req.requirement[0] = Random.Range(req.minReq, req.maxReq);
            req.requirement[1] = Random.Range(req.minReq, Mathf.FloorToInt(req.maxReq * 0.8f));
            req.requirement[2] = Random.Range(req.minReq, Mathf.FloorToInt(req.maxReq * 0.5f));
            req.requirement[3] = Random.Range(req.minReq, Mathf.FloorToInt(req.maxReq * 0.4f));
        }
    }

    public void CheckTrade()
    {
        if(reqIndex == -1)
        {
            return;
        }

        float chanceToGet = Random.Range(0, 100);
        switch(reqIndex)
        {
            case 0:
                factionName = "Circle";
                otherFaction = Factions.Circle;
                break;
            case 1:
                factionName = "Rectangle";
                otherFaction = Factions.Rectangle;
                break;
            case 2:
                factionName = "Triangle";
                otherFaction = Factions.Triangle;
                break;
            case 3:
                factionName = "Square";
                otherFaction = Factions.Square;
                break;
        }
        
        if(chanceToGet <= chance)
        {
            Notification.Instance.PutNotification($"{factionName} has accepted your trade request.");
            hasAccepted = true;
        }
        else
        {
            Notification.Instance.PutNotification($"{factionName} has declined your trade request.");
            hasAccepted = true;
        }

        finalPoints = Mathf.FloorToInt(relationshipIncrease * (chance / 100f));
        float multipler = hasAccepted ? 1f: 0.5f;
        finalPoints = Mathf.FloorToInt(finalPoints * multipler);

        switch(otherFaction)
        {
            case Factions.Circle:
                trust.circleTrust += finalPoints;
                break;

            case Factions.Rectangle:
                trust.rectangleTrust += finalPoints;
                break;

            case Factions.Triangle:
                trust.triangleTrust += finalPoints;
                break;

            case Factions.Square:
                trust.squareTrust += finalPoints;
                break;
        }

        foreach(var faction in visuals)
        {
            faction.button.gameObject.SetActive(false); 
        }

        checkTradeButton.SetActive(true);

        reqIndex = -1;
        chance = 0;
        recievedLetter = true;
        DataPersistenceManager.instance.SaveGame();
        recievedLetter = false;
    }

    //Trade Window 
    public void OpenWindow()
    {
        tradingWindow.SetActive(true);
        UpdateWindow();
        Time.timeScale = 0f;
    }
    public void CloseWindow()
    {
        Time.timeScale = 1f;
        StartCoroutine(closeWindowAnimation(tradingWindow, windowAnim));
    }
    void UpdateWindow()
    {
        List<int> values = new List<int>();
        values.Add(trust.circleTrust);
        values.Add(trust.rectangleTrust);
        values.Add(trust.triangleTrust);
        values.Add(trust.squareTrust);
        
        for(int i = 0; i < visuals.Length; i++)
        {
            if(visuals[i].faction != playerFaction)
            {
                visuals[i].visual.SetActive(true);
                visuals[i].value.text = values[i].ToString();
            }
            else
            {
                visuals[i].visual.SetActive(false);
            }
        }
    }
    public void SendTradeInvite()
    {
        if(chance == 0)
        {
            return;
        }

        foreach(var faction in visuals)
        {
            faction.button.interactable = false;
            faction.button.gameObject.SetActive(false);
        }

        visuals[reqIndex].buttonImage.color = buttonStates[1];
        visuals[reqIndex].buttonVisual.color = textStates[1];
        visuals[reqIndex].buttonVisual.text = "Sent!";
        visuals[reqIndex].button.gameObject.SetActive(true);

        storage.grain -= inviteValues[0];
        storage.steel -= inviteValues[1];
        storage.oil -= inviteValues[2];
        storage.uranium -= inviteValues[3];
        CloseInviteWindow();
        sentInvite = true;
        DataPersistenceManager.instance.SaveGame();
    }

    //Invite Trade Window
    public void OpenInviteWindow(int index)
    {
        inviteWindow.SetActive(true);
        UpdateInviteWindow(index);
        reqIndex = index;
    }
    public void CloseInviteWindow()
    {
        StartCoroutine(closeWindowAnimation(inviteWindow, inviteAnim));
    }
    void UpdateInviteWindow(int index)
    {
        List<int> values = new List<int>();
        values.Add(storage.grain);
        values.Add(storage.steel);
        values.Add(storage.oil);
        values.Add(storage.uranium);
        inviteMax = values.ToArray();

        currentReq = inviteReq[index].requirement;
        for(int i = 0; i < slidersInvite.Length; i++)
        {
            visualsInvite[i].text = values[i].ToString();
            slidersInvite[i].maxValue = values[i];
            slidersInvite[i].value = 0;
            inviteSlots[i].visual.SetActive(false);
            otherSlots[i].value.text = currentReq[i].ToString();
        }
        probability.text = "Probability: 0%";
    }
    public void UpdateInviteValues(int index)
    {
        float finalChance = 0;
        if(canSound)
        {
            float amplify = slidersInvite[index].value / slidersInvite[index].maxValue * 2;
            sound.PlayClip(sound.changingSliders, 0.15f * amplify);
            StartCoroutine(Cooldown());
        }
        for(int i = 0; i < slidersInvite.Length; i++)
        {
            inviteValues[i] = (int)slidersInvite[i].value;
            int difference = inviteMax[i] - inviteValues[i];
            visualsInvite[i].text = difference.ToString();
            if(inviteValues[i] > 0)
            {
                inviteSlots[i].visual.SetActive(true);
                inviteSlots[i].value.text = inviteValues[i].ToString();
            }
            else
            {
                inviteSlots[i].visual.SetActive(false);
            }
            
            finalChance += Mathf.Clamp01((float)inviteValues[i] / (float)currentReq[i]);
        }

        chance = Mathf.RoundToInt(finalChance * 100 / 4);
        probability.text = $"Probability: {chance.ToString()}%";
    }
    public void MaxAll()
    {
        float finalChance = 0;
        for(int i = 0; i < slidersInvite.Length; i++)
        {
            inviteValues[i] = Mathf.Min(inviteMax[i], currentReq[i]);
            slidersInvite[i].value = inviteValues[i];
            int difference = inviteMax[i] - inviteValues[i];
            visualsInvite[i].text = difference.ToString();
            if(inviteValues[i] > 0)
            {
                inviteSlots[i].visual.SetActive(true);
                inviteSlots[i].value.text = inviteValues[i].ToString();
            }
            else
            {
                inviteSlots[i].visual.SetActive(false);
            }
            
            finalChance += Mathf.Clamp01((float)inviteValues[i] / (float)currentReq[i]);
        }
        
        chance = Mathf.RoundToInt(finalChance * 100 / 4);
        probability.text = $"Probability: {chance.ToString()}%";
    }

    //Accepted Trade Window
    public void OpenAcceptWindow()
    {
        checkWindow.SetActive(true);
        UpdateAcceptWindow();
    }
    public void CloseAcceptWindow()
    {
        checkTradeButton.SetActive(false);

        finalPoints = 0;
        chance = 0;
        foreach(var faction in visuals)
        {
            faction.button.interactable = true;
            faction.button.gameObject.SetActive(true); 
            faction.buttonImage.color = buttonStates[0];
            faction.buttonVisual.color = textStates[0];
            faction.buttonVisual.text = "Send Invite";
        }
        UpdateWindow();
        StartCoroutine(closeWindowAnimation(checkWindow, checkAnim));
    }
    void UpdateAcceptWindow()
    {
        string acceptWord = hasAccepted ? acceptanceWords[Random.Range(0, acceptanceWords.Length)] : declinedWords[Random.Range(0, declinedWords.Length)]; 
        string finalWord = acceptWord.Replace(replaceWord, factionName);
        finalWord = finalWord.Replace("\\n", "\n");

        infoVisual.text = finalWord;
        headerVisual.text = $"- {factionName}";

        int previousPoints = 0;
        int nowPoints = 0;

        switch(otherFaction)
        {
            case Factions.Circle:
                previousPoints = trust.circleTrust - finalPoints;
                nowPoints = trust.circleTrust;
                break;

            case Factions.Rectangle:
                previousPoints = trust.rectangleTrust - finalPoints;
                nowPoints = trust.rectangleTrust;
                break;

            case Factions.Triangle:
                previousPoints = trust.triangleTrust - finalPoints;
                nowPoints = trust.triangleTrust;
                break;

            case Factions.Square:
                previousPoints = trust.squareTrust - finalPoints;
                nowPoints = trust.squareTrust;
                break;
        }

        relationshipVisual.text = $"{previousPoints} > {nowPoints}";
    }

    IEnumerator closeWindowAnimation(GameObject window, Animator anim)
    {
        anim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(animationDuration);
        window.SetActive(false);
    }

    IEnumerator Cooldown()
    {
        canSound = false;
        yield return new WaitForSecondsRealtime(cooldownSound);
        canSound = true;
    }
}