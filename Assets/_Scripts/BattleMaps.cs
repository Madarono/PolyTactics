using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Map
{
    public Tilemap groundTilemap;
    public Tilemap roadTilemap;
    public GameObject mapObj;

    [Header("Enemy Info")]
    public Transform[] waypoints;
    public Transform spawnpoint;
}

public class BattleMaps : MonoBehaviour, IDataPersistence
{
    public Map[] maps;
    public int mapIndex = 0;
    public bool randomIndex = true;

    public void LoadData(GameData data)
    {
        this.mapIndex = data.mapIndex;
        this.randomIndex = data.randomizeMapIndex;
        ApplyMap();
    }

    public void SaveData(GameData data)
    {
    }

    void ApplyMap()
    {
        int index = randomIndex ? Random.Range(0, maps.Length) : mapIndex;
        foreach(Map map in maps)
        {
            map.mapObj.SetActive(false);
        }

        TowerManager.Instance.tilemap = maps[index].groundTilemap;
        TowerManager.Instance.trapTilemap = maps[index].roadTilemap;
        EnemyManager.Instance.waypoints = (Transform[])maps[index].waypoints.Clone();
        EnemyManager.Instance.spawnPoint = maps[index].spawnpoint;
        GrassRandomizer.Instance.tilemap = maps[index].groundTilemap;
        GrassRandomizer.Instance.RandomizeGrass();
        maps[index].mapObj.SetActive(true);
        TowerManager.Instance.InstantiateStart();
    }
}
