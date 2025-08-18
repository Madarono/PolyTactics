using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AllianceSystem : MonoBehaviour, IDataPersistence
{
    public static AllianceSystem Instance {get; private set;}
    private Alliances alliances;
    private Relationships relationships;
    private Trust trust;
    private ResourcesStorage storage;
    private Factions playerFaction;
    private bool hasRecieved = false;

    public GameObject allianceWindow;
    public GameObject canvas;
    public Animator allianceAnim;
    public float animationDuration = 0.17f;

    [Header("Neutral Alliance Requirements")]
    public int neutralTrust = 60;
    public int neutralPoints = 50;

    [Header("Like Alliance Requirements")]
    public int likeTrust = 80;
    public int likePoints = 80;

    [Header("Colors")]
    public Color[] buttonStates;
    public Color[] textStates;

    [Header("Visuals")]
    public TradeVisual[] visuals;

    [Header("Alliance Window")]
    private int[] allianceState = new int[4];
    public GameObject formationWindow;
    public Animator formationAnim;
    public GameObject formButton;

    public TextMeshProUGUI[] resourcesPlayer;
    public TextMeshProUGUI[] resourcesFaction;
    public Image[] ticks;
    public Sprite[] tickStates = new Sprite[2];

    public int[] resourcesNeutralReq = new int[4];
    public int[] resourcesLikeReq = new int[4];

    private int allianceIndex = -1;
    private int[] req;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.playerFaction = data.playerFaction;
        hasRecieved = true;
    }

    public void SaveData(GameData data)
    {
        if(hasRecieved)
        {
        }
    }

    void Start()
    {
        allianceWindow.SetActive(false);
        formationWindow.SetActive(false);
        canvas.SetActive(false);
        alliances = Alliances.Instance;
        storage = ResourcesStorage.Instance;
        relationships = Relationships.Instance;
        trust = Trust.Instance;
    }


    //Alliance Window
    public void OpenWindow()
    {
        allianceWindow.SetActive(true);
        canvas.SetActive(true);
        UpdateWindow();
        Time.timeScale = 0f;
    }
    public void CloseWindow()
    {
        Time.timeScale = 1f;
        StartCoroutine(closeWindowAnimation(allianceWindow, allianceAnim, true));
    }
    void UpdateWindow()
    {
        List<int> trustValues = new List<int>();
        trustValues.Add(trust.circleTrust);
        trustValues.Add(trust.rectangleTrust);
        trustValues.Add(trust.triangleTrust);
        trustValues.Add(trust.squareTrust);

        List<int> relationValues = new List<int>();

        List<FactionRelation[]> relations = new List<FactionRelation[]>();
        relations.Add(relationships.circleRelation);
        relations.Add(relationships.rectangleRelation);
        relations.Add(relationships.triangleRelation);
        relations.Add(relationships.squareRelation);

        for(int i = 0; i < relations.Count; i++)
        {
            bool foundFaction = false;
            foreach(var relation in relations[i])
            {
                if(relation.faction == playerFaction)
                {
                    relationValues.Add(relation.relationPoints);
                    foundFaction = true;
                    break;
                }
            }
            if(foundFaction)
            {
                continue;
            }
            relationValues.Add(0);
        }
        
        for(int i = 0; i < visuals.Length; i++)
        {
            if(visuals[i].faction != playerFaction)
            {
                visuals[i].visual.SetActive(true);
                visuals[i].value.text = $"{trustValues[i].ToString()} / {relationValues[i].ToString()}";
                if(alliances.factionAlliances[i].isUnderAlliance)
                {
                    visuals[i].buttonImage.color = buttonStates[2];
                    visuals[i].buttonVisual.color = textStates[2];
                    visuals[i].buttonVisual.text = "Formed!";
                    visuals[i].button.interactable = false;
                }
                else if(trustValues[i] >= likeTrust && relationValues[i] >= likePoints)
                {
                    visuals[i].button.gameObject.SetActive(true);
                    visuals[i].buttonImage.color = buttonStates[1];
                    visuals[i].buttonVisual.color = textStates[1];
                    visuals[i].buttonVisual.text = "Form";
                    visuals[i].button.interactable = true;
                    allianceState[i] = 2;
                }
                else if(trustValues[i] >= neutralTrust && relationValues[i] >= neutralPoints)
                {
                    visuals[i].button.gameObject.SetActive(true);
                    visuals[i].buttonImage.color = buttonStates[0];
                    visuals[i].buttonVisual.color = textStates[0];
                    visuals[i].buttonVisual.text = "Form";
                    visuals[i].button.interactable = true;
                    allianceState[i] = 1;
                }
                else
                {
                    visuals[i].button.gameObject.SetActive(false);
                    allianceState[i] = 0;
                }
            }
            else
            {
                visuals[i].visual.SetActive(false);
                allianceState[i] = 0;
            }
        }
    }

    public void OpenAllianceWindow(int index)
    {
        formationWindow.SetActive(true);
        allianceIndex = index;
        UpdateAllianceWindow(allianceIndex);
        Time.timeScale = 0f;
    }
    public void CloseAllianceWindow()
    {
        StartCoroutine(closeWindowAnimation(formationWindow, formationAnim, false));
    }
    void UpdateAllianceWindow(int index)
    {
        if(allianceState[index] == 0)
        {
            return;
        }

        int state = allianceState[index];
        req = new int[0];
        if(state == 1)
        {
            req = resourcesNeutralReq;
        }
        else
        {
            req = resourcesLikeReq;
        }

        List<int> resources = new List<int>();
        resources.Add(storage.grain);
        resources.Add(storage.steel);
        resources.Add(storage.oil);
        resources.Add(storage.uranium);

        bool[] canForm = new bool[4];
        for(int i = 0; i < resources.Count; i++)
        {
            resourcesPlayer[i].text = resources[i].ToString();
            resourcesFaction[i].text = req[i].ToString();
            if(resources[i] >= req[i])
            {
                ticks[i].sprite = tickStates[1];
                canForm[i] = true;
            }
            else
            {
                ticks[i].sprite = tickStates[0];
                canForm[i] = false;
            }
        }

        foreach(var can in canForm)
        {
            formButton.SetActive(true);
            if(!can)
            {
                formButton.SetActive(false);
                break;
            }
        }
    }
    public void FormAlliance()
    {
        Factions faction = Factions.Neutral;
        switch(allianceIndex)
        {
            case 0:
                faction = Factions.Circle;
                break;

            case 1:
                faction = Factions.Rectangle;
                break;

            case 2:
                faction = Factions.Triangle;
                break;

            case 3:
                faction = Factions.Square;
                break;
        }
        storage.grain -= req[0];
        storage.steel -= req[1];
        storage.oil -= req[2];
        storage.uranium -= req[3];
        alliances.MakeManualAlliance(faction);
        CloseAllianceWindow();
        AIFights.Instance.Refresh();
        InteractionSystem.Instance.InstantiateDots();
        UpdateWindow();
    }

    IEnumerator closeWindowAnimation(GameObject window, Animator anim, bool closeCanvas)
    {
        anim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(animationDuration);
        window.SetActive(false);

        if(closeCanvas)
        {
            canvas.SetActive(false);
        }
    }
}
