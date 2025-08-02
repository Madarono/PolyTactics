using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerSlotInventory : MonoBehaviour
{
    public TowerSlotSO slot;
    
    public Image iconImage;

    [Header("Faction Color")] 
    public ColorFactions[] colorFactions;
    public Image image;

    public void Refresh()
    {
        if(slot.faction == Factions.Universal)
        {
            for(int i = 0; i < colorFactions.Length; i++)
            {
                if(InteractionSystem.Instance.playerFaction == colorFactions[i].faction)
                {
                    image.color = colorFactions[i].color;
                    break;
                }
            }
        }

        for(int i = 0; i < colorFactions.Length; i++)
        {
            if(slot.faction == colorFactions[i].faction)
            {
                image.color = colorFactions[i].color;
                break;
            }
        }

        iconImage.sprite = slot.icon;
    }

    public void SelectTurrent()
    { 
        InteractionSystem.Instance.AddToCurrentTowers(slot, image.color);
    }
}