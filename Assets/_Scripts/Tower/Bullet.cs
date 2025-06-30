using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int pierce;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.health -= damage;
            enemy.Refresh();
            pierce--;
            
            if(pierce > 0)
            {
                return;
            }
                
            Destroy(gameObject);
        }
    }
}