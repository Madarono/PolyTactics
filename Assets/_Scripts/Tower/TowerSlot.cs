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

    [Header("Faction Color")] 
    public ColorFactions[] colorFactions;
    public Factions faction;
    public Image image;

    void Start()
    {
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
        if(isSelected && manager.currentSlot == this)
        {
            manager.InactiveSelection();
            isSelected = false;
        }
        else
        {
            manager.UnlockSelection(towerIndex, this);
            isSelected = true;
        }
    }
}
