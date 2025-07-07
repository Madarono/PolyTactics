using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Tower tower;
    public float damage;
    public int pierce;

    [Header("Immunity")]
    public float duration = 2f;

    [Header("Special Attirbutes - Splash")]
    public bool isSplash;
    public Pool visual;
    public float durationTillDisappear = 1f;
    public float explosionRadius = 3f;
    public float spreadTransfer = 0.5f;
    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent<Enemy>(out Enemy enemy))
        {
            float multiplyer = enemy.PI_Shield;
            if(multiplyer == 0 && enemy.immunities[enemy.cacheImmunity].immuneAgainst == tower.towerType)
            {
                GameObject immunity = tower.manager.immunityPool.GetFromPool();
                immunity.transform.position = col.gameObject.transform.position;
                tower.OutsideCallPool(immunity, duration, tower.manager.immunityPool);
                Destroy(gameObject);
                return;
            }
            else if(enemy.immunities[enemy.cacheImmunity].immuneAgainst != tower.towerType)
            {
                multiplyer = 1;
            }

            enemy.health -= damage * multiplyer;
            enemy.Refresh();

            if(isSplash)
            {
                LayerMask enemyLayer = LayerMask.GetMask("Enemy");
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
                
                System.Array.Sort(hits, (a, b) =>
                {
                    float da = Vector2.SqrMagnitude(a.transform.position - transform.position);
                    float db = Vector2.SqrMagnitude(b.transform.position - transform.position);
                    return da.CompareTo(db);
                });

                GameObject go = visual.GetFromPool();
                go.transform.localScale = new Vector2(explosionRadius, explosionRadius);
                go.transform.position = transform.position;
                if(go.TryGetComponent(out BulletPool goScript))
                {
                    goScript.pool = visual;
                    goScript.GoToPool(durationTillDisappear);
                }
                
                int enemyHits = 1;

                foreach(var hit in hits)
                {
                    if(hit.gameObject.TryGetComponent(out Enemy hitScript) && hit != col && (hitScript.immunities[hitScript.cacheImmunity].immuneAgainst != tower.towerType || 
                    (hitScript.immunities[hitScript.cacheImmunity].immuneAgainst == tower.towerType && hitScript.PI_Shield > 0)))
                    {
                        float decrease = 1/enemyHits;
                        hitScript.health -= damage * spreadTransfer * decrease * multiplyer;
                        hitScript.Refresh();
                        enemyHits++;
                    }
                }
            }
            pierce--;
            
            if(pierce > 0)
            {
                return;
            }
            
            Destroy(gameObject);
        }
    }
}