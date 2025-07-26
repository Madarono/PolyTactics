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
    public GameObject debugRange;
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
    
    public TextMeshProUGUI incomeVisual;
    public TextMeshProUGUI clusterVisual;
    public TextMeshProUGUI waveVisual;
    public TextMeshProUGUI refundVisual;

    public TextMeshProUGUI _rangeVisual;
    public TextMeshProUGUI _damageVisual;
    public TextMeshProUGUI _reloadVisual;
    public TextMeshProUGUI _pierceVisual;

    public TextMeshProUGUI powerVisual;



    public TextMeshProUGUI targettingVisual;
    public TextMeshProUGUI priceVisual;
    public TextMeshProUGUI sellVisual;


    [Header("GameObjects")]
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

    public GameObject incomeObj;
    public GameObject clusterObj;
    public GameObject waveObj;
    public GameObject refundObj;

    public GameObject _rangeObj;
    public GameObject _damageObj;
    public GameObject _reloadObj;
    public GameObject _pierceObj;

    public GameObject powerObj;

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
        if(tower.upgrades[0].incomeGenerated > 0)
        {
            incomeObj.SetActive(true);
            incomeVisual.text = tower.upgrades[tower.individualLv[11]].incomeGenerated.ToString("F1");
        }
        else
        {
            incomeObj.SetActive(false);
        }
        if(tower.upgrades[0].incomePercentage > 0)
        {
            clusterObj.SetActive(true);
            clusterVisual.text = tower.upgrades[tower.individualLv[12]].incomePercentage.ToString("F1");
        }
        else
        {
            clusterObj.SetActive(false);
        }
        if(tower.upgrades[0].wavePercentage > 0)
        {
            waveObj.SetActive(true);
            waveVisual.text = tower.upgrades[tower.individualLv[13]].wavePercentage.ToString("F1");
        }
        else
        {
            waveObj.SetActive(false);
        }
        if(tower.upgrades[0].refundPercentage > 0)
        {
            refundObj.SetActive(true);
            refundVisual.text = tower.upgrades[tower.individualLv[14]].refundPercentage.ToString("F1");
        }
        else
        {
            refundObj.SetActive(false);
        }
        if(tower.upgrades[0].rangeIncrease > 0)
        {
            _rangeObj.SetActive(true);
            _rangeVisual.text = tower.upgrades[tower.individualLv[15]].rangeIncrease.ToString("F1");
        }
        else
        {
            _rangeObj.SetActive(false);
        }
        if(tower.upgrades[0].damageIncrease > 0)
        {
            _damageObj.SetActive(true);
            _damageVisual.text = tower.upgrades[tower.individualLv[16]].damageIncrease.ToString("F1");
        }
        else
        {
            _damageObj.SetActive(false);
        }
        if(tower.upgrades[0].reloadDecrease > 0)
        {
            _reloadObj.SetActive(true);
            _reloadVisual.text = tower.upgrades[tower.individualLv[17]].reloadDecrease.ToString("F1");
        }
        else
        {
            _reloadObj.SetActive(false);
        }
        if(tower.upgrades[0].pierceIncrease > 0)
        {
            _pierceObj.SetActive(true);
            _pierceVisual.text = tower.upgrades[tower.individualLv[18]].pierceIncrease.ToString("F1");
        }
        else
        {
            _pierceObj.SetActive(false);
        }
        if(tower.upgrades[0].powerMultipler > 0)
        {
            powerObj.SetActive(true);
            powerVisual.text = tower.upgrades[tower.individualLv[19]].powerMultipler.ToString("F1");
        }
        else
        {
            powerObj.SetActive(false);
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
            debugRange.SetActive(true);
            debugRange.transform.position = tower.gameObject.transform.position;
            
            float increase = 0;
            if(tower.gameObject.TryGetComponent(out ExtraStats stats))
            {
                increase = stats.isUnderRange ? stats.rangeIncrease : 0;
            }
            float diameter = tower.upgrades[levels[4]].range * 2f * tower.gameObject.transform.localScale.x + (increase * 2f);
            debugRange.transform.localScale = new Vector3(diameter, diameter, debugRange.transform.localScale.z);
            int difference = levels[4] - tower.individualLv[4];

            prices[4] = 0;
            for(int i = tower.individualLv[4] + 1; i <= levels[4]; i++)
            {
                prices[4] += tower.upgrades[i].rangePrice;
            } 
        }
        else
        {
            debugRange.SetActive(false);
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

        if(levels[11] > tower.individualLv[11])
        {
            incomeVisual.text =  tower.upgrades[tower.individualLv[11]].incomeGenerated.ToString("F1") + " -> " + tower.upgrades[levels[11]].incomeGenerated.ToString("F1");
            int difference = levels[11] - tower.individualLv[11];

            prices[11] = 0;
            for(int i = tower.individualLv[11] + 1; i <= levels[11]; i++)
            {
                prices[11] += tower.upgrades[i].incomePrice;
            } 
        }
        else
        {
            prices[11] = 0;
            incomeVisual.text = tower.upgrades[tower.individualLv[11]].incomeGenerated.ToString("F1");
        }

        if(levels[12] > tower.individualLv[12])
        {
            clusterVisual.text =  tower.upgrades[tower.individualLv[12]].incomePercentage.ToString("F1") + " -> " + tower.upgrades[levels[12]].incomePercentage.ToString("F1");
            int difference = levels[12] - tower.individualLv[12];

            prices[12] = 0;
            for(int i = tower.individualLv[12] + 1; i <= levels[12]; i++)
            {
                prices[12] += tower.upgrades[i].clusterPrice;
            } 
        }
        else
        {
            prices[12] = 0;
            clusterVisual.text = tower.upgrades[tower.individualLv[12]].incomePercentage.ToString("F1");
        }

        if(levels[13] > tower.individualLv[13])
        {
            waveVisual.text =  tower.upgrades[tower.individualLv[13]].wavePercentage.ToString("F1") + " -> " + tower.upgrades[levels[13]].wavePercentage.ToString("F1");
            int difference = levels[13] - tower.individualLv[13];

            prices[13] = 0;
            for(int i = tower.individualLv[13] + 1; i <= levels[13]; i++)
            {
                prices[13] += tower.upgrades[i].wavePrice;
            } 
        }
        else
        {
            prices[13] = 0;
            waveVisual.text = tower.upgrades[tower.individualLv[13]].wavePercentage.ToString("F1");
        }

        if(levels[14] > tower.individualLv[14])
        {
            refundVisual.text =  tower.upgrades[tower.individualLv[14]].refundPercentage.ToString("F1") + " -> " + tower.upgrades[levels[14]].refundPercentage.ToString("F1");
            int difference = levels[14] - tower.individualLv[14];

            prices[14] = 0;
            for(int i = tower.individualLv[14] + 1; i <= levels[14]; i++)
            {
                prices[14] += tower.upgrades[i].refundPrice;
            } 
        }
        else
        {
            prices[14] = 0;
            refundVisual.text = tower.upgrades[tower.individualLv[14]].refundPercentage.ToString("F1");
        }

        if(levels[15] > tower.individualLv[15])
        {
            _rangeVisual.text =  tower.upgrades[tower.individualLv[15]].rangeIncrease.ToString("F1") + " -> " + tower.upgrades[levels[15]].rangeIncrease.ToString("F1");
            int difference = levels[15] - tower.individualLv[15];

            prices[15] = 0;
            for(int i = tower.individualLv[15] + 1; i <= levels[15]; i++)
            {
                prices[15] += tower.upgrades[i]._rangePrice;
            } 
        }
        else
        {
            prices[15] = 0;
            _rangeVisual.text = tower.upgrades[tower.individualLv[15]].rangeIncrease.ToString("F1");
        }

        if(levels[16] > tower.individualLv[16])
        {
            _damageVisual.text =  tower.upgrades[tower.individualLv[16]].damageIncrease.ToString("F1") + " -> " + tower.upgrades[levels[16]].damageIncrease.ToString("F1");
            int difference = levels[16] - tower.individualLv[16];

            prices[16] = 0;
            for(int i = tower.individualLv[16] + 1; i <= levels[16]; i++)
            {
                prices[16] += tower.upgrades[i]._damagePrice;
            } 
        }
        else
        {
            prices[16] = 0;
            _damageVisual.text = tower.upgrades[tower.individualLv[16]].damageIncrease.ToString("F1");
        }

        if(levels[17] > tower.individualLv[17])
        {
            _reloadVisual.text =  tower.upgrades[tower.individualLv[17]].reloadDecrease.ToString("F1") + " -> " + tower.upgrades[levels[17]].reloadDecrease.ToString("F1");
            int difference = levels[17] - tower.individualLv[17];

            prices[17] = 0;
            for(int i = tower.individualLv[17] + 1; i <= levels[17]; i++)
            {
                prices[17] += tower.upgrades[i]._reloadPrice;
            } 
        }
        else
        {
            prices[17] = 0;
            _reloadVisual.text = tower.upgrades[tower.individualLv[17]].reloadDecrease.ToString("F1");
        }

        if(levels[18] > tower.individualLv[18])
        {
            _pierceVisual.text =  tower.upgrades[tower.individualLv[18]].pierceIncrease.ToString("F1") + " -> " + tower.upgrades[levels[18]].pierceIncrease.ToString("F1");
            int difference = levels[18] - tower.individualLv[18];

            prices[18] = 0;
            for(int i = tower.individualLv[18] + 1; i <= levels[18]; i++)
            {
                prices[18] += tower.upgrades[i]._piercePrice;
            } 
        }
        else
        {
            prices[18] = 0;
            _pierceVisual.text = tower.upgrades[tower.individualLv[18]].pierceIncrease.ToString("F1");
        }

        if(levels[19] > tower.individualLv[19])
        {
            powerVisual.text =  tower.upgrades[tower.individualLv[19]].powerMultipler.ToString("F1") + " -> " + tower.upgrades[levels[19]].powerMultipler.ToString("F1");
            int difference = levels[19] - tower.individualLv[19];

            prices[19] = 0;
            for(int i = tower.individualLv[19] + 1; i <= levels[19]; i++)
            {
                prices[19] += tower.upgrades[i].powerPrice;
            } 
        }
        else
        {
            prices[19] = 0;
            powerVisual.text = tower.upgrades[tower.individualLv[18]].powerMultipler.ToString("F1");
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
        debugRange.SetActive(false);
        UpdateValues();
    }

    public void Confirm()
    {
        if(settings.money >= finalPrice)
        {
            if(tower.tower.towerType == TowerType.Farm)
            {
                tower.sellValue -= Mathf.FloorToInt(tower.baseValue * (tower.upgrades[tower.individualLv[14]].refundPercentage / 100f));
            }

            settings.money -= finalPrice;
            settings.UpdateVisual();
            for(int i = 0; i < levels.Length; i++)
            {
                tower.individualLv[i] = levels[i];
            }
            if(tower.tower.towerType == TowerType.Farm)
            {
                tower.sellValue += Mathf.FloorToInt(finalPrice * (tower.upgrades[tower.individualLv[14]].refundPercentage / 100f));
                tower.sellValue += Mathf.FloorToInt(tower.baseValue * (tower.upgrades[tower.individualLv[14]].refundPercentage / 100f));
            }
            else
            {
                tower.sellValue += Mathf.FloorToInt(finalPrice * settings.sellPercentage);
            }
            UpdateValues();
            debugRange.SetActive(false);
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
        switch(tower.tower.towerType) 
        {
            case TowerType.Farm:
                towerManager.farm.Remove(tower.tower);
                towerManager.tower.Remove(tower.tower);
                break;
                
            case TowerType.Village:
                tower.tower.DefaultTowers();
                towerManager.villages.Remove(tower.tower);
                towerManager.tower.Remove(tower.tower);
                break;

            default:
                towerManager.tower.Remove(tower.tower);
                break;
        }
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
