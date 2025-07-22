using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class Grass
{
    public TileBase[] grassVarients;
    public TileBase grassTileTypeToReplace;
    public float chance;
}

public class GrassRandomizer : MonoBehaviour
{
    public static GrassRandomizer Instance {get; private set;}
    
    public Tilemap tilemap;
    public Grass[] grass;

    void Awake()
    {
        Instance = this;
    }

    public void RandomizeGrass()
    {
        BoundsInt bounds = tilemap.cellBounds;
        for(int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for(int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x,y,0);
                TileBase currentTile = tilemap.GetTile(pos);
                float random = Random.Range(0, 100);
                for(int i = 0; i < grass.Length; i++)
                {
                    if(currentTile == grass[i].grassTileTypeToReplace && random <= grass[i].chance)
                    {
                        TileBase newTile = grass[i].grassVarients[Random.Range(0, grass[i].grassVarients.Length)];
                        tilemap.SetTile(pos, newTile);
                    }
                }
            }
        }
    }
}
