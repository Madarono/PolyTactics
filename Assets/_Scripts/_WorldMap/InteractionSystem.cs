using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
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
    public Camera cam;


    [Header("Tilemaps")]
    public Tilemap ground;
    public Tilemap blue;
    public Tilemap red;
    public Tilemap green;
    public Tilemap yellow;
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

    [Header("Choosing Inventory")]
    public List<TowerSlotSO> currentTowers = new List<TowerSlotSO>();
    public List<GameObject> currentTowersObj = new List<GameObject>();
    
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

    private GameObject lastDot;
    private int factionIndex; //For the tilebase

    Vector3Int intPos;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.coins = data.a_coins;
        this.grains = data.a_grains;
        this.steel = data.a_steel;
        this.oil = data.a_oil;
        this.uranium = data.a_uranium;
        this.multiplyer = data.resourceMultiplyer;

        this.playerFaction = data.playerFaction;
        PerlinNoise.Instance.InstantiateStart();
        InstantiateDots();
        CalculateGain();
    }

    public void SaveData(GameData data)
    {
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
                    SoundManager.Instance.PlayClip(SoundManager.Instance.selectDot, 1f);
                }
                else
                {
                    isOpen = false;
                    SoundManager.Instance.PlayClip(SoundManager.Instance.deselectDot, 1f);
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
                SoundManager.Instance.PlayClip(SoundManager.Instance.changePosition, 1f);
            }
            else
            {
                if(isOpen)
                {
                    SoundManager.Instance.PlayClip(SoundManager.Instance.deselectDot, 0.8f);
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

        float landCoins = coins * bases[factionIndex].multiplyer;
        float landGrains = grains * bases[factionIndex].multiplyer;
        float landSteel = steel * bases[factionIndex].multiplyer;
        float landOil = oil * bases[factionIndex].multiplyer;
        float landUranium = uranium * bases[factionIndex].multiplyer;

        coinsVisual.text = landCoins.ToString();
        grainVisual.text = landGrains.ToString();
        steelVisual.text = landSteel.ToString();
        oilVisual.text = landOil.ToString();
        uraniumVisual.text = landUranium.ToString();
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
                break;
            }
            else if(greenTile != null && greenTile == bases[i].tile)
            {
                factionIndex = i;
                break;
            }
            else if(blueTile != null && blueTile == bases[i].tile)
            {
                factionIndex = i;
                break;
            }
            else if(redTile != null && redTile == bases[i].tile)
            {
                factionIndex = i;
                break;
            }
        }

        if(factionIndex == -1)
        {
            factionIndex = bases.Length - 1;
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

        foreach(TowerSlotSO tower in inventory.towers)
        {
            GameObject go = Instantiate(slotPrefab, slotParent.position, Quaternion.identity);
            go.transform.SetParent(slotParent);
            go.transform.localScale = Vector3.one;
            go.transform.rotation = slotParent.rotation;
            if(go.TryGetComponent(out TowerSlotInventory goScript))
            {
                if(currentTowers.Contains(tower))
                {
                    if(go.TryGetComponent(out Button button))
                    {
                        button.interactable = false;
                    }
                }
                goScript.slot = tower;
                goScript.Refresh();
            }
        } 
    }

    public void AddToCurrentTowers(TowerSlotSO slot, Color color)
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
        }

        currentTowers.Add(slot);
        currentTowersObj.Add(go);

        CloseInventoryWindow();
        CheckAddButton();
    }
    public void RemoveCurrentWoers(TowerSlotSO slot, GameObject obj)
    {
        currentTowers.Remove(slot);
        currentTowersObj.Remove(obj);
        Destroy(obj);
        CheckAddButton();
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
