using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings : MonoBehaviour
{
    public int money;
    public TextMeshProUGUI moneyVisual;

    void Start()
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        moneyVisual.SetText("${0}", money);
    }
}