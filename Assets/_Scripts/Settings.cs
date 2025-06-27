using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Settings : MonoBehaviour
{
    public int money;
    public TextMeshProUGUI moneyVisual;

    public float sellPercentage = 0.7f;

    void Start()
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        moneyVisual.text = "$" + money.ToString();
    }
}