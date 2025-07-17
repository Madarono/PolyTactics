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
}

public class TowerUpgrade : MonoBehaviour
{
    public Tower tower;
    public Upgrades[] upgrades;
    public int sellValue;
    public bool provideTargetting = true;

    [Tooltip("Every index is equal to the index in the upgrades showing individual level to each upgrade")]
    public int[] individualLv = new int[11];

    void Start()
    {
        individualLv = new int[11];
    }

    public void ApplyTower()
    {
        tower.bulletDamage = upgrades[individualLv[0]].bulletDamage;
        tower.bulletPierce = upgrades[individualLv[1]].pierce;
        tower.o_reloadTime = upgrades[individualLv[2]].reloadTime;
        tower.rotationSpeed = upgrades[individualLv[3]].rotationSpeed;
        tower.range = upgrades[individualLv[4]].range;
        tower.slowPercentage = upgrades[individualLv[5]].slowPercentage;
        tower.freezeChance = upgrades[individualLv[6]].freezeChance;
        tower.criticalChance = upgrades[individualLv[7]].criticalChance;
        tower.explosionRadius = upgrades[individualLv[8]].splashRadius;
        tower.passiveDamage = upgrades[individualLv[9]].passiveDmg;
        tower.timeTillRemoval = upgrades[individualLv[10]].immunityDecay;
        tower.UpdateRange();
    }
}