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
    public Pool criticalPool;
    public Pool splashPool;
    public Settings settings;
    public TowerPrefab[] towerPrefab;
    private List<GameObject> display = new List<GameObject>();
    private int previousIndex;
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

    [Header("Keep track of TowerSlots")]
    public TowerSlot currentSlot;

    [Header("Fortower")]
    public UpgradeManager upgradeManager;
    public float searchDelay = 0.5f;


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
        StopAllCoroutines();
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

        foreach(TowerPrefab script in towerPrefab)
        {
            GameObject go = Instantiate(script.display, Vector3.one, Quaternion.identity);
            go.SetActive(false);
            display.Add(go);
        }

        StartCoroutine(SearchDelay());

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
            if(go.TryGetComponent(out Tower goScript))
            {
                goScript.manager = this;
                goScript.criticalPool = criticalPool;
                goScript.splashPool = splashPool;
                goScript.upgradeManager = upgradeManager;
                goScript.upgrade.sellValue = Mathf.FloorToInt(towerPrefab[currentPrefabIndex].price * settings.sellPercentage);
                tower.Add(goScript);
            }
            selectedPrefab = null;
            isSelecting = false;
            displayPrefab.SetActive(false);
            isFull[index] = true;
        }
    }
    public void UnlockSelection(int index, TowerSlot script)
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
        if(currentSlot != null)
        {
            currentSlot.isSelected = false;
        }
        currentSlot = script;
        display[previousIndex].SetActive(false);
        displayPrefab = display[index];
        displayPrefab.SetActive(false);
        for(int i = 0; i < dots.Count; i++)
        {
            dots[i].SetActive(isFull[i] ? false : true);
        }
        previousIndex = index;
    }
    public void InactiveSelection()
    {
        foreach(GameObject obj in placeButtons)
        {
            obj.SetActive(false);
        }
        selectedPrefab = null;
        isSelecting = false;
        currentSlot = null;
        displayPrefab.SetActive(false);
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

    IEnumerator SearchDelay()
    {
        bool loop = true;
        
        while(loop)
        {
            yield return new WaitForSeconds(searchDelay);
            foreach(Tower script in tower)
            {
                script.UpdateValues();
                script.SelectEnemy();
            }
        }
    }
}
