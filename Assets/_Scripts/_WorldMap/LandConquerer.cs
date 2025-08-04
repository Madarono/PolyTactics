using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class LandConquerer : MonoBehaviour, IDataPersistence
{
    public static LandConquerer Instance {get; private set;}

    public List<int> landPlaces = new List<int>();
    public List<Factions> landFaction = new List<Factions>();

    public Tilemap blue;
    public TileBase blueTile;
    
    public Tilemap red;
    public TileBase redTile;

    public Tilemap yellow;
    public TileBase yellowTile;

    public Tilemap green;
    public TileBase greenTile;

    [Header("Checking others")]
    public Tilemap[] tilemaps;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.landPlaces = data.landPlaces.ToList();
        this.landFaction = data.landFaction.ToList();
        // ApplyFaction(data.landFaction);
    } 

    public void SaveData(GameData data)
    {
        data.landPlaces = this.landPlaces.ToArray();  
        data.landFaction = landFaction.ToArray();
    } 

    public void ApplyFaction(int[] factionIndexes)
    {
        Factions playerFaction = InteractionSystem.Instance.playerFaction;
        foreach(int index in factionIndexes)
        {
            switch(index)
            {
                case 0:
                    landFaction.Add(Factions.Circle);
                    break;

                case 1:
                    landFaction.Add(Factions.Rectangle);
                    break;

                case 2:
                    landFaction.Add(Factions.Triangle);
                    break;

                case 3:
                    landFaction.Add(Factions.Square);
                    break;

                case 4:
                    landFaction.Add(playerFaction);
                    break;
            }
        }
    }

    int[] SwitchFaction()
    {
        Factions playerFaction = InteractionSystem.Instance.playerFaction;
        int[] newFactionIndexes = new int[landFaction.Count];

        for(int i = 0; i < landFaction.Count; i++)
        {
            switch(landFaction[i])
            {
                case Factions.Circle:
                    newFactionIndexes[i] = 0;
                    break;

                case Factions.Rectangle:
                    newFactionIndexes[i] = 1;
                    break;

                case Factions.Triangle:
                    newFactionIndexes[i] = 2;
                    break;

                case Factions.Square:
                    newFactionIndexes[i] = 3;
                    break;

                case Factions.Neutral:
                    switch(playerFaction)
                    {
                        case Factions.Circle:
                            newFactionIndexes[i] = 0;
                            break;

                        case Factions.Rectangle:
                            newFactionIndexes[i] = 1;
                            break;

                        case Factions.Triangle:
                            newFactionIndexes[i] = 2;
                            break;

                        case Factions.Square:
                            newFactionIndexes[i] = 3;
                            break;
                    }
                    break;
            }
        }

        return newFactionIndexes;
    }

    public void AddPlaces(int[] places, Factions faction)
    {
        for(int i = 0; i < landPlaces.Count; i += 3)
        {
            if(places[0] == landPlaces[i] && places[1] == landPlaces[i + 1] && places[2] == landPlaces[i + 2])
            {
                landFaction[i / 3] = faction;
                return;
            }
        }

        foreach(int place in places)
        {
            landPlaces.Add(place);
        }
        landFaction.Add(faction);
    }

    public void ApplyPlaces()
    {
        Factions playerFaction = InteractionSystem.Instance.playerFaction;
        for(int i = 0; i < landPlaces.Count; i += 3)
        {
            int factionIndex = i / 3;
            Tilemap tilemap = blue;
            TileBase tileBase = greenTile;
            switch(landFaction[factionIndex])
            {
                case Factions.Circle:
                    tilemap = blue;
                    tileBase = blueTile;
                    break;
                    
                case Factions.Rectangle:
                    tilemap = red;
                    tileBase = redTile;
                    break;

                case Factions.Triangle:
                    tilemap = yellow;
                    tileBase = yellowTile;
                    break;

                case Factions.Square:
                    tilemap = green;
                    tileBase = greenTile;
                    break;

                case Factions.Neutral:
                    switch(playerFaction)
                    {
                        case Factions.Circle:
                            tilemap = blue;
                            tileBase = blueTile;
                            break;

                        case Factions.Rectangle:
                            tilemap = red;
                            tileBase = redTile;
                            break;

                        case Factions.Triangle:
                            tilemap = yellow;
                            tileBase = yellowTile;
                            break;

                        case Factions.Square:
                            tilemap = green;
                            tileBase = greenTile;
                            break;
                    }
                    break;
            }
            Vector3Int placePos = new Vector3Int(landPlaces[i], landPlaces[i+1], landPlaces[i+2]);

            tilemap.SetTile(placePos, tileBase);
            for(int p = 0; p < tilemaps.Length; p++)
            {
                if(tilemaps[p] == tilemap)
                {
                    continue;
                }
                
                TileBase tile = tilemaps[p].GetTile(placePos);
                if(tile != null)
                {
                    tilemaps[p].SetTile(placePos, null);
                }
            }
        }
    }
}