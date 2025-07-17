using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EnemyRange : MonoBehaviour
{
    public Enemy parentEnemy;

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent(out Enemy colScript))
        {
            parentEnemy.enemyRange.Add(colScript);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.TryGetComponent(out Enemy colScript) && parentEnemy.enemyRange.Contains(colScript))
        {
            parentEnemy.enemyRange.Remove(colScript);
        }
    }

}