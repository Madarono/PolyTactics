using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FactionConquer : MonoBehaviour
{
    public static FactionConquer Instance {get; private set;}
    
    public Factions playerFaction;
    public float groundChance = 10f;
    public float factionChanceHate = 25f; 
    public float factionChanceNeutral = 10f;

    public Dictionary<Vector3Int, TileBase> tilesByPosition = new();

    [Header("Tilemaps")]
    public Tilemap ground;
    public Tilemap blue;
    public Tilemap red;
    public Tilemap yellow;
    public Tilemap green;
    private Tilemap tilemap;

    [Header("TileBases")]
    public TileBase blueTile;
    public TileBase redTile;
    public TileBase yellowTile;
    public TileBase greenTile;

    [Header("AfterMath")]
    public int pointDeduction = 10;

    [Header("Debug")]
    public bool debug = false;

    void Awake()
    {
        Instance = this;
        tilesByPosition.Clear();
    }

    void Update()
    {
        if(!debug)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            ConquerForAll();
        }
    }

    public void Conquer(Factions attacker, FactionRelation[] relations, Tilemap[] tilemaps, Tilemap attackerTilemap)
    {
        tilesByPosition.Clear();
        tilemap = attackerTilemap;
        BoundsInt bounds = tilemap.cellBounds;
        for(int x = bounds.xMin - 1; x < bounds.xMax + 1; x++)
        {
            for(int y = bounds.yMin - 1; y < bounds.yMax + 1; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if(tilemap.GetTile(pos) == null) 
                {
                    continue;
                }

                List<(Vector3Int pos, TileBase tile)> adjacentFactionTiles = GetAdjacentFactionTiles(pos, tilemaps);
                foreach(var (adjacent, adjacentTiles) in adjacentFactionTiles)
                {
                    if(!tilesByPosition.ContainsKey(adjacent))
                    {
                        tilesByPosition[adjacent] = adjacentTiles;
                    }
                }
            }
        }

        List<Vector3Int> places = new List<Vector3Int>(); 
        
        foreach(var pair in tilesByPosition) //Imma probably continue with var, this is actually pretty cool
        {        
            Vector3Int pos = pair.Key;
            TileBase tile = pair.Value;
            float chance = Random.Range(0, 100f);

            Relation blueRelation = Relation.Like;
            Relation redRelation = Relation.Like;
            Relation yellowRelation = Relation.Like;
            Relation greenRelation = Relation.Like;
            foreach(var relation in relations) //To get relation
            {
                switch(relation.faction)
                {
                    case Factions.Circle:
                        blueRelation = relation.relation;
                        break;

                    case Factions.Rectangle:
                        redRelation = relation.relation;
                        break;

                    case Factions.Triangle:
                        yellowRelation = relation.relation;
                        break;

                    case Factions.Square:
                        greenRelation = relation.relation;
                        break;
                }
            }

            if(tile == blueTile && ((chance <= factionChanceHate && blueRelation != Relation.Hate) || (chance <= factionChanceNeutral && blueRelation != Relation.Neutral)))
            {
                places.Add(pos);
            }
            else if(tile == redTile && ((chance <= factionChanceHate && redRelation != Relation.Hate) || (chance <= factionChanceNeutral && redRelation != Relation.Neutral)))
            {
                places.Add(pos);
            }
            else if(tile == yellowTile && ((chance <= factionChanceHate && yellowRelation != Relation.Hate) || (chance <= factionChanceNeutral && yellowRelation != Relation.Neutral)))
            {
                places.Add(pos);
            }
            else if(tile == greenTile && ((chance <= factionChanceHate && greenRelation != Relation.Hate) || (chance <= factionChanceNeutral && greenRelation != Relation.Neutral)))
            {
                places.Add(pos);
            }
        }
        if(places.Count == 0)
        {
            List<Vector3Int> groundPlaces = new List<Vector3Int>();
            for(int x = bounds.xMin - 1; x < bounds.xMax + 1; x++)
            {
                for(int y = bounds.yMin - 1; y < bounds.yMax + 1; y++)
                {
                    Vector3Int posGround = new Vector3Int(x, y, 0);
                    if(ground.GetTile(posGround) != null && HasAnyAdjacentFactionTile(posGround, attackerTilemap))
                    {
                        groundPlaces.Add(posGround);
                    }
                }
            }

            if(groundPlaces.Count > 0)
            {
                Vector3Int finalPlaceGround = groundPlaces[Random.Range(0, groundPlaces.Count)];
                int[] placeGroundInt = new int[3] {finalPlaceGround.x, finalPlaceGround.y, finalPlaceGround.z};
                LandConquerer.Instance.AddPlaces(placeGroundInt, attacker);
                LandConquerer.Instance.ApplyPlaces();
            }
            return;
        }


        int random = Random.Range(0, places.Count);
        Vector3Int finalPlace = places[random];
        TileBase endTile = ground.GetTile(finalPlace);

        if (tilesByPosition.TryGetValue(finalPlace, out TileBase possibleTile)) 
        {
            endTile = possibleTile;
        }

        if(endTile == possibleTile) //This ain't a ground tile, since the dictionary didnt put it
        {
            Relationships relationships = Relationships.Instance; 
            for(int i = 0; i < relations.Length; i++)
            {
                if(endTile == blueTile && relations[i].faction == Factions.Circle)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints - pointDeduction, 0);
                    for(int o = 0; o < relationships.circleRelation.Length; o++)
                    {
                        if(attacker == relationships.circleRelation[o].faction)
                        {
                            relationships.circleRelation[o].relationPoints = Mathf.Max(relationships.circleRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.circleRelation);
                            break;
                        }
                    }
                    break;   
                }
                else if(endTile == redTile && relations[i].faction == Factions.Rectangle)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints -= pointDeduction, 0);
                    for(int o = 0; o < relationships.rectangleRelation.Length; o++)
                    {
                        if(attacker == relationships.rectangleRelation[o].faction)
                        {
                            relationships.rectangleRelation[o].relationPoints = Mathf.Max(relationships.rectangleRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.rectangleRelation);
                            break;
                        }
                    }
                    break;   
                }
                else if(endTile == yellowTile && relations[i].faction == Factions.Triangle)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints -= pointDeduction, 0);
                    for(int o = 0; o < relationships.triangleRelation.Length; o++)
                    {
                        if(attacker == relationships.triangleRelation[o].faction)
                        {
                            relationships.triangleRelation[o].relationPoints = Mathf.Max(relationships.triangleRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.triangleRelation);
                            break;
                        }
                    }
                    break;   
                }
                else if(endTile == greenTile && relations[i].faction == Factions.Square)
                {
                    relations[i].relationPoints = Mathf.Max(relations[i].relationPoints -= pointDeduction, 0);
                    for(int o = 0; o < relationships.squareRelation.Length; o++)
                    {
                        if(attacker == relationships.squareRelation[o].faction)
                        {
                            relationships.squareRelation[o].relationPoints = Mathf.Max(relationships.squareRelation[o].relationPoints - pointDeduction, 0);
                            relationships.CheckRelation(relationships.squareRelation);
                            break;
                        }
                    }
                    break;   
                }
            }
            relationships.CheckRelation(relations);
        }
        int[] placeInt = new int[3] {finalPlace.x, finalPlace.y, finalPlace.z};

        LandConquerer.Instance.AddPlaces(placeInt, attacker);
        LandConquerer.Instance.ApplyPlaces();
    }

    public void ConquerForAll()
    {
        //Blue
        if(playerFaction != Factions.Circle)
        {
            Tilemap[] tilemaps = new Tilemap[3] {green, red, yellow};
            Conquer(Factions.Circle, Relationships.Instance.circleRelation, tilemaps, blue);
        }

        //Red
        if(playerFaction != Factions.Rectangle)
        {
            Tilemap[] tilemaps = new Tilemap[3] {blue, green, yellow};
            Conquer(Factions.Rectangle, Relationships.Instance.rectangleRelation, tilemaps, red);
        }

        //Yellow
        if(playerFaction != Factions.Triangle)
        {
            Tilemap[] tilemaps = new Tilemap[3] {blue, red, green};
            Conquer(Factions.Triangle, Relationships.Instance.triangleRelation, tilemaps, yellow);
        }

        //Green
        if(playerFaction != Factions.Square)
        {
            Tilemap[] tilemaps = new Tilemap[3] {blue, red, yellow};
            Conquer(Factions.Square, Relationships.Instance.squareRelation, tilemaps, green);
        }
    }


    List<(Vector3Int pos, TileBase tile)> GetAdjacentFactionTiles(Vector3Int center, Tilemap[] factionMaps)
    {
        Vector3Int[] directions = {
            Vector3Int.up, Vector3Int.down,
            Vector3Int.left, Vector3Int.right,
            new Vector3Int(1, 1, 0), new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, -1, 0), new Vector3Int(1, -1, 0)
        };

        List<(Vector3Int pos, TileBase tile)> result = new List<(Vector3Int, TileBase)>();

        foreach(var dir in directions)
        {
            Vector3Int checkPos = center + dir;

            foreach (var map in factionMaps)
            {
                var tile = map.GetTile(checkPos);
                if (tile != null)
                {
                    result.Add((checkPos, tile));
                    break;
                }
            }
        }

        return result;
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


}

