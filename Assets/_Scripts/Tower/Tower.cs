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

public enum TowerType
{
    Basic,
    Freezer,
}

public class Tower : MonoBehaviour
{
    [Header("Identity")]
    public Factions faction;
    public TowerType towerType;
    public string towerName;

    public TowerUpgrade upgrade;
    [HideInInspector]public UpgradeManager upgradeManager;
    [HideInInspector]public TowerManager manager;
    
    [Tooltip("This is when you're preparing to buy the turrent, this avoids any checks and only checks for range.")]
    public bool isDebug;

    [Header("Attributes - Tower")]
    public Targetting targetting;
    public float bulletDamage = 2f;
    public float bulletLifespan = 10f;
    public float reloadTime = 0.5f;
    public float rotationSpeed = 4.5f;
    public int bulletPierce = 1;
    public float fireForce = 4f;

    [Header("Attrubutes - Freezer")]
    public float slowPercentage = 0.5f;
    public float freezeChance = 10f;
    public float freezeDuration = 1f;

    [Header("Animation")]
    public Animator towerAnim;
    
    [HideInInspector]public float o_reloadTime;

    [Header("Firing")]
    // public float searchDelay = 1f;
    // private float o_SearchDelay;
    public float angleOffset = 2f;
    public Transform firePoint;
    public GameObject bulletPrefab;

    [Header("Range")]
    public GameObject rangeObj;
    public SpriteRenderer rangeRend;
    public float range = 2f;
    private bool isHovering;
    public bool isSelected;

    [Header("Enemy detection")]
    public float predictionMultiplyer = 10f;
    public List<EnemyInfo> enemy = new List<EnemyInfo>();
    public GameObject lockOnEnemy;
    private int indexOfEnemy;

    void Start()
    {
        o_reloadTime = reloadTime;
        // o_SearchDelay = searchDelay;
        rangeRend.enabled = false;
        rangeObj.SetActive(true);
        UpdateRange();

        if(isDebug)
        {
            rangeRend.enabled = true;
            rangeObj.SetActive(true);
            this.enabled = false;
        }
    }

    void Update()
    {
        if(reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;
        }

        // if(searchDelay > 0)
        // {
        //     searchDelay -= Time.deltaTime;
        // }
        // else
        // {
        //     searchDelay = o_SearchDelay;
        //     UpdateValues();
        //     SelectEnemy();
        // }

        if(isHovering || isSelected)
        {
            rangeRend.enabled = true;
        }
        else if(!isHovering && !isSelected)
        {
            rangeRend.enabled = false;
        }
    }

    void FixedUpdate()
    {
        if(lockOnEnemy == null || towerType == TowerType.Freezer)
        {
            return;
        }

        Vector3 predictedPosition = Vector3.MoveTowards(lockOnEnemy.transform.position, enemy[indexOfEnemy].enemy.waypoint[enemy[indexOfEnemy].enemy.waypointIndex], Time.deltaTime * enemy[indexOfEnemy].enemy.speed * predictionMultiplyer);

        Vector3 direction = (predictedPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed * 100f);
        float difference = Quaternion.Angle(transform.rotation, targetRotation);
        if(difference <= angleOffset && reloadTime <= 0)
        {
            reloadTime = o_reloadTime;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject go = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        go.transform.rotation = transform.rotation;

        if(go.TryGetComponent(out Rigidbody2D rb) && go.TryGetComponent(out Bullet bullet))
        {
            bullet.damage = bulletDamage;
            bullet.pierce = bulletPierce;
            Destroy(go, bulletLifespan);
            rb.AddForce(firePoint.transform.right * fireForce, ForceMode2D.Impulse);
            towerAnim.SetTrigger(Animator.StringToHash("Shoot"));
        }
    }

    public void GatherEnemy(Enemy enemyScript)
    {
        EnemyInfo colScript = new EnemyInfo();
        colScript.enemy = enemyScript;
        enemy.Add(colScript);
        UpdateValues(); 
    }
    public void RemoveEnemy(Enemy enemyScript)
    {
        for(int i = 0; i < enemy.Count; i++)
        {
            if(enemy[i].enemy == enemyScript)
            {
                enemy.RemoveAt(i);
                UpdateValues();
                SelectEnemy();
                break;
            }
        }
    }

    public void UpdateValues()
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

    public void SelectEnemy()
    {
        lockOnEnemy = null;
        indexOfEnemy = 0;
        if(enemy.Count == 0)
        {
            return;
        }

        if(towerType == TowerType.Basic)
        {
            switch(targetting)
            {
                case Targetting.First:
                    int firstEnemy = 0; 
                    float closestDistance = 999;
                    int highestPlacement = 0;
                    for(int i = 0; i < enemy.Count; i++)
                    {
                        if(enemy[i].placement > highestPlacement)
                        {
                            firstEnemy = i;
                            closestDistance = enemy[i].distance;
                            highestPlacement = enemy[i].placement;
                        }
                        else if(enemy[i].placement == highestPlacement && enemy[i].distance < closestDistance)
                        {
                            firstEnemy = i;
                            closestDistance = enemy[i].distance;
                            highestPlacement = enemy[i].placement;
                        }
                    }

                    lockOnEnemy = enemy[firstEnemy].enemy.gameObject;
                    indexOfEnemy = firstEnemy;
                    break;

                case Targetting.Last:
                    int lastEnemy = 0; 
                    float farthestDistance = 0;
                    int lowestPlacement = 999;
                    for(int i = 0; i < enemy.Count; i++)
                    {
                        if(enemy[i].placement < lowestPlacement)
                        {
                            lastEnemy = i;
                            farthestDistance = enemy[i].distance;
                            lowestPlacement = enemy[i].placement;
                        }
                        if(enemy[i].placement == lowestPlacement && enemy[i].distance > farthestDistance)
                        {
                            lastEnemy = i;
                            farthestDistance = enemy[i].distance;
                            lowestPlacement = enemy[i].placement;
                        }
                    }

                    lockOnEnemy = enemy[lastEnemy].enemy.gameObject;
                    indexOfEnemy = lastEnemy;
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
                    indexOfEnemy = strongEnemy;
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
                    indexOfEnemy = weakEnemy;
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
                    indexOfEnemy = closeEnemy;
                    break;

            }
        }
        else if(towerType == TowerType.Freezer)
        {
            foreach(EnemyInfo script in enemy)
            {
                // script.enemy.speed = script.enemy.o_speed * slowPercentage;
                float random = Random.Range(0, 100);
                if(random <= freezeChance)
                {
                    script.enemy.Freeze(freezeDuration, script.enemy.o_speed * slowPercentage);
                }
            }
        }
        
    }

    public void UpdateRange()
    {
        float diameter = range * 2f;
        rangeObj.transform.localScale = new Vector3(diameter, diameter, 1f);
    }

    public void ShowRange()
    {
        if(manager.isSelecting)
        {
            isHovering = false;
            return;
        }

        isHovering = true;
    }
    public void HideRange()
    {
        if(manager.isSelecting)
        {
            isHovering = false;
            return;
        }

        isHovering = false;
    }

    public void BothInfo()
    {
        if(manager.isSelecting)
        {
            return;
        }

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
        upgradeManager.tower = upgrade;
        upgradeManager.UpdateWindowPositioning();
        upgradeManager.UpdateValues();
        isSelected = true;
    }
    public void HideInfo()
    {
        isSelected = false; 
        if(upgradeManager.tower == upgrade)
        {
            upgradeManager.tower = null;
        }
    }
}
