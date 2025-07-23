using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerSlot", menuName = "Custom/Slot", order = 1)]
public class TowerSlotSO : ScriptableObject
{
    public Factions faction;
    public Sprite icon;
    public int towerIndex;

    [Header("Modifications")]
    public bool isTrap;

    [Header("Limits")]
    public int limit;
    public bool isFarm;
}
