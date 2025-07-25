using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ExtraStats : MonoBehaviour
{
    public Tower tower;
    public float rangeIncrease;
    public float damageIncrease;
    public float reloadDecrease;
    public int pierceIncrease;

    public float[] track = new float[4];
    public bool isUnderRange;

    void Awake()
    {
        KeepTrack();
    }

    public void KeepTrack()
    {
        track[0] = tower.range;
        track[1] = tower.bulletDamage;
        track[2] = tower.o_reloadTime;
        track[3] = (float)tower.bulletPierce;
    }

    public void ApplyToTower()
    {
        tower.range = track[0] + rangeIncrease;
        tower.bulletDamage = track[1] + damageIncrease;
        tower.o_reloadTime = Mathf.Max(tower.o_reloadTime = track[2] - reloadDecrease, 0.05f);
        tower.reloadTime = tower.o_reloadTime;
        tower.bulletPierce = (int)track[3] + pierceIncrease; 
        tower.UpdateRange();
    }

    public void ReturnToDefaults()
    {
        tower.range = track[0];
        tower.bulletDamage = track[1];
        tower.o_reloadTime = track[2];
        tower.bulletPierce = (int)track[3];
        tower.reloadTime = tower.o_reloadTime;
        tower.UpdateRange();
    }
}