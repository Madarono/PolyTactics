using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class FactionTileBase
{
    public TileBase tile;
    public string tileName;
    public Color windowColor;
    public float multiplyer = 1;
}

public class InteractionSystem : MonoBehaviour, IDataPersistence
{
    public static InteractionSystem Instance {get; private set;}
    public Factions playerFaction;
    public Factions enemyFaction;
    public Camera cam;


    [Header("Tilemaps")]
    public Tilemap ground;
    public Tilemap blue;
    public Tilemap red;
    public Tilemap yellow;
    public Tilemap green;
    private Tilemap tilemap;

    [Header("Visual")]
    public GameObject dotPrefab;
    public Transform dotParent;
    public float dotScaleOverride = 0.65f;
    public List<GameObject> dots = new List<GameObject>();
    public bool isHidden = false;
    public Color dotActive;
    public Color dotInactive;

    [Header("Values")]
    public int waves;
    public float waveWeight;
    public int coins;
    public int grains;
    public int steel;
    public int oil;
    public int uranium;
    public float multiplyer = 1f;
    public float multiplyerincreament = 0.1f;

    public TextMeshProUGUI coinsVisual;
    public TextMeshProUGUI grainVisual;
    public TextMeshProUGUI steelVisual;
    public TextMeshProUGUI oilVisual;
    public TextMeshProUGUI uraniumVisual;
    public TextMeshProUGUI waveVisual;

    [Header("Choosing Inventory")]
    public List<TowerSlotSO> currentTowers = new List<TowerSlotSO>();
    public List<GameObject> currentTowersObj = new List<GameObject>();
    public List<int> currentTowersIndex = new List<int>();
    
    public GameObject addButton;
    public GameObject inventoryWindow;
    public GameObject slotPrefab;
    public Transform slotParent;
    public GameObject displayPrefab;
    public Transform displayParent;
    private bool inventoryOn;

    [Header("Window")]
    public FactionTileBase[] bases;
    public GameObject levelWindow;
    public Transform[] windowStates = new Transform[2];
    public bool isOpen;
    public float windowSpeed = 5f;
    public Image whiteSpace;
    public TextMeshProUGUI headerVisual;

    // public float zoom = 1f;
    // public float zoomSpeed = 20;
    // public float moveSpeed = 10;
    
    [Header("Play")]
    public GameObject playWindow;
    public Animator playWindowAnim;
    public GameObject leaveTransition;
    public float leaveDuration;
    public float playWindowCloseDuration = 1f;
    public bool saveLevel;
    public int[] levelPlace = new int[3];
    public int levelFaction;
    public bool hasWon;
    private bool makeWarsHappen = false;

    private GameObject lastDot;
    private int factionIndex; //For the tilebase
    bool checkedPlayer = false;

    Vector3Int intPos;

    void Awake()
    {
        Instance = this;
        leaveTransition.SetActive(false);
        playWindow.SetActive(false);
    }

    public void LoadData(GameData data)
    {
        this.multiplyer = data.resourceMultiplyer;
        this.playerFaction = data.playerFaction;
        this.hasWon = data.hasWon;
        this.levelPlace = data.levelPlace;
        this.makeWarsHappen = data.makeWarsHappen;
        this.checkedPlayer = data.checkedPlayer;
        
        if(hasWon)
        {
            this.multiplyer += multiplyerincreament;
        }

        PerlinNoise.Instance.InstantiateStart(data.seed, data.width, data.height); //Call for noice here
        FactionConquer.Instance.playerFaction = this.playerFaction;        
        StartCoroutine(CallLater());
    }

    public void SaveData(GameData data)
    {
        data.makeWarsHappen = this.makeWarsHappen;
        data.checkedPlayer = this.checkedPlayer;
        if(saveLevel)
        {
            data.a_waves = Mathf.RoundToInt(this.waves * bases[factionIndex].multiplyer * this.multiplyer);
            data.a_waveWeight = Mathf.RoundToInt(this.waveWeight * bases[factionIndex].multiplyer * this.multiplyer);
            data.coins = Mathf.RoundToInt(this.coins * bases[factionIndex].multiplyer * this.multiplyer);
            data.grain = Mathf.RoundToInt(this.grains * bases[factionIndex].multiplyer * this.multiplyer);
            data.steel = Mathf.RoundToInt(this.steel * bases[factionIndex].multiplyer * this.multiplyer);
            data.oil = Mathf.RoundToInt(this.oil * bases[factionIndex].multiplyer * this.multiplyer);
            data.uranium = Mathf.RoundToInt(this.uranium * bases[factionIndex].multiplyer * this.multiplyer);
            data.resourceMultiplyer = this.multiplyer;
            data.levelPlace = this.levelPlace;
            data.enemyFaction = this.enemyFaction;
            data.hasWon = false; //Change this if there are bugs later on
            data.checkedPlayer = false;
            data.slotIndex = this.currentTowersIndex.ToArray();
        }
    }

    IEnumerator CallLater()
    {
        yield return null;
        Inventory.Instance.LoadTowers();
        Inventory.Instance.LoadStockTowers();
        CheckFaction.Instance.CheckVisuals(playerFaction);
        FactionPower.Instance.CalculateStrength();
        if(hasWon) //This is for land
        {
            Vector3Int placeVector = new Vector3Int(levelPlace[0], levelPlace[1], levelPlace[2]);
            FactionConquer.Instance.glowPlayerPositions.Add(placeVector);
            LandConquerer.Instance.AddPlaces(levelPlace, playerFaction);
            if(!checkedPlayer)
            {
                CheckFactionsPlayer(placeVector);
                checkedPlayer = true;
            }
        }
        if(makeWarsHappen)
        {
            FactionConquer.Instance.ConquerForAll();
            Trading.Instance.CheckTrade();
            makeWarsHappen = false;
        }
        LandConquerer.Instance.ApplyPlaces();
        FactionConquer.Instance.ShowGlow();
        
        InstantiateDots();
        saveLevel = false;
        DataPersistenceManager.instance.SaveGame();
    }

    public void CheckFactionsPlayer(Vector3Int place)
    {
        //Circle attacked Square, Rectangle hated Square so Rectangle with Circle has better relation
        Relationships relationships = Relationships.Instance; 
        Factions otherFaction = Factions.Neutral;
        FactionRelation[] relation = new FactionRelation[0];
        FactionRelation[] relationOther = new FactionRelation[0];
        FactionRelation[] relationOther2 = new FactionRelation[0];
        FactionRelation[] relationOther3 = new FactionRelation[0];

        FactionRelation[] playerRelation = new FactionRelation[0];

        switch(playerFaction)
        {
            case Factions.Circle:
                playerRelation = relationships.circleRelation;
                break;

            case Factions.Rectangle:
                playerRelation = relationships.rectangleRelation;
                break;

            case Factions.Triangle:
                playerRelation = relationships.triangleRelation;
                break;

            case Factions.Square:
                playerRelation = relationships.squareRelation;
                break;
        }

        if(blue.GetTile(place) != null && playerFaction != Factions.Circle)
        {
            otherFaction = Factions.Circle;
            relation = relationships.circleRelation;
            relationOther = relationships.squareRelation;
            relationOther2 = relationships.rectangleRelation;
            relationOther3 = relationships.triangleRelation;
        }
        else if(red.GetTile(place) != null && playerFaction != Factions.Rectangle)
        {
            otherFaction = Factions.Rectangle;
            relation = relationships.rectangleRelation;
            relationOther = relationships.squareRelation;
            relationOther2 = relationships.circleRelation;
            relationOther3 = relationships.triangleRelation;
        }
        else if(yellow.GetTile(place) != null && playerFaction != Factions.Triangle)
        {
            otherFaction = Factions.Triangle;
            relation = relationships.triangleRelation;
            relationOther = relationships.squareRelation;
            relationOther2 = relationships.rectangleRelation;
            relationOther3 = relationships.circleRelation;
        }
        else if(green.GetTile(place) != null && playerFaction != Factions.Square)
        {
            otherFaction = Factions.Square;
            relation = relationships.squareRelation;
            relationOther = relationships.circleRelation;
            relationOther2 = relationships.rectangleRelation;
            relationOther3 = relationships.triangleRelation;
        }

        //So now we have the relation and the faction of the enemy

        for(int i = 0; i < relation.Length; i++) //Hate from other, both have same element indexes
        {
            if(playerFaction == relation[i].faction)
            {
                relation[i].relationPoints -= FactionConquer.Instance.pointDeduction;
            }
            if(otherFaction == playerRelation[i].faction)
            {
                playerRelation[i].relationPoints -= FactionConquer.Instance.pointDeduction;
            }
        }

        int indexPlayer = -1;
        int indexPlayer2 = -1;
        int indexPlayer3 = -1;
        for(int i = 0; i < relationOther.Length; i++)
        {
            if(playerFaction == relationOther[i].faction && indexPlayer == -1)
            {
                indexPlayer = i;
            }
            if(otherFaction == relationOther[i].faction && relationOther[i].relation == Relation.Hate && indexPlayer > -1)
            {
                relationOther[indexPlayer].relationPoints += FactionConquer.Instance.playerGain;
            }

            if(playerFaction == relationOther2[i].faction && indexPlayer2 == -1)
            {
                indexPlayer2 = i;
            }
            if(otherFaction == relationOther2[i].faction && relationOther2[i].relation == Relation.Hate && indexPlayer2 > -1)
            {
                relationOther2[indexPlayer2].relationPoints += FactionConquer.Instance.playerGain;
            }

            if(playerFaction == relationOther3[i].faction && indexPlayer3 == -1)
            {
                indexPlayer3 = i;
            }
            if(otherFaction == relationOther3[i].faction && relationOther3[i].relation == Relation.Hate && indexPlayer3 > -1)
            {
                relationOther3[indexPlayer3].relationPoints += FactionConquer.Instance.playerGain;
            }
        }
    }

    //Gains
    public void CalculateGain()
    {
        coins = Mathf.RoundToInt(coins * multiplyer);
        grains = Mathf.RoundToInt(grains * multiplyer);
        steel = Mathf.RoundToInt(steel * multiplyer);
        oil = Mathf.RoundToInt(oil * multiplyer);
        uranium = Mathf.RoundToInt(uranium * multiplyer);
    }
    
    //Dots
    public void InstantiateDots()
    {
        if(dots.Count > 0)
        {
            foreach(GameObject dot in dots)
            {
                Destroy(dot);
            }
            dots.Clear();
        }
        
        switch(playerFaction)
        {
            case Factions.Circle:
                tilemap = blue;
                break;

            case Factions.Rectangle:
                tilemap = red;
                break;

            case Factions.Square:
                tilemap = green;
                break;
            
            case Factions.Triangle:
                tilemap = yellow;
                break;
        }

        BoundsInt bounds = tilemap.cellBounds;
        for(int x = bounds.xMin - 1; x < bounds.xMax + 1; x++)
        {
            for(int y = bounds.yMin - 1; y < bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x,y,0);
                TileBase currentTile = tilemap.GetTile(pos);
                TileBase groundTile = ground.GetTile(pos);

                if(currentTile == null && groundTile != null && HasAnyAdjacentFactionTile(pos, tilemap)) //No already placed one and there is a ground tile
                {
                    Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                    GameObject go = Instantiate(dotPrefab, worldPos, Quaternion.identity);
                    go.transform.SetParent(dotParent);
                    go.transform.localScale = new Vector3(dotScaleOverride, dotScaleOverride, dotScaleOverride);
                    dots.Add(go);
                }
            }
        }

        HideDots();
    }
    public void ShowDots()
    {
        if(!isHidden)
        {
            return;
        }

        foreach(GameObject dot in dots)
        {
            dot.SetActive(true);
        }

        isHidden = false;
    }
    public void HideDots()
    {
        if(isHidden)
        {
            return;
        }

        foreach(GameObject dot in dots)
        {
            dot.SetActive(false);
        }

        isHidden = true;
    }
    bool HasAnyAdjacentFactionTile(Vector3Int center, Tilemap factionMap)
    {
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right, new Vector3Int(1,1,0), new Vector3Int(-1,1,0), new Vector3Int(-1,-1,0), new Vector3Int(1,-1,0)};

        foreach(Vector3Int dir in directions)
        {
            if(factionMap.GetTile(center + dir) != null)
            {
                return true;
            }
        }
        return false;
    }

    //Selecting Level
    void Update()
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
                intPos = ground.WorldToCell(worldPos);
                worldPos.z = 0f;
                if(!isHidden && isSteppingOnLand(intPos))
                {
                    UpdateWindow();
                    isOpen = true;
                    levelPlace[0] = intPos.x;
                    levelPlace[1] = intPos.y;
                    levelPlace[2] = intPos.z;
                    SoundManager.Instance.PlayClip(SoundManager.Instance.changePosition, 1f);
                }
                else
                {
                    if(isOpen)
                    {
                        SoundManager.Instance.PlayClip(SoundManager.Instance.deselectDot, 0.6f);
                    }
                    isOpen = false;
                    CloseInventoryWindow();
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
            intPos = ground.WorldToCell(worldPos);
            worldPos.z = 0f;
            if(!isHidden && isSteppingOnLand(intPos))
            {
                UpdateWindow();
                isOpen = true;
                levelPlace[0] = intPos.x;
                levelPlace[1] = intPos.y;
                levelPlace[2] = intPos.z;
                SoundManager.Instance.PlayClip(SoundManager.Instance.changePosition, 1f);
            }
            else
            {
                if(isOpen)
                {
                    SoundManager.Instance.PlayClip(SoundManager.Instance.deselectDot, 0.6f);
                }
                isOpen = false;
                CloseInventoryWindow();
            }
        }
    }
    void UpdateWindow()
    {
        headerVisual.text = bases[factionIndex].tileName;
        whiteSpace.color = bases[factionIndex].windowColor;

        int landCoins = Mathf.RoundToInt(coins * bases[factionIndex].multiplyer * multiplyer);
        int landGrains = Mathf.RoundToInt(grains * bases[factionIndex].multiplyer * multiplyer);
        int landSteel = Mathf.RoundToInt(steel * bases[factionIndex].multiplyer * multiplyer);
        int landOil = Mathf.RoundToInt(oil * bases[factionIndex].multiplyer * multiplyer);
        int landUranium = Mathf.RoundToInt(uranium * bases[factionIndex].multiplyer * multiplyer);
        int landWaves = Mathf.RoundToInt(waves * bases[factionIndex].multiplyer * multiplyer);

        coinsVisual.text = landCoins.ToString();
        grainVisual.text = landGrains.ToString();
        steelVisual.text = landSteel.ToString();
        oilVisual.text = landOil.ToString();
        uraniumVisual.text = landUranium.ToString();
        waveVisual.text = "Waves : " + landWaves.ToString();
    }
    bool isSteppingOnLand(Vector3Int pos)
    {
        Vector3 tilePos = ground.GetCellCenterWorld(pos);
        TileBase greenTile = green.GetTile(pos);
        TileBase blueTile = blue.GetTile(pos);
        TileBase redTile = red.GetTile(pos);
        TileBase yellowTile = yellow.GetTile(pos);

        factionIndex = -1;
        
        for(int i = 0; i < bases.Length; i++)
        {
            if(yellowTile != null && yellowTile == bases[i].tile)
            {
                factionIndex = i;
                enemyFaction = Factions.Triangle;
                break;
            }
            else if(greenTile != null && greenTile == bases[i].tile)
            {
                factionIndex = i;
                enemyFaction = Factions.Square;
                break;
            }
            else if(blueTile != null && blueTile == bases[i].tile)
            {
                factionIndex = i;
                enemyFaction = Factions.Circle;
                break;
            }
            else if(redTile != null && redTile == bases[i].tile)
            {
                factionIndex = i;
                enemyFaction = Factions.Rectangle;
                break;
            }
            else
            {
                factionIndex = i;
                enemyFaction = Factions.Neutral;
            }
        }
        for(int i = 0; i < dots.Count; i++)
        {
            if(tilePos == dots[i].transform.position)
            {
                if(lastDot != null && lastDot.TryGetComponent(out SpriteRenderer rend))
                {
                    rend.color = dotInactive;
                }
                if(dots[i].TryGetComponent(out SpriteRenderer rend1))
                {
                    rend1.color = dotActive;
                }
                lastDot = dots[i];
                return true;
            }
        }

        if(lastDot != null && lastDot.TryGetComponent(out SpriteRenderer rend2))
        {
            rend2.color = dotInactive;
        }
        return false;
    }

    //Inventory
    public void BothInventoryWindow()
    {
        inventoryOn = !inventoryOn;

        if(inventoryOn)
        {
            OpenInventoryWindow();
            UpdateInventoryWindow();
        }
        else
        {
            CloseInventoryWindow();
        }
    }

    void OpenInventoryWindow()
    {
        inventoryWindow.SetActive(true);
        inventoryOn = true;
    }

    void CloseInventoryWindow()
    {
        inventoryWindow.SetActive(false);
        inventoryOn = false;
    }

    void CheckAddButton()
    {
        if(currentTowers.Count >= 5)
        {
            addButton.SetActive(false);
        }
        else
        {
            addButton.SetActive(true);
        }
    }

    public void UpdateInventoryWindow()
    {
        Inventory inventory = Inventory.Instance;

        if(slotParent.childCount > 0)
        {
            for(int i = slotParent.childCount - 1; i >= 0; i--)
            {
                Transform child = slotParent.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        for(int i = 0; i < inventory.towers.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotParent.position, Quaternion.identity);
            go.transform.SetParent(slotParent);
            go.transform.localScale = Vector3.one;
            go.transform.rotation = slotParent.rotation;
            if(go.TryGetComponent(out TowerSlotInventory goScript))
            {
                if(currentTowers.Contains(inventory.towers[i]))
                {
                    if(go.TryGetComponent(out Button button))
                    {
                        button.interactable = false;
                    }
                }
                goScript.slot = inventory.towers[i];
                goScript.slotIndex = inventory.towerIndex[i];
                goScript.Refresh();
            }
        } 
    }

    public void AddToCurrentTowers(TowerSlotSO slot, Color color, int index)
    {
        GameObject go = Instantiate(displayPrefab, displayParent.position, Quaternion.identity);
        go.transform.SetParent(displayParent);
        go.transform.localScale = Vector3.one;
        go.transform.rotation = displayParent.rotation;
        if(go.TryGetComponent(out InventoryDisplay goScript))
        {
            goScript.slot = slot;
            goScript.iconImage.sprite = slot.icon;
            goScript.image.color = color;
            goScript.slotIndex = index;
        }

        currentTowers.Add(slot);
        currentTowersObj.Add(go);
        currentTowersIndex.Add(index);

        CloseInventoryWindow();
        CheckAddButton();
    }
    public void RemoveCurrentTowers(TowerSlotSO slot, GameObject obj, int index)
    {
        currentTowers.Remove(slot);
        currentTowersObj.Remove(obj);
        currentTowersIndex.Remove(index);
        Destroy(obj);
        CheckAddButton();
    }

    //Play
    public void OpenPlayWindow()
    {
        if(currentTowersIndex.Count == 0)
        {
            Notification.Instance.PutNotification("You need atleast 1 tower for battle.");
            return;
        }
        playWindow.SetActive(true);
    }

    public void ClosePlayWindow()
    {
        StartCoroutine(CloseWindow(playWindow, playWindowAnim, playWindowCloseDuration));
    }

    public void ConfirmPlay()
    {
        saveLevel = true;
        DataPersistenceManager.instance.SaveGame();
        StartCoroutine(GoToBattle());
    }

    IEnumerator GoToBattle()
    {
        leaveTransition.SetActive(true);
        yield return new WaitForSecondsRealtime(leaveDuration);
        DataPersistenceManager.instance.SaveGame();
        SceneManager.LoadScene("Game");
    }

    IEnumerator CloseWindow(GameObject window, Animator anim, float duration)
    {
        anim.SetTrigger("Close");
        yield return new WaitForSeconds(duration);
        window.SetActive(false);
    }

    void FixedUpdate()
    {
        if(isOpen)
        {
            // Vector3Int pos = intPos;
            // pos.z = -10;
            // pos.x += 1;
            // pos.y += 1;
            levelWindow.transform.position = Vector3.Lerp(levelWindow.transform.position, windowStates[1].position, Time.deltaTime * windowSpeed);
            // cam.transform.position = Vector3.Lerp(cam.transform.position, pos, Time.deltaTime * moveSpeed);
            // cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, zoom, Time.deltaTime * zoomSpeed);
        }
        else
        {
            levelWindow.transform.position = Vector3.Lerp(levelWindow.transform.position, windowStates[0].position, Time.deltaTime * windowSpeed);
        }
    }
}
