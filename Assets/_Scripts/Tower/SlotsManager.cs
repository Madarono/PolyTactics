using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsManager : MonoBehaviour
{
    public Settings settings;
    public bool followSettings;

    public TowerSlotSO[] currentSlots;

    [Header("Faction Stock Slots")]
    public TowerSlotSO[] circleSlots;
    public TowerSlotSO[] rectangleSlots;
    public TowerSlotSO[] squareSlots;
    public TowerSlotSO[] triangleSlots;

    [Header("Spawning of Slots")]
    public TowerManager manager;
    public GameObject slotPrefab;
    public Transform parent;

    public void Start()
    {
        if(followSettings)
        {
            switch(settings.playerFaction)
            {
                case Factions.Square:
                    currentSlots = (TowerSlotSO[])squareSlots.Clone();
                    break;

                case Factions.Triangle:
                    currentSlots = (TowerSlotSO[])triangleSlots.Clone();
                    break;

                case Factions.Rectangle:
                    currentSlots = (TowerSlotSO[])rectangleSlots.Clone();
                    break;

                case Factions.Circle:
                    currentSlots = (TowerSlotSO[])circleSlots.Clone();
                    break;
            }
        }

        if(currentSlots.Length == 0)
        {
            return;
        }

        if(parent.childCount > 0)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                Destroy(child.gameObject);
            }
        }

        foreach(TowerSlotSO slot in currentSlots)
        {
            GameObject go = Instantiate(slotPrefab, parent.transform.position, Quaternion.identity);
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            if(go.TryGetComponent(out TowerSlot goScript))
            {
                goScript.towerIndex = slot.towerIndex;
                goScript.iconImage.sprite = slot.icon;
                goScript.faction = slot.faction;
                goScript.isTrap = slot.isTrap;
                goScript.manager = manager; 
            } 
        }
    }
}
