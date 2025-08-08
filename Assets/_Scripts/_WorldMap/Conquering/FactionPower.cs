using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class FactionStrength
{
    public Factions faction;
    public Tilemap tilemap;
    public int strength;
}

public class FactionPower : MonoBehaviour, IDataPersistence
{
    public static FactionPower Instance {get; private set;}
    [Header("Faction Strength")]
    public FactionStrength[] factionStrength;
    public int[] strength = new int[4];
    bool hasRecieved = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.strength = data.strength;
        ReturnToFaction();
        hasRecieved = true;
    }

    public void SaveData(GameData data)
    {
        if(hasRecieved)
        {
            GoToStrength();
            data.strength = this.strength;
        }
    }

    void ReturnToFaction()
    {
        for(int i = 0 ; i < factionStrength.Length; i++)
        {
            factionStrength[i].strength = strength[i];
        }
    }

    void GoToStrength()
    {
        for(int i = 0; i < factionStrength.Length; i++)
        {
            strength[i] = factionStrength[i].strength;
        }
    }

    public void CalculateStrength()
    {
        foreach(var faction in factionStrength)
        {
            faction.strength = 0;
            BoundsInt bounds = faction.tilemap.cellBounds;

            for(int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for(int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if(faction.tilemap.GetTile(pos) != null)
                    {
                        faction.strength++;
                    }
                }
            }
        }
    }
}