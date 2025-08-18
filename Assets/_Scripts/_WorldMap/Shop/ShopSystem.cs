using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ShopSystem : MonoBehaviour, IDataPersistence
{
    public static ShopSystem Instance {get; private set;}
    private ResourcesStorage storage;
    public GameObject window;
    public GameObject canvas;
    public float closeDuration = 0.17f;
    public Animator windowAnim;

    [Header("Visual")]
    public ShopItemSO[] items;
    public GameObject itemPrefab;
    public Transform itemParent;
    public TextMeshProUGUI coinsVisual;

    private List<ShopItem> shopItems = new List<ShopItem>();
    private List<int> shopSaved = new List<int>();
    public HashSet<TowerSlotSO> slots = new HashSet<TowerSlotSO>();
    private bool hasRecieved = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.shopSaved = data.shopSaved.ToList();
        hasRecieved = true;
        InitializeLoad();
        FormWindow();
    }

    public void SaveData(GameData data)
    {
        if(hasRecieved)
        {
            InitializeToSave();
            data.shopSaved = this.shopSaved.ToArray();
        }
    }

    void InitializeToSave()
    {
        shopSaved.Clear();
        foreach(var slot in slots)
        {
            for(int i = 0; i < Inventory.Instance.towersList.Length; i++)
            {
                if(slot == Inventory.Instance.towersList[i])
                {
                    shopSaved.Add(i);
                    break;
                }
            }
        }
    }

    void InitializeLoad()
    {
        slots.Clear();
        foreach(var slot in shopSaved)
        {
            slots.Add(Inventory.Instance.towersList[slot]);
        }
        Inventory.Instance.shopTowers = slots.ToList();
        Inventory.Instance.shopIndex = shopSaved;
    }

    void Start()
    {
        storage = ResourcesStorage.Instance;
        canvas.SetActive(false);
        window.SetActive(false);
    }

    public void OpenWindow()
    {
        canvas.SetActive(true);
        window.SetActive(true);
        Time.timeScale = 0f;
        UpdateWindow();
    }

    public void CloseWindow()
    {
        Time.timeScale = 1f;
        StartCoroutine(closeWindow(window, windowAnim, true));
    }

    void FormWindow()
    {
        if(itemParent.childCount > 0)
        {
            for(int i = itemParent.childCount - 1; i >= 0; i--)
            {
                Transform child = itemParent.GetChild(i);
                Destroy(child.gameObject);
            }
        }
        foreach(var item in items)
        {
            GameObject go = Instantiate(itemPrefab, itemParent.position, Quaternion.identity);
            go.transform.SetParent(itemParent);
            go.transform.localScale = Vector3.one;
            if(go.TryGetComponent(out ShopItem goScript))
            {
                goScript.Refresh(item.price, item.name, item.icon, item.slot);
                shopItems.Add(goScript);
                if(slots.Contains(goScript.slot))
                {
                    goScript.Inactive();
                }
                else
                {
                    goScript.Active();
                }
            } 
        }
    }

    void UpdateWindow()
    {
        coinsVisual.text = $"${storage.coins}";
    }

    public void Buy(TowerSlotSO slot, ShopItem item)
    {
        slots.Add(slot);
        item.Inactive();
        Inventory.Instance.shopTowers = slots.ToList();
        UpdateWindow();
    }

    IEnumerator closeWindow(GameObject window, Animator anim, bool closeCanvas)
    {
        anim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(closeDuration);
        window.SetActive(false);

        if(closeCanvas)
        {
            canvas.SetActive(false);
        }
    }
}
