using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyInfo
{
    public Enemy enemy;
    public float distance;
    public float distanceToPlayer;
    public float health;
    public int placement;
}

public enum Targetting
{
    First,
    Last,
    Strong,
    Weak,
    Close
}

public class Tower : MonoBehaviour
{
    [HideInInspector]public TowerManager manager;
    [Tooltip("This is when you're preparing to buy the turrent, this avoids any checks and only checks for range.")]
    public bool isDebug;

    [Header("Attributes")]
    public Targetting targetting;
    public float reloadTime;
    private float o_reloadTime;

    [Header("Range")]
    public GameObject rangeObj;
    public SpriteRenderer rangeRend;
    public float range;
    private bool isHovering;
    public bool isSelected;

    [Header("Enemy detection")]
    public List<EnemyInfo> enemy = new List<EnemyInfo>();
    public GameObject lockOnEnemy;

    void Start()
    {
        o_reloadTime = reloadTime;
        rangeRend.enabled = false;
        rangeObj.SetActive(true);
        UpdateRange();
    }

    void Update()
    {
        if(isDebug)
        {
            rangeRend.enabled = true;
            rangeObj.SetActive(true);
            return;
        }

        if(reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;
        }
        else
        {
            reloadTime = o_reloadTime;
            UpdateValues();
            SelectEnemy();
        }

        if(isHovering || isSelected)
        {
            rangeRend.enabled = true;
        }
        else if(!isHovering && !isSelected)
        {
            rangeRend.enabled = false;
        }
    }

    public void GatherEnemy(GameObject obj)
    {
        EnemyInfo colScript = new EnemyInfo();
        colScript.enemy = obj.gameObject.GetComponent<Enemy>();
        enemy.Add(colScript);
        UpdateValues(); 
    }

    void UpdateValues()
    {
        foreach(EnemyInfo info in enemy)
        {
            if(info.enemy != null)
            {
                info.placement = info.enemy.waypointIndex;
                info.distance = Vector2.Distance(info.enemy.transform.position, info.enemy.waypoint[info.placement]);
                info.distanceToPlayer = Vector2.Distance(info.enemy.transform.position, transform.position);
                info.health = info.enemy.health;
            }
            else
            {
                enemy.Remove(info);
            }
        }
    }

    void SelectEnemy()
    {
        if(enemy.Count == 0)
        {
            return;
        }

        switch(targetting)
        {
            case Targetting.First:
                int firstEnemy = 0; 
                float closestDistance = 999;
                int highestPlacement = 0;
                for(int i = 0; i < enemy.Count; i++)
                {
                    if(enemy[i].placement >= highestPlacement && enemy[i].distance < closestDistance)
                    {
                        firstEnemy = i;
                        closestDistance = enemy[i].distance;
                        highestPlacement = enemy[i].placement;
                    }
                }

                lockOnEnemy = enemy[firstEnemy].enemy.gameObject;
                break;

            case Targetting.Last:
                int lastEnemy = 0; 
                float farthestDistance = 0;
                int lowestPlacement = 999;
                for(int i = 0; i < enemy.Count; i++)
                {
                    if(enemy[i].placement <= lowestPlacement && enemy[i].distance > farthestDistance)
                    {
                        lastEnemy = i;
                        farthestDistance = enemy[i].distance;
                        lowestPlacement = enemy[i].placement;
                    }
                }

                lockOnEnemy = enemy[lastEnemy].enemy.gameObject;
                break;

            case Targetting.Strong:
                int strongEnemy = 0; 
                float _health = 0;
                float _closestDistance = 999;
                int _highestPlacement = 0;
                for(int i = 0; i < enemy.Count; i++)
                {
                    if(enemy[i].health > _health)
                    {
                        strongEnemy = i;
                        _health = enemy[i].health;
                        _closestDistance = enemy[i].distance;
                        _highestPlacement = enemy[i].placement;
                    }
                    else if(enemy[i].health == _health)
                    {
                        if(enemy[i].placement >= _highestPlacement && enemy[i].distance < _closestDistance)
                        {
                            strongEnemy = i;
                            _health = enemy[i].health;
                            _closestDistance = enemy[i].distance;
                            _highestPlacement = enemy[i].placement;
                        }
                    }
                }

                lockOnEnemy = enemy[strongEnemy].enemy.gameObject;
                break;

            case Targetting.Weak:
                int weakEnemy = 0; 
                float health = 999;
                float closestDistance_ = 999;
                int highestPlacement_ = 0;
                for(int i = 0; i < enemy.Count; i++)
                {
                    if(enemy[i].health < health)
                    {
                        weakEnemy = i;
                        health = enemy[i].health;
                        closestDistance_ = enemy[i].distance;
                        highestPlacement_ = enemy[i].placement;
                    }
                    else if(enemy[i].health == health)
                    {
                        if(enemy[i].placement >= highestPlacement_ && enemy[i].distance < closestDistance_)
                        {
                            weakEnemy = i;
                            health = enemy[i].health;
                            closestDistance_ = enemy[i].distance;
                            highestPlacement_ = enemy[i].placement;
                        }
                    }
                }

                lockOnEnemy = enemy[weakEnemy].enemy.gameObject;
                break;

            case Targetting.Close:
                int closeEnemy = 0;
                float closeDistance = 999;
                for(int i = 0; i < enemy.Count; i++)
                {
                    if(enemy[i].distanceToPlayer <= closeDistance)
                    {
                        closeEnemy = i;
                        closeDistance = enemy[i].distanceToPlayer;
                    }
                }

                lockOnEnemy = enemy[closeEnemy].enemy.gameObject;
                break;

        }
    }

    void UpdateRange()
    {
        float diameter = range * 2f;
        rangeObj.transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    public void ShowRange()
    {
        isHovering = true;
    }
    public void HideRange()
    {
        isHovering = false;
    }

    public void BothInfo()
    {
        isSelected = !isSelected;

        if(isSelected)
        {
            ShowInfo();
        }
        else
        {
            HideInfo();
        }
    }
    public void ShowInfo()
    {
        manager.HideOtherTowerInfo();
        isSelected = true;
    }
    public void HideInfo()
    {
        isSelected = false; 
    }
}
