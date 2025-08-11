using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class AIFights : MonoBehaviour, IDataPersistence
{
    public static AIFights Instance {get; private set;}
    private Notification notification;
    private Alliances alliances;
    private InteractionSystem interaction;
    
    [Header("Visual")]
    public Color[] factionColors;
    public Color idleColor;
    public GameObject[] buttons;
    public Color[] dotsSelection = new Color[2];
    [Tooltip("Color of dots after confirming selection")]public Color[] dotsFaction;

    private List<int> strength = new List<int>();
    private List<int> factionIndex = new List<int>();
    private List<Color> color = new List<Color>();
    public List<Vector3Int> placeToFight = new List<Vector3Int>();
    [HideInInspector]public List<GameObject> dots = new List<GameObject>();
    
    [HideInInspector]public bool canFight = false;
    private int amountSelected = -1;
    private Vector3Int pendingPlace;
    private SpriteRenderer pastDot;

    [Header("Visual")]
    public Image background;
    public TextMeshProUGUI visual;

    [Header("For saving")]
    public int[] placesInt;
    public int[] lastFight;
    public int[] strength_fights;
    public int[] relationIndex;
    private Factions playerFaction;
    private bool hasRecieved = false;
    private bool hasFought = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.placesInt = data.placesInt;
        this.strength_fights = data.strength_fights;
        this.playerFaction = data.playerFaction;
        this.lastFight = data.lastFight;
        hasRecieved = true;
    } 

    public void SaveData(GameData data)
    {
        if(hasRecieved && hasFought)
        {
            PutToSave();
            data.placesInt = this.placesInt;
            data.strength_fights = this.strength_fights;
            data.lastFight = this.lastFight;
        }
    }

    void PutToSave()
    {
        List<int> places = new List<int>();
        foreach(var pos in placeToFight)
        {
            places.Add(pos.x);
            places.Add(pos.y);
            places.Add(pos.z);
        }

        placesInt = places.ToArray();

        List<int> strengthInt = new List<int>();

        for(int i = 0; i < strength.Count; i++)
        {
            if(i < places.Count)
            {
                strengthInt.Add(strength[i]);
            }
        }
        
        strength_fights = strengthInt.ToArray();
    }

    public void PutIntoGlow()
    {
        if(lastFight.Length == 0)
        {
            return;
        }

        List<Vector3Int> pos = new List<Vector3Int>();

        for(int i = 0; i < lastFight.Length; i += 3)
        {
            pos.Add(new Vector3Int(lastFight[i], lastFight[i + 1], lastFight[i + 2]));
        } 

        foreach(var position in pos)
        {
            FactionConquer.Instance.glowPlayerPositions.Add(position);
        }
    }

    public void CheckAllys()
    {
        List<Vector3Int> pos = new List<Vector3Int>();

        for(int i = 0; i < placesInt.Length; i += 3)
        {
            pos.Add(new Vector3Int(placesInt[i], placesInt[i + 1], placesInt[i + 2]));
        }

        if(pos.Count == 0)
        {
            Debug.Log("Nothing innit");
            hasFought = true;
            return;
        }

        List<int> wonPlaces = new List<int>();
        List<Vector3Int> wonPlacesVector = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();
        Tilemap[] tilemaps = new Tilemap[4] {interaction.blue, interaction.red, interaction.yellow, interaction.green};

        for(int i = 0; i < strength_fights.Length; i++)
        {
            TileBase tile = GetTile(pos[i], tilemaps);
            int enemyTileIndex = -1;
            
            if(tile == interaction.blueTile)
            {
                enemyTileIndex = 0;
            }
            else if(tile == interaction.redTile)
            {
                enemyTileIndex = 1;
            }
            else if(tile == interaction.yellowTile)
            {
                enemyTileIndex = 2;
            }
            else if(tile == interaction.greenTile)
            {
                enemyTileIndex = 3;
            }
            else
            {
                enemyTileIndex = 4;
            }

            float attackerStrength = strength_fights[i];
            float enemyStrength = 0;
            if(enemyTileIndex != 4)
            {
                enemyStrength = FactionPower.Instance.factionStrength[enemyTileIndex].strength;
                Debug.Log(enemyStrength.ToString());
            }
            else
            {
                enemyStrength = attackerStrength / 4f;
                Debug.Log($"Enemy Strength Ground: {enemyStrength.ToString()}");
            }
            
            float winChance = (attackerStrength * 100f) / (attackerStrength + enemyStrength + 1f);
            float fightChance = Random.Range(0, 100f);
            if(fightChance <= winChance)
            {
                int[] places = new int[3] {pos[i].x, pos[i].y, pos[i].z};
                LandConquerer.Instance.AddPlaces(places, playerFaction);
                FactionConquer.Instance.glowPlayerPositions.Add(pos[i]);
                wonPlaces.Add(pos[i].x);
                wonPlaces.Add(pos[i].y);
                wonPlaces.Add(pos[i].z);
                wonPlacesVector.Add(pos[i]);
                tiles.Add(tile);
            }
        }
        lastFight = wonPlaces.ToArray();
        
        Relationships relationships = Relationships.Instance; 
        int pointDeduction = FactionConquer.Instance.pointDeduction;
        int playerGain = FactionConquer.Instance.playerGain;
        for(int l = 0; l < tiles.Count; l++)
        {
            TileBase tile = tiles[l];
            FactionRelation[] relations = new FactionRelation[0];
            switch(playerFaction)
            {
                case Factions.Circle:
                    relations = relationships.circleRelation;
                    break;
                
                case Factions.Rectangle:
                    relations = relationships.rectangleRelation;
                    break;
                
                case Factions.Triangle:
                    relations = relationships.triangleRelation;
                    break;

                case Factions.Square:
                    relations = relationships.squareRelation;
                    break;
            }

            for(int i = 0; i < relations.Length; i++)
            {
                if(tile == interaction.blueTile && relations[i].faction == Factions.Circle)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints - pointDeduction, 0);
                    for(int o = 0; o < relationships.circleRelation.Length; o++)
                    {
                        if(playerFaction == relationships.circleRelation[o].faction)
                        {
                            relationships.circleRelation[o].relationPoints = Mathf.Max(relationships.circleRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.circleRelation);
                            break;
                        }
                    }
                    break;   
                }
                else if(tile == interaction.redTile && relations[i].faction == Factions.Rectangle)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints - pointDeduction, 0);
                    for(int o = 0; o < relationships.rectangleRelation.Length; o++)
                    {
                        if(playerFaction == relationships.rectangleRelation[o].faction)
                        {
                            relationships.rectangleRelation[o].relationPoints = Mathf.Max(relationships.rectangleRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.rectangleRelation);
                            break;
                        }
                    }
                    break;   
                }
                else if(tile == interaction.yellowTile && relations[i].faction == Factions.Triangle)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints - pointDeduction, 0);
                    for(int o = 0; o < relationships.triangleRelation.Length; o++)
                    {
                        if(playerFaction == relationships.triangleRelation[o].faction)
                        {
                            relationships.triangleRelation[o].relationPoints = Mathf.Max(relationships.triangleRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.triangleRelation);
                            break;
                        }
                    }
                    break;   
                }
                else if(tile == interaction.greenTile && relations[i].faction == Factions.Square)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints - pointDeduction, 0);
                    for(int o = 0; o < relationships.squareRelation.Length; o++)
                    {
                        if(playerFaction == relationships.squareRelation[o].faction)
                        {
                            relationships.squareRelation[o].relationPoints = Mathf.Max(relationships.squareRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.squareRelation);
                            break;
                        }
                    }
                    break;   
                }
            }
            relationships.CheckRelation(relations);
        }
        
        hasFought = true;
    }
    
    void Start()
    {
        amountSelected = -1;
        notification = Notification.Instance;
        alliances = Alliances.Instance;
        interaction = InteractionSystem.Instance;
    }

    void Update()
    {
        if(canFight)
        {
            if(Application.isMobilePlatform && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return;
                }

                if(touch.phase == TouchPhase.Began)
                {
                    Vector3 worldPos = Camera.main.ScreenToWorldPoint(touch.position);
                    Vector3Int intPos = interaction.ground.WorldToCell(worldPos);
                    worldPos.z = 0f;
                    if(!interaction.isHidden && isSteppingOnLand(intPos))
                    {
                        if(!placeToFight.Contains(intPos))
                        {
                            // placeToFight.Add(intPos);
                            // StartCoroutine(RemoveCanFight());
                            pendingPlace = intPos;
                        }
                        SoundManager.Instance.PlayClip(SoundManager.Instance.changePosition, 1f);
                    }
                }
            }
            else if(Input.GetMouseButtonDown(0))
            {
                if(EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int intPos = interaction.ground.WorldToCell(worldPos);
                worldPos.z = 0f;
                if(!interaction.isHidden && isSteppingOnLand(intPos))
                {
                    if(!placeToFight.Contains(intPos))
                    {
                        // placeToFight.Add(intPos);
                        // StartCoroutine(RemoveCanFight());
                        pendingPlace = intPos;
                    }
                    SoundManager.Instance.PlayClip(SoundManager.Instance.changePosition, 1f);
                }
            }
        }
    }

    public void Refresh()
    {
        FactionPower power = FactionPower.Instance;
        strength.Clear();
        color.Clear();
        factionIndex.Clear();

        for(int i = 0; i < alliances.factionAlliances.Length; i++)
        {
            if(!alliances.factionAlliances[i].isUnderAlliance)
            {
                continue;
            }

            for(int o = 0; o < power.factionStrength.Length; o++)
            {
                if(alliances.factionAlliances[i].faction == power.factionStrength[o].faction)
                {
                    strength.Add(power.factionStrength[o].strength);
                    switch(alliances.factionAlliances[i].faction)
                    {
                        case Factions.Circle:
                            color.Add(factionColors[0]);
                            factionIndex.Add(0);
                            break;

                        case Factions.Rectangle:
                            color.Add(factionColors[1]);
                            factionIndex.Add(1);
                            break;

                        case Factions.Triangle:
                            color.Add(factionColors[2]);
                            factionIndex.Add(2);
                            break;

                        case Factions.Square:
                            color.Add(factionColors[3]);
                            factionIndex.Add(3);
                            break;
                    }
                    break;
                }
            }
        }

        if(color.Count > 0 && amountSelected < color.Count - 1)
        {
            background.color = color[amountSelected + 1];
        }
        else
        {
            background.color = idleColor;
        }

        int leftToFight = strength.Count - placeToFight.Count;
        visual.text = leftToFight.ToString();
    }

    public void Fight()
    {
        int leftToFight = strength.Count - placeToFight.Count;
        if(leftToFight == 0 || amountSelected == strength.Count - 1)
        {
            if(strength.Count > 0)
            {
                notification.PutNotification("No more allys to help.");
            }
            else
            {
                notification.PutNotification("No alliance currently active.");
            }
            return;
        }

        interaction.isOpen = false;
        interaction.CloseInventoryWindow();
        if(interaction.lastDot != null && interaction.lastDot.TryGetComponent(out SpriteRenderer rend2))
        {
            rend2.color = interaction.dotInactive;
            interaction.lastDot = null;
        }
        foreach(var obj in buttons)
        {
            obj.SetActive(true);
        }

        canFight = true;
        notification.PutNotification("Select the land for the ally to fight.");
    }

    public void Accept()
    {
        placeToFight.Add(pendingPlace);
        // Set dot color;
        pastDot.color = dotsFaction[factionIndex[placeToFight.Count - 1]];
        relationIndex = factionIndex.ToArray();
        dots.Add(pastDot.gameObject);
        List<int> places = new List<int>();
        foreach(var pos in placeToFight)
        {
            places.Add(pos.x);
            places.Add(pos.y);
            places.Add(pos.z);
        }

        placesInt = places.ToArray();
        StartCoroutine(RemoveCanFight());
    }

    public void Decline()
    {
        canFight = false;
        if(pastDot != null)
        {
            pastDot.color = dotsSelection[0];
            pastDot = null;
        }

        foreach(var obj in buttons)
        {
            obj.SetActive(false);
        }
        Refresh();
    }

    bool isSteppingOnLand(Vector3Int pos)
    {
        Vector3 tilePos = interaction.ground.GetCellCenterWorld(pos);

        for(int i = 0; i < interaction.dots.Count; i++)
        {
            if(tilePos == interaction.dots[i].transform.position && !dots.Contains(interaction.dots[i]))
            {
                if(pastDot != null)
                {
                    pastDot.color = dotsSelection[0];
                }

                if(interaction.dots[i].TryGetComponent(out SpriteRenderer rend))
                {
                    rend.color = dotsSelection[1];
                    pastDot = rend;
                }
                return true;
            }
        }
        return false;
    }

    TileBase GetTile(Vector3Int pos, Tilemap[] tilemaps)
    {
        foreach(var tilemap in tilemaps)
        {
            TileBase tile = tilemap.GetTile(pos);
            if(tile != null)
            {
                return tile;
            }
        }
        return null;
    }

    IEnumerator RemoveCanFight()
    {
        yield return null;
        canFight = false;
        amountSelected++;
        foreach(var obj in buttons)
        {
            obj.SetActive(false);
        }
        pastDot = null;
        Refresh();
    }
}
