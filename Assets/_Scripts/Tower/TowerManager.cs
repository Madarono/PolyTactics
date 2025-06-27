using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

[System.Serializable]
public class TowerPrefab
{
    public GameObject prefab;
    public GameObject display;
    public int price;
}


public class TowerManager : MonoBehaviour
{
    public Settings settings;
    public TowerPrefab[] towerPrefab;
    public GameObject[] placeButtons;
    
    [Header("Tilemap")]
    public Tilemap tilemap;
    public List<Vector3Int> tilePositions = new List<Vector3Int>();
    public List<bool> isFull = new List<bool>();
    public List<GameObject> dots = new List<GameObject>();
    public GameObject dotPrefab;
    public Transform dotParent;

    public bool isSelecting;

    [Header("Towers")]
    public List<Tower> tower = new List<Tower>();
    public TextMeshProUGUI priceVisual;

    [Header("Scripts for tower")]
    public UpgradeManager upgradeManager;


    private GameObject selectedPrefab;
    private int currentPrefabIndex;
    private GameObject displayPrefab;
    private Camera cam;
    private Vector3 mouseWorldPos;
    private Vector3Int cellPos;
    private TileBase clickedTile;
    private int cacheDotIndex;

    void Start()
    {
        foreach(GameObject obj in placeButtons)
        {
            obj.SetActive(false);
        }
        cam = Camera.main;
        BoundsInt bounds = tilemap.cellBounds;

        foreach(Vector3Int pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
            if(tile != null)
            {
                tilePositions.Add(pos);
                isFull.Add(false);
                GameObject go = Instantiate(dotPrefab, worldPos, Quaternion.identity);
                go.SetActive(false);
                go.transform.SetParent(dotParent);
                dots.Add(go);
            }
        }

    }

    void Update()
    {
        if (isSelecting)
        {
            if (Application.isMobilePlatform && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    {
                        return;
                    }

                    HandlePlacement(touch.position);
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return;
                }

                HandlePlacement(Input.mousePosition);
            }
        }

        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return; 
                }

                HideOtherTowerInfo();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            HideOtherTowerInfo();
        }
    }

    void HandlePlacement(Vector3 inputPosition)
    {
        mouseWorldPos = cam.ScreenToWorldPoint(inputPosition);
        cellPos = tilemap.WorldToCell(mouseWorldPos);
        clickedTile = tilemap.GetTile(cellPos);
        int tileIndex = tilePositions.IndexOf(cellPos);
    
        bool validTile = clickedTile != null && tileIndex != -1 && !isFull[tileIndex];
    
        displayPrefab.SetActive(validTile);
        dots[cacheDotIndex].SetActive(true);
    
        if (validTile)
        {
            displayPrefab.transform.position = tilemap.GetCellCenterWorld(cellPos);
            dots[tileIndex].SetActive(false);
            cacheDotIndex = tileIndex;
        }
    }



    public void ConfirmPlaceTower()
    {
        int tileIndex = tilePositions.IndexOf(cellPos);
        bool validTile = clickedTile != null && tileIndex != -1 && !isFull[tileIndex];
        
        if(validTile && settings.money >= towerPrefab[currentPrefabIndex].price)
        {
            settings.money -= towerPrefab[currentPrefabIndex].price;
            settings.UpdateVisual();
            PlaceTower(cellPos, tileIndex);
            InactiveSelection();
        }
    }
    void PlaceTower(Vector3Int position, int index)
    {
        if (selectedPrefab == null)
        {
            return;
        }

        if (!isFull[index])
        {
            Vector3 worldPos = tilemap.GetCellCenterWorld(position);
            GameObject go = Instantiate(selectedPrefab, worldPos, Quaternion.identity);
            Tower goScript = go.GetComponent<Tower>();
            goScript.manager = this;
            goScript.upgradeManager = upgradeManager;
            // goScript.upgrade.sellValue = (int)Mathf.Round(towerPrefab[currentPrefabIndex].price * settings.sellPercentage);
            goScript.upgrade.sellValue = Mathf.FloorToInt(towerPrefab[currentPrefabIndex].price * settings.sellPercentage);
            tower.Add(goScript);
            selectedPrefab = null;
            isSelecting = false;
            Destroy(displayPrefab);
            isFull[index] = true;
        }
    }
    public void UnlockSelection(int index)
    {
        foreach(GameObject obj in placeButtons)
        {
            obj.SetActive(true);
        }
        HideOtherTowerInfo();
        selectedPrefab = towerPrefab[index].prefab;
        currentPrefabIndex = index;
        priceVisual.text = "$" + towerPrefab[index].price.ToString();
        isSelecting = true;
        displayPrefab = Instantiate(towerPrefab[index].display, Vector3.zero, Quaternion.identity);
        displayPrefab.SetActive(false);
        for(int i = 0; i < dots.Count; i++)
        {
            dots[i].SetActive(isFull[i] ? false : true);
        }
    }
    public void InactiveSelection()
    {
        foreach(GameObject obj in placeButtons)
        {
            obj.SetActive(false);
        }
        selectedPrefab = null;
        isSelecting = false;
        Destroy(displayPrefab);
        foreach(GameObject dot in dots)
        {
            dot.SetActive(false);
        }
    }

    public void HideOtherTowerInfo()
    {
        foreach(Tower script in tower)
        {
            script.HideInfo();
            script.HideRange();
        }

        upgradeManager.HideLevelRemovers();
    }
}
