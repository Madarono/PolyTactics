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
    public Color visualColor;
    public float durationTillDisappear = 1f;
    public float explosionRadius = 3f;
    public float spreadTransfer = 0.5f;

    [Header("Special Attributes - Knockback")]
    public float magnitude = 0.25f;
    public float _duration = 0.05f;

    
    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.TryGetComponent<Enemy>(out Enemy enemy))
        {
            float multiplyer = enemy.PI_Shield;
            TowerType type = tower.towerType;
            if(type == TowerType.Minigun || type == TowerType.Knockback)
            {
                type = TowerType.Basic;
            }

            if(multiplyer == 0 && enemy.immunities[enemy.cacheImmunity].immuneAgainst == type)
            {
                if(PauseSystem.Instance.graphics == 1)
                {
                    GameObject immunity = tower.manager.immunityPool.GetFromPool();
                    immunity.transform.position = col.gameObject.transform.position;
                    tower.OutsideCallPool(immunity, duration, tower.manager.immunityPool);
                }
                tower.sound.PlayClip(tower.sound.immunity, 1f);
                Destroy(gameObject);
                return;
            }
            else if(enemy.immunities[enemy.cacheImmunity].immuneAgainst != type)
            {
                multiplyer = 1;
            }

            enemy.HurtEnemy(damage * multiplyer);
            if(tower.towerType == TowerType.Knockback)
            {
                enemy.StartCoroutine(enemy.Knockback(tower.durationBack, tower.backMultipler * multiplyer));
                if(tower.backMultipler >= 4 && PauseSystem.Instance.screenShake)
                {
                    CameraShake.Instance.Shake(_duration, magnitude);
                }
            }
            if(enemy != null) //If the damage killed the enemy
            {
                enemy.Refresh();
            }

            if(isSplash)
            {
                tower.sound.PlayClip(tower.sound.explosion, 1f);
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
                if(go.TryGetComponent(out SpriteRenderer rend))
                {
                    rend.color = visualColor;
                }
                if(go.TryGetComponent(out BulletPool goScript))
                {
                    goScript.pool = visual;
                    goScript.GoToPool(durationTillDisappear);
                }
                
                int enemyHits = 1;

                foreach(var hit in hits)
                {
                    if(hit.gameObject.TryGetComponent(out Enemy hitScript) && hit != col && (hitScript.immunities[hitScript.cacheImmunity].immuneAgainst != type || 
                    (hitScript.immunities[hitScript.cacheImmunity].immuneAgainst == type && hitScript.PI_Shield > 0)))
                    {
                        float dist = Vector2.Distance(hit.transform.position, transform.position);
                        float falloff = Mathf.Clamp01(1f - dist / (explosionRadius));
                        hitScript.HurtEnemy(damage * spreadTransfer * falloff * multiplyer);
                        hitScript.Refresh();
                        enemyHits++;
                    }
                }
            }
            else
            {
                tower.sound.PlayClip(tower.sound.enemyHit, 2f);
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