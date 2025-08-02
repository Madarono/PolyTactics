using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum WorldFactionPosition
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

[System.Serializable]
public class FactionWorldMap
{
    public WorldFactionPosition factionPos;
    public TileBase tile;
    public Tilemap tileMap;
}

public class PerlinNoise : MonoBehaviour
{
    public static PerlinNoise Instance {get; private set;}
    public CameraPinch pinch;

    public int width = 128;
    public int height = 128;

    public float scale = 5f;
    public float threshold = 0.6f;

    public float offsetX;
    public float offsetY;

    public Tilemap ground;
    public Tilemap groundShadow;
    public Tilemap water;
    public Tilemap border;
    public TileBase waterTile;
    public TileBase groundTile;
    public TileBase shadowTile;
    public TileBase whiteTile;

    [Header("Factions")]
    public FactionWorldMap[] factions; 
    public float falloffStrength = 10f;

    [Header("Seed")]
    public int seed;
    public bool useSeed = true;
    
    public bool randomSeed = false;
    public int minSeed = 0;
    public int maxSeed = 999999;

    [Header("Camera")]
    public Camera cam;

    void Awake()
    {
        Instance = this;
    }

    public void InstantiateStart()
    {        
        if(!useSeed)
        {
            GenerateLand();
            return;
        }

        float finalSeed = randomSeed ? Random.Range(minSeed, maxSeed) : seed;
        seed = (int)finalSeed;

        offsetX = Mathf.PerlinNoise(finalSeed * 0.1f, 0f) * 100f;
        offsetY = Mathf.PerlinNoise(0f, finalSeed * 0.1f) * 100f;
        GenerateLand();
    }

    void GenerateLand()
    {
        HashSet<WorldFactionPosition> assignedFactions = new HashSet<WorldFactionPosition>();

        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int landIndex = CalculateNoise(x, y);
                Vector3Int pos = new Vector3Int(x, y, 0);


                if (landIndex == 0) 
                {
                    continue;
                }

                TileBase tile = groundTile;

                Vector2 center = new Vector2(width / 2f, height / 2f);
                Vector2 current = new Vector2(x, y);

                float angle = Mathf.Atan2(current.y - center.y, current.x - center.x) * Mathf.Rad2Deg;
                angle = (angle + 360f) % 360f;

                WorldFactionPosition selectedFaction;

                if (angle >= 45f && angle < 135f)
                {
                    selectedFaction = WorldFactionPosition.TopRight;
                }
                else if (angle >= 135f && angle < 225f)
                {
                    selectedFaction = WorldFactionPosition.BottomRight;
                }
                else if (angle >= 225f && angle < 315f)
                {
                    selectedFaction = WorldFactionPosition.BottomLeft;
                }
                else
                {
                    selectedFaction = WorldFactionPosition.TopLeft;
                }

                Tilemap map = ground;
                float noise = Mathf.PerlinNoise((offsetX + x) * 0.1f, (offsetY + y) * 0.1f);
                if(noise > 0.5f)
                {
                    for(int i = 0; i < factions.Length; i++)
                    {
                        if (factions[i].factionPos == selectedFaction)
                        {
                            tile = factions[i].tile;
                            map = factions[i].tileMap;
                            assignedFactions.Add(selectedFaction);
                            break;
                        }
                    }
                }

                map.SetTile(pos, tile);
                ground.SetTile(pos, groundTile);
                groundShadow.SetTile(pos, shadowTile);
            }
        }

        for(int x = -2; x < width + 2; x++)
        {
            for(int y = -2; y < height + 2; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                water.SetTile(pos, waterTile);
            }
        }

        for(int x = -3; x < width + 3; x++)
        {
            for(int y = -3; y < height + 3; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                border.SetTile(pos, whiteTile);
            }
        }



        foreach(FactionWorldMap faction in factions)
        {
            if (!assignedFactions.Contains(faction.factionPos))
            {
                Vector3Int forcePos = GetCenterOfFaction(faction.factionPos);
                ground.SetTile(forcePos, faction.tile);
            }
        }

        Vector3Int middlePos = new Vector3Int(width / 2, height / 2, -10);
        cam.transform.position = middlePos;
        pinch.islandPlace = new Vector3(width / 2, height / 2, -10);
        cam.orthographicSize = width * 0.9375f;
    }

    Vector3Int GetCenterOfFaction(WorldFactionPosition pos)
    {
        int x = 0, y = 0;
        switch(pos)
        {
            case WorldFactionPosition.TopLeft:
                x = width / 4; y = 3 * height / 4; 
                break;

            case WorldFactionPosition.TopRight:
                x = 3 * width / 4; y = 3 * height / 4; 
                break;

            case WorldFactionPosition.BottomLeft:
                x = width / 4; y = height / 4; 
                break;

            case WorldFactionPosition.BottomRight:
                x = 3 * width / 4; y = height / 4; 
                break;
        }
        return new Vector3Int(x, y, 0);
    }



    int CalculateNoise(int x, int y)
    {
        float xCoord = offsetX + (float)x / width * scale;
        float yCoord = offsetY + (float)y / height * scale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        return sample > threshold ? 1 : 0;
    }
}
