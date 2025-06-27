using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Upgrades
{
    [Header("Damage")]
    public float bulletDamage;
    public int bulletPrice;

    [Header("Bullet speed")]
    public float fireForce;
    public int firePrice;
    
    [Header("Reload time")]
    public float reloadTime;
    public int reloadPrice;
    
    [Header("Turning speed")]
    public float rotationSpeed;
    public int rotationPrice;

    [Header("Range")]
    public float range;
    public int rangePrice;
}

public class TowerUpgrade : MonoBehaviour
{
    public Tower tower;
    public Upgrades[] upgrades;
    public int sellValue;

    [Tooltip("Every index is equal to the index in the upgrades showing individual level to each upgrade")]
    public int[] individualLv = new int[5];

    public void ApplyTower()
    {
        tower.bulletDamage = upgrades[individualLv[0]].bulletDamage;
        tower.fireForce = upgrades[individualLv[1]].fireForce;
        tower.o_reloadTime = upgrades[individualLv[2]].reloadTime;
        tower.rotationSpeed = upgrades[individualLv[3]].rotationSpeed;
        tower.range = upgrades[individualLv[4]].range;
        tower.UpdateRange();
    }
}