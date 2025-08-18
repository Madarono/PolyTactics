using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    private SoundManager sound;
    public TowerSlotSO slot;
    public Image icon;
    public TextMeshProUGUI name;
    public TextMeshProUGUI priceVisual;
    public Button interactButton;
    public TextMeshProUGUI buttonVisual;
    public int price;

    void Start()
    {
        sound = SoundManager.Instance;
    }

    public void Refresh(int price, string name, Sprite icon, TowerSlotSO slot)
    {
        this.price = price;
        this.priceVisual.text = price.ToString();
        this.name.text = name;
        this.icon.sprite = icon;
        this.slot = slot;
    }

    public void Inactive()
    {
        interactButton.interactable = false;
        buttonVisual.text = "Bought";
    }

    public void Active()
    {
        interactButton.interactable = true;
        buttonVisual.text = "Buy";
    }

    public void Buy()
    {
        if(ResourcesStorage.Instance.coins >= price)
        {
            ResourcesStorage.Instance.coins -= price;
            ShopSystem.Instance.Buy(slot, this);
            sound.PlayClip(sound.confirmUpgrade, 1f);
        }
        else
        {
            int randomIndex = Random.Range(0, sound.buttonClicks.Length);
            sound.PlayClip(sound.buttonClicks[randomIndex], 1f);
        }
    }
}
