using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("Window")]
    public TowerManager towerManager;
    public Settings settings;
    public TowerUpgrade tower;
    public GameObject window;
    public Transform[] windowStates;
    public float speedOfWindow;
    
    [Header("Visuals")]
    public GameObject upgradeButtons;
    public GameObject sell;
    public TextMeshProUGUI headerVisual;
    public TextMeshProUGUI damageVisual;
    public TextMeshProUGUI speedVisual;
    public TextMeshProUGUI reloadVisual;
    public TextMeshProUGUI rotationVisual;
    public TextMeshProUGUI rangeVisual;
    public TextMeshProUGUI priceVisual;
    public TextMeshProUGUI sellVisual;
    public TextMeshProUGUI targettingVisual;

    [Header("Faction colors")]
    public ColorFactions[] factionColor;
    public TextMeshProUGUI[] textsToChange;
    public Image whiteSpace;
    public VisualFaction[] iconVisuals;

    [Header("Upgrading")]
    public int[] levels;
    public GameObject[] decreaseButton;
    public GameObject[] increaseButton;
    public int[] prices;
    private int finalPrice;

    void Start()
    {
        finalPrice = 0;
        priceVisual.text = "$" + finalPrice.ToString();
        HideLevelRemovers();
    }

    void FixedUpdate()
    {
        if(tower != null)
        {
            window.transform.position = Vector3.Lerp(window.transform.position, windowStates[1].position, Time.deltaTime * speedOfWindow);
        }
        else
        {
            window.transform.position = Vector3.Lerp(window.transform.position, windowStates[0].position, Time.deltaTime * speedOfWindow);
        }
    }

    public void UpdateValues()
    {
        Targetting[] types = (Targetting[])System.Enum.GetValues(typeof(Targetting));
        int currentIndex = System.Array.IndexOf(types, tower.tower.targetting);
        targettingVisual.text = types[currentIndex].ToString();

        headerVisual.text = tower.tower.towerName;
        damageVisual.text = tower.upgrades[tower.individualLv[0]].bulletDamage.ToString("F1");
        speedVisual.text = tower.upgrades[tower.individualLv[1]].fireForce.ToString("F1");
        reloadVisual.text = tower.upgrades[tower.individualLv[2]].reloadTime.ToString("F1");
        rotationVisual.text = tower.upgrades[tower.individualLv[3]].rotationSpeed.ToString("F1");
        rangeVisual.text = tower.upgrades[tower.individualLv[4]].range.ToString("F1");
        sellVisual.text = "Sell - $" + tower.sellValue.ToString();



        for(int i = 0; i < factionColor.Length; i++)
        {
            if(tower.tower.faction == factionColor[i].faction)
            {
                foreach(TextMeshProUGUI txt in textsToChange)
                {
                    txt.color = factionColor[i].color;
                }
                foreach(VisualFaction script in iconVisuals)
                {
                    script.ChangeSprite(i);
                }
                whiteSpace.color = factionColor[i].color;
                break;
            }
        }

        HideLevelRemovers();

        levels = (int[])tower.individualLv.Clone();
        prices = new int[tower.individualLv.Length];

        for(int i = 0; i < levels.Length; i++)
        {
            if(levels[i] == tower.upgrades.Length - 1)
            {
                increaseButton[i].SetActive(false);
                continue;
            }
            increaseButton[i].SetActive(true);
        }

        finalPrice = 0;
        priceVisual.text = "$" + finalPrice.ToString();
        upgradeButtons.SetActive(false);
        sell.SetActive(true);
    }

    public void HideLevelRemovers()
    {
        foreach(GameObject obj in decreaseButton)
        {
            obj.SetActive(false);
        }
    }

    public void ShowUpgrades()
    {
        if(levels[0] > tower.individualLv[0])
        {
            damageVisual.text =  tower.upgrades[tower.individualLv[0]].bulletDamage.ToString("F1") + " -> " + tower.upgrades[levels[0]].bulletDamage.ToString("F1");
            prices[0] = 0;
            for(int i = tower.individualLv[0] + 1; i <= levels[0]; i++)
            {
                prices[0] += tower.upgrades[i].bulletPrice;
            } 
        }
        else
        {
            prices[0] = 0;
            damageVisual.text = tower.upgrades[tower.individualLv[0]].bulletDamage.ToString("F1");
        }

        if(levels[1] > tower.individualLv[1])
        {
            speedVisual.text =  tower.upgrades[tower.individualLv[1]].fireForce.ToString("F1") + " -> " + tower.upgrades[levels[1]].fireForce.ToString("F1");
            int difference = levels[1] - tower.individualLv[1];

            prices[1] = 0;
            for(int i = tower.individualLv[1] + 1; i <= levels[1]; i++)
            {
                prices[1] += tower.upgrades[i].firePrice;
            } 
        }
        else
        {
            prices[1] = 0;
            speedVisual.text = tower.upgrades[tower.individualLv[1]].fireForce.ToString("F1");
        }

        if(levels[2] > tower.individualLv[2])
        {
            reloadVisual.text =  tower.upgrades[tower.individualLv[2]].reloadTime.ToString("F1") + " -> " + tower.upgrades[levels[2]].reloadTime.ToString("F1");
            int difference = levels[2] - tower.individualLv[2];

            prices[2] = 0;
            for(int i = tower.individualLv[2] + 1; i <= levels[2]; i++)
            {
                prices[2] += tower.upgrades[i].reloadPrice;
            } 
        }
        else
        {
            prices[2] = 0;
            reloadVisual.text = tower.upgrades[tower.individualLv[2]].reloadTime.ToString("F1");
        }

        if(levels[3] > tower.individualLv[3])
        {
            rotationVisual.text =  tower.upgrades[tower.individualLv[3]].rotationSpeed.ToString("F1") + " -> " + tower.upgrades[levels[3]].rotationSpeed.ToString("F1");
            int difference = levels[3] - tower.individualLv[3];

            prices[3] = 0;
            for(int i = tower.individualLv[3] + 1; i <= levels[3]; i++)
            {
                prices[3] += tower.upgrades[i].rotationPrice;
            } 
        }
        else
        {
            prices[3] = 0;
            rotationVisual.text = tower.upgrades[tower.individualLv[3]].rotationSpeed.ToString("F1");
        }

        if(levels[4] > tower.individualLv[4])
        {
            rangeVisual.text =  tower.upgrades[tower.individualLv[4]].range.ToString("F1") + " -> " + tower.upgrades[levels[4]].range.ToString("F1");
            int difference = levels[4] - tower.individualLv[4];

            prices[4] = 0;
            for(int i = tower.individualLv[4] + 1; i <= levels[4]; i++)
            {
                prices[4] += tower.upgrades[i].rangePrice;
            } 
        }
        else
        {
            prices[4] = 0;
            rangeVisual.text = tower.upgrades[tower.individualLv[4]].range.ToString("F1");
        }

        finalPrice = 0;
        foreach(int price in prices)
        {
            finalPrice += price;
        }

        priceVisual.text = "$" + finalPrice.ToString();
    }

    public void SelectUpgrade(int index)
    {
        if(levels[index] + 1 == tower.upgrades.Length)
        {
            return;
        }
        levels[index]++;
        if(levels[index] == tower.upgrades.Length - 1)
        {
            increaseButton[index].SetActive(false);
        }
        decreaseButton[index].SetActive(true);
        upgradeButtons.SetActive(true);
        sell.SetActive(false);
        ShowUpgrades();
    }

    public void DelectUpgrade(int index)
    {
        if(levels[index] == tower.individualLv[index])
        {
            return;
        }

        levels[index]--;
        increaseButton[index].SetActive(true);
        if(levels[index] == 0)
        {
            decreaseButton[index].SetActive(false);
        }

        ShowUpgrades();
    }

    public void Decline()
    {
        UpdateValues();
    }

    public void Confirm()
    {
        if(settings.money >= finalPrice)
        {
            settings.money -= finalPrice;
            settings.UpdateVisual();
            for(int i = 0; i < levels.Length; i++)
            {
                tower.individualLv[i] = levels[i];
            }
            // tower.sellValue += (int)Mathf.Round(finalPrice / settings.sellPercentage);
            tower.sellValue += Mathf.FloorToInt(finalPrice * settings.sellPercentage);
            UpdateValues();
            tower.ApplyTower();
        }
    }

    public void Sell()
    {
        if(tower == null)
        {
            return;
        }

        Vector3 towerPos = tower.gameObject.transform.position;
        Vector3Int cellPos = towerManager.tilemap.WorldToCell(towerPos);
        int tileIndex = towerManager.tilePositions.IndexOf(cellPos);
        towerManager.isFull[tileIndex] = false;
        settings.money += tower.sellValue;
        settings.UpdateVisual();
        Destroy(tower.gameObject);
    }

    public void NextTargetting()
    {
        Targetting[] types = (Targetting[])System.Enum.GetValues(typeof(Targetting));
        int currentIndex = System.Array.IndexOf(types, tower.tower.targetting);
        int nextIndex = (currentIndex + 1) % types.Length;
        targettingVisual.text = types[nextIndex].ToString();
        tower.tower.targetting = types[nextIndex];
    }

    public void LastTargetting()
    {
        Targetting[] types = (Targetting[])System.Enum.GetValues(typeof(Targetting));
        int currentIndex = System.Array.IndexOf(types, tower.tower.targetting);
        int lastIndex = (currentIndex - 1 + types.Length) % types.Length;
        targettingVisual.text = types[lastIndex].ToString();
        tower.tower.targetting = types[lastIndex];
    }
}
