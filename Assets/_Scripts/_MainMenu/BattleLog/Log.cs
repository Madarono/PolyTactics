using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Log : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI duration;
    public TextMeshProUGUI endState;

    public void Refresh(int battleNumber, string faction, string factionColor, int dayNumber, string endTalk, Sprite iconSprite)
    {
        title.text = $"<b>Battle {battleNumber}:</b> <color={factionColor}>{faction}</color>";
        duration.text = $"<b>Duration:</b> {dayNumber} Days";
        endState.text = $"<b>End State:</b> {endTalk}";
        icon.sprite = iconSprite;
    }

    
}
