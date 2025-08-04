using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SendSlotInfo : MonoBehaviour, IDataPersistence
{
    public TowerSlotSO[] towers;
    public int[] slotIndex;

    public void LoadData(GameData data)
    {
        this.slotIndex = data.slotIndex;
        LoadToSlot();
    }

    public void SaveData(GameData data)
    {
    }

    void LoadToSlot()
    {
        List<TowerSlotSO> slots = new List<TowerSlotSO>();

        foreach(int index in slotIndex)
        {
            slots.Add(towers[index]);
        }

        SlotsManager.Instance.currentSlots = slots.ToArray();
        SlotsManager.Instance.InitiateStart();
    }
}