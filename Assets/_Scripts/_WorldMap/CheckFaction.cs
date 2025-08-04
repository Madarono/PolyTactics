using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckFaction : MonoBehaviour
{
    public static CheckFaction Instance {get; private set;}

    public ColorFactions[] factionColor;
    public Image[] imgToCheck; 

    void Awake()
    {
        Instance = this;
    }

    public void CheckVisuals(Factions faction)
    {
        for(int i = 0; i < factionColor.Length; i++)
        {
            if(faction == factionColor[i].faction)
            {
                foreach(Image img in imgToCheck)
                {
                    img.color = factionColor[i].color;
                }
                break;
            }
        }
    }
}
