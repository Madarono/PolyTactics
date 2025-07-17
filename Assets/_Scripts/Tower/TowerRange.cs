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
            switch(tower.towerType)
            {
                case TowerType.Freezer:
                    float PI_Multipler = 1f; //PI -> Partial Immunity
                    if(tower.towerType == enemy.immunities[enemy.cacheImmunity].immuneAgainst)
                    {
                        PI_Multipler = enemy.PI_Shield;
                    }
                    enemy.speed = Mathf.Lerp(enemy.o_speed, enemy.o_speed * tower.slowPercentage, PI_Multipler);
                    enemy.overlayEffect.gameObject.SetActive(true);
                    enemy.overlayEffect.color = tower.cold;
                    enemy.isSlowed = true;
                    break;

                case TowerType.Debuff:
                    tower.debuffEnemies.Add(col.gameObject);
                    enemy.StartCoroutine(enemy.TimeForRemoval(tower.timeTillRemoval, tower));
                    break;

            }
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if(col.TryGetComponent<Enemy>(out Enemy enemy))
        {
            tower.RemoveEnemy(enemy);

            switch(tower.towerType)
            {         
                case TowerType.Freezer:
                    enemy.speed = enemy.o_speed;
                    enemy.overlayEffect.gameObject.SetActive(false);
                    enemy.isSlowed = false;
                    break;

                case TowerType.Debuff:
                    if(tower.debuffEnemies.Contains(col.gameObject))
                    {
                        tower.debuffEnemies.Remove(col.gameObject);
                    }
                    break;
            }
        }
    }
}