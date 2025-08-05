using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/FactionTile")]
public class FactionTile : Tile
{
    public Factions factionOwner;
    public int tileID; // optional: for ID-based checks
}
