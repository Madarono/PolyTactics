using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Immunity
{
    public TowerType immuneAgainst;
    public Color immunityColor;
}

public class Enemy : MonoBehaviour
{
    [HideInInspector]public EnemyManager manager;
    [HideInInspector]public Settings settings;
    public List<Vector3> waypoint = new List<Vector3>();
    public int waypointIndex;
    
    [Header("Attributes")]
    public float speed = 1f;
    [HideInInspector]public float o_speed;
    public float health;
    public int damageToBase = 2;
    [HideInInspector]public float o_health;

    [Header("Rotation")]
    public float rotationRatio = 10/3;
    private float rotationSpeed = 2f;
    public float requirementDistance = 0.01f;

    [Header("Health bar")]
    public Transform healthBar;
    public float maxScale = 0.95f;
    public float minScale = 0f;

    [Header("Effects")]
    public SpriteRenderer overlayEffect;
    public SpriteRenderer criticalEffect;
    private Color cold;

    [Header("Extra Enemies")]
    public Enemy[] childEnemies;

    [Header("Shield")]
    public Transform shieldBar; 
    public float shieldChance = 10f;
    private bool hasShield = false;
    private bool determinedShield = false;
    public float shieldHealth;
    public float shieldWait = 0.5f;
    public float shieldIncreaser = 0.05f;
    private float o_shieldHealth; 

    [Header("Healing")]
    public GameObject healRange;
    public float range = 2f;
    public float healDelay = 0.1f;
    public float healPercentage = 0.01f / 6f;
    public List<Enemy> enemyRange = new List<Enemy>();

    [Header("Immunity")]
    public Immunity[] immunities;
    public SpriteRenderer immunityEffect;
    public Animator immunityAnimator;
    public float chanceOfImmunity = 15f;
    public float chanceOfFull = 5f;
    public float PI_Shield = 0.4f;
    private bool determinedImmunity;
    [HideInInspector]public int cacheImmunity;

    [Header("Camera shake Base")]
    public float magnitude = 0.05f;
    public float time = 0.1f; 


    [Header("Reward")]
    public int moneyReward = 5;


    private float slowSpeed;
    [HideInInspector]public bool isSlowed;

    [Header("Health Bar wait")]
    private GameObject healthBarParent;
    public float delayTillHide = 3f;
    Coroutine healthBarCoroutine;
    

    void Start()
    {
        healthBarParent = healthBar.parent.gameObject;
        if(!determinedShield)
        {
            DetermineShield();
        }
        if(healRange != null)
        {
            healRange.transform.localScale = new Vector3(range, range, healRange.transform.localScale.z);
            StartCoroutine(HealAround());
        }
        if(o_shieldHealth > 0)
        {
            StartCoroutine(HealShield());
        }
        if (manager == null)
        {
            manager = EnemyManager.Instance;
        }
        if (settings == null)
        {
            settings = Settings.Instance;
        }
        if(waypoint.Count == 0)
        {
            SetWaypoints(manager.waypoints);
        }
        if(childEnemies.Length > 0)
        {
            foreach(Enemy script in childEnemies)
            {
                script.manager = manager;
                script.settings = settings;
                script.health = script.health * (1 + (manager.healthScale * manager.currentWave)) * manager.scalingMultiplyer[manager.index].multiplyer;
                script.shieldHealth = script.shieldHealth * (1 + (manager.healthScale * manager.currentWave)) * manager.scalingMultiplyer[manager.index].multiplyer;
                script.speed = script.speed * (1 + (manager.speedScale * manager.currentWave)) * manager.scalingMultiplyer[manager.index].multiplyer;
                script.moneyReward *= Mathf.RoundToInt(manager.coinMultipler[manager.index].multiplyer);
                script.DetermineImmunity();
                script.DetermineShield();
                script.enabled = false;
            }
        }

        if(!determinedImmunity)
        {
            DetermineImmunity();
        }
        overlayEffect.gameObject.SetActive(false);
        o_health = health;
        o_speed = speed;
        rotationSpeed = speed * rotationRatio;
        // Refresh();
        healthBarParent.SetActive(false);
    }

    public void HurtEnemy(float amount)
    {
        if(healthBarCoroutine != null)
        {
            StopCoroutine(healthBarCoroutine);
        }

        healthBarParent.SetActive(true);

        float remainingAmount = amount;
        if(hasShield)
        {
            shieldHealth -= amount;
            if(shieldHealth < 0)
            {
                remainingAmount = Mathf.Abs(shieldHealth);
                shieldHealth = 0;
            }
            else
            {
                remainingAmount = 0;
            }
        }

        healthBarCoroutine = StartCoroutine(HideHealthBar());

        health -= remainingAmount;
    } 

    private void DetermineImmunity()
    {
        float random = Random.Range(0, 100);
        float chance = Mathf.Min(chanceOfImmunity * manager.immunityMultipler[manager.index].multiplyer, 100f) * (1 + (manager.immunityScale * manager.currentWave)) * manager.scalingMultiplyer[manager.index].multiplyer;
        float chanceFull = Mathf.Min(chanceOfFull * manager.immunityMultipler[manager.index].multiplyer, 100f) * (1 + (manager.immunityScale * manager.currentWave)) * manager.scalingMultiplyer[manager.index].multiplyer;;
        if(random <= chanceFull)
        {
            cacheImmunity = Random.Range(0, immunities.Length);
            immunityEffect.color = immunities[cacheImmunity].immunityColor;
            immunityEffect.gameObject.SetActive(true);
            immunityAnimator.SetBool("Full", false);
            PI_Shield = 0f;
        }
        else if(random <= chance)
        {
            cacheImmunity = Random.Range(0, immunities.Length);
            immunityEffect.color = immunities[cacheImmunity].immunityColor;
            immunityEffect.gameObject.SetActive(true);
            immunityAnimator.SetBool("Full", true);
            PI_Shield =  PI_Shield * manager.PI_Multiplyer[manager.index].multiplyer;
        }
        else
        {
            RemoveImmunities();
        }

        determinedImmunity = true;
    }

    private void DetermineShield()
    {
        float random = Random.Range(0, 100);
        float chance = Mathf.Min(shieldChance * manager.immunityMultipler[manager.index].multiplyer, 100f) * (1 + (manager.immunityScale * manager.currentWave)) * manager.scalingMultiplyer[manager.index].multiplyer;
        if(random <= chance)
        {
            hasShield = true;
            o_shieldHealth = shieldHealth;
            shieldBar.gameObject.SetActive(true);
        }
        else
        {
            hasShield = false;
            o_shieldHealth = 0;
            shieldBar.gameObject.SetActive(false);
        }

        determinedShield = true;
    }
    
    public void RemoveImmunities()
    {   
        PI_Shield = 1f;
        immunityEffect.gameObject.SetActive(false);
    }

    public void SetWaypoints(Transform[] pos)
    {
        foreach(Transform p in pos)
        {
            waypoint.Add(p.position);
        }
    }

    public void Refresh()
    {
        float percentage = health / o_health;
        float newScale = Mathf.Lerp(minScale, maxScale, percentage);
        healthBar.localScale = new Vector3(newScale, healthBar.localScale.y, healthBar.localScale.z);
        
        if(hasShield)
        {
            float percentageShield = shieldHealth / o_shieldHealth; 
            float newShieldScale = Mathf.Lerp(minScale, maxScale, percentageShield);
            shieldBar.localScale = new Vector3(newShieldScale, shieldBar.localScale.y, shieldBar.localScale.z);
        }
        
        if(health <= 0)
        {
            if(childEnemies.Length > 0)
            {
                foreach(Enemy script in childEnemies)
                {
                    script.waypoint = waypoint;
                    script.waypointIndex = waypointIndex;
                    script.enabled = true;
                    script.gameObject.SetActive(true);
                    script.transform.SetParent(manager.enemyParent);
                    manager.enemiesLeft++;
                }
            }
            settings.money += moneyReward;
            settings.UpdateVisual();
            manager.DestroyEnemy(gameObject);
            this.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if(waypoint.Count == 0)
        {
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, waypoint[waypointIndex], Time.deltaTime * speed);
        
        Vector3 direction = waypoint[waypointIndex] - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotationSpeed);

        float distance = Vector2.Distance(transform.position, waypoint[waypointIndex]);
        if((distance <= requirementDistance) && waypointIndex <= waypoint.Count - 1)
        {
            waypointIndex++;
            if(waypointIndex == waypoint.Count)
            {
                settings.health -= damageToBase;
                settings.CheckHealth();
                int randomIndex = Random.Range(0, manager.soundManager.baseHit.Length);
                manager.soundManager.PlayClip(manager.soundManager.baseHit[randomIndex], 1f);
                if(PauseSystem.Instance.screenShake)
                {
                    CameraShake.Instance.Shake(time, magnitude);
                }
                manager.DestroyEnemy(gameObject);
            }
        }
    }

    public void Freeze(float duration, float slowSpeed, Color cold, Color freeze)
    {
        speed = 0;
        this.slowSpeed = slowSpeed;
        this.cold = cold;
        overlayEffect.color = freeze;
        Invoke("ReturnToSlow", duration);
    }

    void ReturnToSlow()
    {
        speed = slowSpeed;
        overlayEffect.color = cold;
    }

    public void VisualCritical(Color flashColor, float flashDuration, float fadeDuration)
    {
        StartCoroutine(CriticalDamage(flashColor, flashDuration, fadeDuration));
    }

    IEnumerator CriticalDamage(Color flashColor, float flashDuration, float fadeDuration)
    {
        Color originalColor = criticalEffect.color;

        criticalEffect.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        float timer = 0f;
        while(timer < fadeDuration) //Here fades it
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            criticalEffect.color = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        criticalEffect.color = new Color(flashColor.r, flashColor.g, flashColor.b, 0f);
    }

    public IEnumerator TimeForRemoval(float duration, Tower tower)
    {
        yield return new WaitForSeconds(duration);
        if(tower.debuffEnemies.Contains(gameObject))
        {
            if(PI_Shield < 1)
            {
                manager.soundManager.PlayClip(manager.soundManager.removeImmunity, 1f);
            }
            RemoveImmunities();
        }
    }

    IEnumerator HealShield()
    {
        while(shieldHealth > 0)
        {
            shieldHealth = Mathf.Min(shieldHealth + (o_shieldHealth * shieldIncreaser), o_shieldHealth);
            float percentageShield = shieldHealth / o_shieldHealth; 
            float newShieldScale = Mathf.Lerp(minScale, maxScale, percentageShield);
            shieldBar.localScale = new Vector3(newShieldScale, shieldBar.localScale.y, shieldBar.localScale.z);
            yield return new WaitForSeconds(shieldWait);
        }
    }

    IEnumerator HideHealthBar()
    {
        yield return new WaitForSeconds(delayTillHide);
        healthBarParent.SetActive(false);
    }

    IEnumerator HealAround()
    {
        bool loop = true;
        
        while(loop)
        {
            if(enemyRange.Count > 0)
            {
                foreach(Enemy script in new List<Enemy>(enemyRange))
                {
                    if(script.enabled)
                    {
                        script.health += script.health * healPercentage;
                        script.health = Mathf.Min(script.health, script.o_health);
                        script.Refresh();
                    }
                }
            } 

            yield return new WaitForSeconds(healDelay);
        }
    }
}