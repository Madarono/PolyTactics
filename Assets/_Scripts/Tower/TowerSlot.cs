using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ColorFactions
{
    public Factions faction;
    public Color color;
}

public class TowerSlot : MonoBehaviour
{
    public TowerManager manager;
    public int towerIndex;
    public bool isSelected;

    public Image iconImage;

    [Header("Faction Color")] 
    public ColorFactions[] colorFactions;
    public Factions faction;
    public Image image;

    [Header("Modificiations")]
    public bool isTrap;
    public bool isUniversal;

    [Header("Limits")]
    public int limit;
    public bool isFarm;

    void Start()
    {
        if(faction == Factions.Universal)
        {
            for(int i = 0; i < colorFactions.Length; i++)
            {
                if(Settings.Instance.playerFaction == colorFactions[i].faction)
                {
                    image.color = colorFactions[i].color;
                    break;
                }
            }
            isUniversal = true;
        }
        else
        {
            isUniversal = false;
        }

        for(int i = 0; i < colorFactions.Length; i++)
        {
            if(faction == colorFactions[i].faction)
            {
                image.color = colorFactions[i].color;
                break;
            }
        }
    }

    public void SelectTurrent()
    { 
        if(isFarm && manager.farm.Count >= limit)
        {
            return;
        }

        if(isSelected && manager.currentSlot == this)
        {
            manager.InactiveSelection();
            isSelected = false;
        }
        else
        {
            manager.UnlockSelection(towerIndex, this, isTrap);
            isSelected = true;
        }
    }
}
