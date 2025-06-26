using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [HideInInspector]public List<Vector3> waypoint = new List<Vector3>();
    [HideInInspector]public int waypointIndex;
    
    [Header("Attributes")]
    public float speed = 1f;
    public float health;
    private float o_health;

    [Header("Rotation")]
    public float rotationRatio = 10/3;
    private float rotationSpeed = 2f;
    public float requirementDistance = 0.01f;

    [Header("Health bar")]
    public Transform healthBar;
    public float maxScale = 0.95f;
    public float minScale = 0f;
    

    void Start()
    {
        o_health = health;
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
            //Give coins here
            Destroy(gameObject);
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
        if((distance <= requirementDistance) && waypointIndex < waypoint.Count)
        {
            waypointIndex++;
        }
    }
}