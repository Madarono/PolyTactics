using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public TowerManager towerManager;
    public TowerType type = TowerType.Trap;
    public int pierce = 5;
    public int roundsAlive = 5;
    public int maxUses = 20;
    public List<GameObject> currentEnemies;
    public float slownessPercent = 0.5f;

    [Header("Bomb")] //It is slow by default;
    public bool isBomb;
    public float percentOfDamage;
    public float maxDamage;

    private float cacheSpeed;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent(out Enemy enemy) && enemy.immunities[enemy.cacheImmunity].immuneAgainst != type && !currentEnemies.Contains(col.gameObject) && (currentEnemies.Count < pierce || (currentEnemies.Count < maxUses) && maxUses < pierce))
        {
            currentEnemies.Add(col.gameObject);

            if(isBomb)
            {
                float damage = Mathf.Min(enemy.o_health * percentOfDamage, maxDamage);
                enemy.health -= damage;
                enemy.Refresh();
            }
            else
            {
                cacheSpeed = enemy.speed;
                enemy.speed = enemy.o_speed * slownessPercent;
            }
            maxUses--;
            if(maxUses >= 0)
            {
                return;
            }

            if(enemy.isSlowed && !isBomb)
            {
                enemy.speed = cacheSpeed;
            }
            else if(!isBomb)
            {
                enemy.speed = enemy.o_speed;
            }


            Vector3 towerPos = gameObject.transform.position;
            Vector3Int cellPos = towerManager.trapTilemap.WorldToCell(towerPos);
            int tileIndex = towerManager.trapPositions.IndexOf(cellPos);
            towerManager.isFull_Trap[tileIndex] = false;
            Destroy(gameObject);
        }
    } 

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.TryGetComponent(out Enemy enemy) && currentEnemies.Contains(col.gameObject))
        {
            currentEnemies.Remove(col.gameObject);

            if(enemy.isSlowed && !isBomb)
            {
                enemy.speed = cacheSpeed;
            }
            else if(!isBomb)
            {
                enemy.speed = enemy.o_speed;
            }
            
            if(maxUses <= 0)
            {
                Vector3 towerPos = gameObject.transform.position;
                Vector3Int cellPos = towerManager.trapTilemap.WorldToCell(towerPos);
                int tileIndex = towerManager.trapPositions.IndexOf(cellPos);
                towerManager.isFull_Trap[tileIndex] = false;
                Destroy(gameObject);
            }
        }
    }

    public void ReduceLives()
    {
        roundsAlive--;
        if(roundsAlive <= 0)
        {
            Vector3 towerPos = gameObject.transform.position;
            Vector3Int cellPos = towerManager.trapTilemap.WorldToCell(towerPos);
            int tileIndex = towerManager.trapPositions.IndexOf(cellPos);
            towerManager.isFull_Trap[tileIndex] = false;
            towerManager.trapTower.Remove(this);
            Destroy(gameObject);
        }
    }
}
