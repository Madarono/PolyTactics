using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("Window")]
    public SoundManager sound;
    public TowerManager towerManager;
    public Settings settings;
    public TowerUpgrade tower;
    public GameObject window;
    public Transform[] windowStatesRight;
    public Transform[] windowStatesLeft;
    public float stayDistance = 0.1f;
    public PositionChange[] positionChange;
    private Transform[] windowStates; //Current one
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
    public TextMeshProUGUI slowVisual;
    public TextMeshProUGUI freezeVisual;
    public TextMeshProUGUI criticalVisual;
    public TextMeshProUGUI splashVisual;
    public TextMeshProUGUI passiveVisual;
    public TextMeshProUGUI decayVisual;

    public TextMeshProUGUI targettingVisual;
    public TextMeshProUGUI priceVisual;
    public TextMeshProUGUI sellVisual;


    [Header("GameObejcts")]
    public GameObject speedObj;
    public GameObject reloadObj;
    public GameObject rotationObj;
    public GameObject rangeObj;
    public GameObject damageObj;
    public GameObject slowObj;
    public GameObject freezeObj;
    public GameObject criticalObj;
    public GameObject splashObj;
    public GameObject passiveObj;
    public GameObject decayObj;

    public GameObject targettingObj;
    public GameObject freezerObj;
    


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
        windowStates = windowStatesLeft;
        window.transform.position = windowStates[0].position;
        foreach(PositionChange script in positionChange)
        {
            script.MovePlaces(0);
        }

        finalPrice = 0;
        priceVisual.text = "$" + finalPrice.ToString();
        HideLevelRemovers();
    }

    void FixedUpdate()
    {
        if(tower != null)
        {
            window.transform.position = Vector3.Lerp(window.transform.position, windowStates[1].position, Time.unscaledDeltaTime * speedOfWindow);
        }
        else
        {
            window.transform.position = Vector3.Lerp(window.transform.position, windowStates[0].position, Time.unscaledDeltaTime * speedOfWindow);
        }
    }

    public void UpdateWindowPositioning()
    {
        float distance = Vector2.Distance(window.transform.position, windowStates[1].position);
        if(distance <= stayDistance)
        {
            return;
        }

        if(tower.transform.position.x <= 0)
        {
            windowStates = windowStatesRight;
            foreach(PositionChange script in positionChange)
            {
                script.MovePlaces(0);
            }
        }
        else
        {
            windowStates = windowStatesLeft;
            foreach(PositionChange script in positionChange)
            {
                script.MovePlaces(1);
            }
        }


        window.transform.position = windowStates[0].position;
    }

    public void UpdateValues()
    {
        Targetting[] types = (Targetting[])System.Enum.GetValues(typeof(Targetting));
        int currentIndex = System.Array.IndexOf(types, tower.tower.targetting);
        targettingVisual.text = types[currentIndex].ToString();

        headerVisual.text = tower.tower.towerName;
        if(tower.upgrades[0].bulletDamage > 0)
        {
            damageVisual.text = tower.upgrades[tower.individualLv[0]].bulletDamage.ToString("F1");
            damageObj.SetActive(true);
        }
        else
        {
            damageObj.SetActive(false);
        }
        
        if(tower.upgrades[0].pierce > 0)
        {
            speedObj.SetActive(true);
            speedVisual.text = tower.upgrades[tower.individualLv[1]].pierce.ToString("F1");
        }
        else
        {
            speedObj.SetActive(false);
        }

        if(tower.upgrades[0].reloadTime > 0)
        {
            reloadObj.SetActive(true);
            reloadVisual.text = tower.upgrades[tower.individualLv[2]].reloadTime.ToString("F1");
        }
        else
        {
            reloadObj.SetActive(false);
        }

        if(tower.upgrades[0].rotationSpeed > 0)
        {
            rotationObj.SetActive(true);
            rotationVisual.text = tower.upgrades[tower.individualLv[3]].rotationSpeed.ToString("F1");
        }
        else
        {
            rotationObj.SetActive(false);
        }

        if(tower.upgrades[0].range > 0)
        {
            rangeObj.SetActive(true);
            rangeVisual.text = tower.upgrades[tower.individualLv[4]].range.ToString("F1");
        }
        else
        {
            rangeObj.SetActive(false);
        }

        if(tower.upgrades[0].slowPercentage > 0)
        {
            slowObj.SetActive(true);
            slowVisual.text = tower.upgrades[tower.individualLv[5]].slowPercentage.ToString("F1");
        }
        else
        {
            slowObj.SetActive(false);
        }

        if(tower.upgrades[0].freezeChance > 0)
        {
            freezeObj.SetActive(true);
            freezeVisual.text = tower.upgrades[tower.individualLv[6]].freezeChance.ToString("F1");
        }
        else
        {
            freezeObj.SetActive(false);
        }

        if(tower.upgrades[0].criticalChance > 0)
        {
            criticalObj.SetActive(true);
            criticalVisual.text = tower.upgrades[tower.individualLv[7]].criticalChance.ToString("F1");
        }
        else
        {
            criticalObj.SetActive(false);
        }
        if(tower.upgrades[0].splashRadius > 0)
        {
            splashObj.SetActive(true);
            splashVisual.text = tower.upgrades[tower.individualLv[8]].splashRadius.ToString("F1");
        }
        else
        {
            splashObj.SetActive(false);
        }
        if(tower.upgrades[0].passiveDmg > 0)
        {
            passiveObj.SetActive(true);
            passiveVisual.text = tower.upgrades[tower.individualLv[9]].passiveDmg.ToString("F2");
        }
        else
        {
            passiveObj.SetActive(false);
        }
        if(tower.upgrades[0].immunityDecay > 0)
        {
            decayObj.SetActive(true);
            decayVisual.text = tower.upgrades[tower.individualLv[10]].immunityDecay.ToString("F1");
        }
        else
        {
            decayObj.SetActive(false);
        }




        if(tower.provideTargetting)
        {
            targettingObj.SetActive(true);
            freezerObj.SetActive(false);
        }
        else
        {
            targettingObj.SetActive(false);
            freezerObj.SetActive(true);
        }

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
            speedVisual.text =  tower.upgrades[tower.individualLv[1]].pierce.ToString("F1") + " -> " + tower.upgrades[levels[1]].pierce.ToString("F1");
            int difference = levels[1] - tower.individualLv[1];

            prices[1] = 0;
            for(int i = tower.individualLv[1] + 1; i <= levels[1]; i++)
            {
                prices[1] += tower.upgrades[i].piercePrice;
            } 
        }
        else
        {
            prices[1] = 0;
            speedVisual.text = tower.upgrades[tower.individualLv[1]].pierce.ToString("F1");
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

        if(levels[5] > tower.individualLv[5])
        {
            slowVisual.text =  tower.upgrades[tower.individualLv[5]].slowPercentage.ToString("F1") + " -> " + tower.upgrades[levels[5]].slowPercentage.ToString("F1");
            int difference = levels[5] - tower.individualLv[5];

            prices[5] = 0;
            for(int i = tower.individualLv[5] + 1; i <= levels[5]; i++)
            {
                prices[5] += tower.upgrades[i].slowPrice;
            } 
        }
        else
        {
            prices[5] = 0;
            slowVisual.text = tower.upgrades[tower.individualLv[5]].slowPercentage.ToString("F1");
        }
        
        if(levels[6] > tower.individualLv[6])
        {
            freezeVisual.text =  tower.upgrades[tower.individualLv[6]].freezeChance.ToString("F1") + " -> " + tower.upgrades[levels[6]].freezeChance.ToString("F1");
            int difference = levels[6] - tower.individualLv[6];

            prices[6] = 0;
            for(int i = tower.individualLv[6] + 1; i <= levels[6]; i++)
            {
                prices[6] += tower.upgrades[i].freezePrice;
            } 
        }
        else
        {
            prices[6] = 0;
            freezeVisual.text = tower.upgrades[tower.individualLv[6]].freezeChance.ToString("F1");
        }

        if(levels[7] > tower.individualLv[7])
        {
            criticalVisual.text =  tower.upgrades[tower.individualLv[7]].criticalChance.ToString("F1") + " -> " + tower.upgrades[levels[7]].criticalChance.ToString("F1");
            int difference = levels[7] - tower.individualLv[7];

            prices[7] = 0;
            for(int i = tower.individualLv[7] + 1; i <= levels[7]; i++)
            {
                prices[7] += tower.upgrades[i].criticalPrice;
            } 
        }
        else
        {
            prices[7] = 0;
            criticalVisual.text = tower.upgrades[tower.individualLv[7]].criticalChance.ToString("F1");
        }

        if(levels[8] > tower.individualLv[8])
        {
            splashVisual.text =  tower.upgrades[tower.individualLv[8]].splashRadius.ToString("F1") + " -> " + tower.upgrades[levels[8]].splashRadius.ToString("F1");
            int difference = levels[8] - tower.individualLv[8];

            prices[8] = 0;
            for(int i = tower.individualLv[8] + 1; i <= levels[8]; i++)
            {
                prices[8] += tower.upgrades[i].splashPrice;
            } 
        }
        else
        {
            prices[8] = 0;
            splashVisual.text = tower.upgrades[tower.individualLv[8]].splashRadius.ToString("F1");
        }

        if(levels[9] > tower.individualLv[9])
        {
            passiveVisual.text =  tower.upgrades[tower.individualLv[9]].passiveDmg.ToString("F2") + " -> " + tower.upgrades[levels[9]].passiveDmg.ToString("F2");
            int difference = levels[9] - tower.individualLv[9];

            prices[9] = 0;
            for(int i = tower.individualLv[9] + 1; i <= levels[9]; i++)
            {
                prices[9] += tower.upgrades[i].passivePrice;
            } 
        }
        else
        {
            prices[9] = 0;
            passiveVisual.text = tower.upgrades[tower.individualLv[9]].passiveDmg.ToString("F2");
        }

        if(levels[10] > tower.individualLv[10])
        {
            decayVisual.text =  tower.upgrades[tower.individualLv[10]].immunityDecay.ToString("F1") + " -> " + tower.upgrades[levels[10]].immunityDecay.ToString("F1");
            int difference = levels[10] - tower.individualLv[10];

            prices[10] = 0;
            for(int i = tower.individualLv[10] + 1; i <= levels[10]; i++)
            {
                prices[10] += tower.upgrades[i].immunityPrice;
            } 
        }
        else
        {
            prices[10] = 0;
            decayVisual.text = tower.upgrades[tower.individualLv[10]].immunityDecay.ToString("F1");
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
        if(levels[index] == tower.individualLv[index])
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
            tower.sellValue += Mathf.FloorToInt(finalPrice * settings.sellPercentage);
            UpdateValues();
            tower.ApplyTower();
            sound.PlayClip(sound.confirmUpgrade, 1f);
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
