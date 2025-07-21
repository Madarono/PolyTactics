using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManualDisplay : MonoBehaviour
{
    public ManualItem item;

    [Header("Visual")]
    public Image icon;
    public TextMeshProUGUI name;
    public TextMeshProUGUI info;

    public void ApplyItem()
    {
        icon.sprite = item.icon;
        name.text = item.name;
        info.text = item.info;
    }
}
