using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public int pierce = 5;
    public int maxUses = 20;
    public List<GameObject> currentEnemies;
    public float slownessPercent = 0.5f;

    private float cacheSpeed;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent(out Enemy enemy) && !currentEnemies.Contains(col.gameObject) && (currentEnemies.Count < pierce || (currentEnemies.Count < maxUses) && maxUses < pierce))
        {
            currentEnemies.Add(col.gameObject);
            cacheSpeed = enemy.speed;
            enemy.speed = enemy.o_speed * slownessPercent;
        }
    } 

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.TryGetComponent(out Enemy enemy) && currentEnemies.Contains(col.gameObject))
        {
            currentEnemies.Remove(col.gameObject);
            if(enemy.isSlowed)
            {
                enemy.speed = cacheSpeed;
            }
            else
            {
                enemy.speed = enemy.o_speed;
            }
            maxUses--;
            if(maxUses <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
