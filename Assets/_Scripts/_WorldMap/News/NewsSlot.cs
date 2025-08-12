using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewsSlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI header;
    public TextMeshProUGUI info;

    public void Refresh(Sprite icon, string header, string info)
    {
        this.icon.sprite = icon;
        this.header.text = header;
        this.info.text = info;
    }
}
