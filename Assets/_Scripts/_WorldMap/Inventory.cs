using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour, IDataPersistence
{
    public static Inventory Instance {get; private set;}
    public TowerSlotSO[] towersList;

    public List<TowerSlotSO> towers = new List<TowerSlotSO>();
    public List<int> towerIndex = new List<int>();

    [Header("Debug")]
    public int index;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        towerIndex = data.towerIndex.ToList();
        LoadTowers();
    }

    public void SaveData(GameData data)
    {
        data.towerIndex = towerIndex.ToArray();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            AddNewTower(index);
        }
    }

    void LoadTowers()
    {
        towers.Clear();

        foreach(int index in towerIndex)
        {
            towers.Add(towersList[index]);
        }
    }

    void AddNewTower(int index)
    {
        towers.Add(towersList[index]);
        towerIndex.Add(index);
        DataPersistenceManager.instance.SaveGame();
    }


}
