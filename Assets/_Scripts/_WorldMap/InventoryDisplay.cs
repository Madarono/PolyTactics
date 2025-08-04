using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : MonoBehaviour
{
    public TowerSlotSO slot;
    public int slotIndex;
    public Image iconImage;
    public Image image;

    public void SelectTurrent()
    { 
        InteractionSystem.Instance.RemoveCurrentTowers(slot, gameObject, slotIndex);
    }
}