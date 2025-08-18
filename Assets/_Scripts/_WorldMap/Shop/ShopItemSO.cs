using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopItem", menuName = "Custom/Shop_Item", order = 1)]
public class ShopItemSO : ScriptableObject
{
    public TowerSlotSO slot;
    public Sprite icon;
    public string name;
    public int price;
}
