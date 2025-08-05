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

    [Header("StockTowers")]
    public int[] indexBlue;
    public int[] indexRed;
    public int[] indexYellow;
    public int[] indexGreen;

    [Header("Debug")]
    public int index;
    public bool debug = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        towerIndex = data.towerIndex.ToList();
        // LoadTowers(); - It is done by the InteractionSystem.cs
    }

    public void SaveData(GameData data)
    {
        data.towerIndex = towerIndex.ToArray();
    }

    void Update()
    {
        if(!debug)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            AddNewTower(index);
        }
    }

    public void LoadTowers()
    {
        towers.Clear();

        foreach(int index in towerIndex)
        {
            towers.Add(towersList[index]);
        }
    }

    void AddNewTower(int index)
    {
        if(towerIndex.Contains(index))
        {
            return;
        }

        towers.Add(towersList[index]);
        towerIndex.Add(index);
        DataPersistenceManager.instance.SaveGame();
    }

    public void LoadStockTowers()
    {
        if(towerIndex.Count > 0)
        {
            return;
        }

        int[] indexes = new int[3];
        switch(InteractionSystem.Instance.playerFaction)
        {
            case Factions.Circle:
                indexes = (int[])indexBlue.Clone();
                break;

            case Factions.Rectangle:
                indexes = (int[])indexRed.Clone();
                break;

            case Factions.Triangle:
                indexes = (int[])indexYellow.Clone();
                break;

            case Factions.Square:
                indexes = (int[])indexGreen.Clone();
                break;
        }

        foreach(int index in indexes)
        {
            AddNewTower(index);
        }
    }


}
