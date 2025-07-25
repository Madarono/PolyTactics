using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrades
{
    [Header("Damage")]
    public float bulletDamage;
    public int bulletPrice;

    [Header("Pierce")]
    public int pierce;
    public int piercePrice;
    
    [Header("Reload time")]
    public float reloadTime;
    public int reloadPrice;
    
    [Header("Turning speed")]
    public float rotationSpeed;
    public int rotationPrice;

    [Header("Range")]
    public float range;
    public int rangePrice;


    [Header("Critical chance")]
    public float criticalChance;
    public int criticalPrice;

    [Header("Slow percentage")]
    public float slowPercentage;
    public int slowPrice;

    [Header("Freeze chance")]
    public float freezeChance;
    public int freezePrice;
    
    [Header("Splash Radius")]
    public float splashRadius;
    public int splashPrice;

    [Header("Passive Damage")]
    public float passiveDmg;
    public int passivePrice;

    [Header("Immunity Decay")]
    public float immunityDecay;
    public int immunityPrice;

    [Header("Farm - Income")]
    public int incomeGenerated;
    public int incomePrice; 

    [Header("Farm - Cluster")]
    public float incomePercentage;
    public int clusterPrice;

    [Header("Farm - Wave")]
    public float wavePercentage;
    public int wavePrice;

    [Header("Farm - Refund")]
    public float refundPercentage;
    public int refundPrice;

    [Header("Village - RangeIncrease")]
    public float rangeIncrease;
    public int _rangePrice;

    [Header("Village - DamageIncrease")]
    public float damageIncrease;
    public int _damagePrice;

    [Header("Village - reloadIncrease")]
    public float reloadDecrease;
    public int _reloadPrice;

    [Header("Village - pierceIncrease")]
    public float pierceIncrease;
    public int _piercePrice;

}

public class TowerUpgrade : MonoBehaviour
{
    public Tower tower;
    public ExtraStats extraStats;
    public Upgrades[] upgrades;
    public int sellValue;
    public int baseValue;
    public bool provideTargetting = true;

    [Tooltip("Every index is equal to the index in the upgrades showing individual level to each upgrade")]
    public int[] individualLv = new int[19];

    void Start()
    {
        individualLv = new int[19];
        if(extraStats == null)
        {
            extraStats = GetComponent<ExtraStats>();
        }
    }

    public void ApplyTower()
    {
        tower.bulletDamage = upgrades[individualLv[0]].bulletDamage;
        tower.bulletPierce = upgrades[individualLv[1]].pierce;
        tower.o_reloadTime = upgrades[individualLv[2]].reloadTime;
        tower.rotationSpeed = upgrades[individualLv[3]].rotationSpeed;
        if(tower.towerType != TowerType.Farm)
        {
            tower.range = upgrades[individualLv[4]].range;
        }

        tower.slowPercentage = upgrades[individualLv[5]].slowPercentage;
        tower.freezeChance = upgrades[individualLv[6]].freezeChance;
        tower.criticalChance = upgrades[individualLv[7]].criticalChance;
        tower.explosionRadius = upgrades[individualLv[8]].splashRadius;
        tower.passiveDamage = upgrades[individualLv[9]].passiveDmg;
        tower.timeTillRemoval = upgrades[individualLv[10]].immunityDecay;
        tower.incomeGenerated = upgrades[individualLv[11]].incomeGenerated;
        tower.incomePercentage = upgrades[individualLv[12]].incomePercentage / 100f;
        tower.wavePercentage = upgrades[individualLv[13]].wavePercentage / 100f;
        tower.refundPercentage = upgrades[individualLv[14]].refundPercentage / 100f;
        tower.rangeIncrease = upgrades[individualLv[15]].rangeIncrease;
        tower.damageIncrease = upgrades[individualLv[16]].damageIncrease;
        tower.reloadDecrease = upgrades[individualLv[17]].reloadDecrease;
        tower.pierceIncrease = (int)upgrades[individualLv[18]].pierceIncrease;

        if(tower.passiveUpgrade.Length > 0)
        {
            int totalLvl = 0;
            foreach(int lvl in individualLv)
            {
                totalLvl += lvl;
            }

            for(int i = tower.passiveUpgrade.Length - 1; i >= 0; i--)
            {
                if(totalLvl >= tower.passiveUpgrade[i].requirement)
                {
                    if(tower.towerType == TowerType.Minigun)
                    {
                        tower.shotsTillHeated = tower.passiveUpgrade[i].rewardValue;
                    }
                    break;
                }
            }
        }
        tower.UpdateRange();

        if(tower.towerType == TowerType.Village)
        {
            tower.CheckTowers();
        }

        if(extraStats == null)
        {
            return;
        }


        extraStats.KeepTrack();
        if(extraStats.isUnderRange)
        {
            extraStats.ApplyToTower();
        }
    }
}