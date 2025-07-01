using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float o_health;

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

    [Header("Reward")]
    public int moneyReward = 5;


    private float slowSpeed;
    

    void Start()
    {
        overlayEffect.gameObject.SetActive(false);
        o_health = health;
        o_speed = speed;
        rotationSpeed = speed * rotationRatio;
        Refresh();
    }

    public void SetWaypoints(Vector3[] pos)
    {
        foreach(Vector3 p in pos)
        {
            waypoint.Add(p);
        }
    }

    public void Refresh()
    {
        float percentage = health / o_health;
        float newScale = Mathf.Lerp(minScale, maxScale, percentage);
        healthBar.localScale = new Vector3(newScale, healthBar.localScale.y, healthBar.localScale.z);
        if(health <= 0)
        {
            settings.money += moneyReward;
            settings.UpdateVisual();
            manager.DestroyEnemy(gameObject);
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
}