using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerRange : MonoBehaviour
{
    public Tower tower;
    public Collider2D col;

    void Start()
    {
        Collider2D[] results = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;
        filter.useLayerMask = false;

        int hits = col.OverlapCollider(filter, results);

        if(hits <= 0)
        {
            return;
        }
        
        for(int i = 0; i < hits; i++)
        {
            if(results[i].TryGetComponent<Enemy>(out Enemy enemy))
            {
                tower.GatherEnemy(enemy);
            }
        }
        tower.UpdateValues();
        tower.SelectEnemy();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent<Enemy>(out Enemy enemy))
        {
            tower.GatherEnemy(enemy);
            if(tower.towerType == TowerType.Freezer)
            {
                enemy.speed = enemy.o_speed * tower.slowPercentage;
                enemy.overlayEffect.gameObject.SetActive(true);
                enemy.overlayEffect.color = tower.cold;
                enemy.isSlowed = true;
            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.TryGetComponent<Enemy>(out Enemy enemy))
        {
            tower.RemoveEnemy(enemy);
            if(tower.towerType == TowerType.Freezer)
            {
                enemy.speed = enemy.o_speed;
                enemy.overlayEffect.gameObject.SetActive(false);
                enemy.isSlowed = false;
            }
        }
    }
}