using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class EnemyInfo
{
    public Enemy enemy;
    public float distance;
    public float distanceToPlayer;
    public float health;
    public int placement;
}

[System.Serializable]
public class RequirementInt
{
    public int requirement;
    public int rewardValue;
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
    Sniper,
    Splash,
    Trap,
    Debuff,
    Farm,
    Minigun,
    Village,
    Knockback
}

public class Tower : MonoBehaviour
{
    public bool isShocked = false;

    [Header("Identity")]
    public Factions faction;
    public TowerType towerType;
    public string towerName;

    public TowerUpgrade upgrade;
    [HideInInspector]public UpgradeManager upgradeManager;
    [HideInInspector]public TowerManager manager;
    [HideInInspector]public SoundManager sound;
    
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

    [Header("Range")]
    public GameObject rangeObj;
    public SpriteRenderer rangeRend;
    public float range = 2f;
    private bool isHovering;
    public bool isSelected;

    [Header("Special Attirbutes - Splash")]
    public Pool splashPool;
    public Color splashColor;
    public float explosionRadius = 3f;
    public float spreadTransfer = 0.5f;

    [Header("Special Attirbutes - Sniper")]
    public GameObject hitLine;
    public float criticalChance = 10f;
    public float criticalMultiplter = 2f;
    public Color criticalColor;
    public float flashDuration = 0.3f;
    public float fadeDuration = 0.5f;
    public Pool criticalPool;
    [HideInInspector]public GameObject activePool;
    public float criticalVisualDuration = 1f;

    [Header("Special Attrubutes - Freezer")]
    public float slowPercentage = 0.5f;
    public float freezeChance = 10f;
    public float freezeDuration = 1f;
    public Color cold;
    public Color freeze;

    [Header("Special Attriibutes - Debuff")]
    public float passiveDamage = 0.01f;
    public float timeTillRemoval = 1f;
    public List<GameObject> debuffEnemies = new List<GameObject>();

    [Header("Special Attributes - Farm")]
    public int incomeGenerated = 50;
    public float incomePercentage = 0;
    public float wavePercentage = 0;
    public float refundPercentage = 0.7f;
    public TextMeshPro incomeVisual;
    public float durationTillHide;

    [Header("Special Attributes - Minigun")]
    public float chargingTime = 2f;
    public int shotsTillHeated = 100;
    public int currentShots = 0;
    public float timeTillCooldown = 5f;
    private float o_timeTillCooldown;
    public RequirementInt[] passiveUpgrade;
    public Color idleColor;
    public Color coolingColor;
    public SpriteRenderer rend;

    [Header("Special Attributes - Village")]
    public float rangeIncrease;
    public float damageIncrease;
    public float reloadDecrease;
    public int pierceIncrease;
    public List<ExtraStats> towersNear = new List<ExtraStats>();

    [Header("Special Attributes - Knockback")]
    public float backMultipler = 2f;
    public float durationBack = 1f;

    [Header("Animation")]
    public Animator towerAnim;
    
    [HideInInspector]public float o_reloadTime;

    [Header("Firing")]
    public float angleOffset = 2f;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float immunityDuration = 2f;

    [Header("Camera Shake")]
    public float magnitude;
    public float time;

    [Header("Enemy detection")]
    public float predictionMultiplyer = 10f;
    public List<EnemyInfo> enemy = new List<EnemyInfo>();
    public GameObject lockOnEnemy;
    private int indexOfEnemy;
    public bool needsUpdate;

    private bool canShoot = true;

    void Awake()
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
        
        if(towerType == TowerType.Minigun)
        {
            canShoot = false;
            o_timeTillCooldown = timeTillCooldown;
        }
    }

    void Update()
    {
        if(isShocked)
        {
            return;
        }

        if(reloadTime > 0 && canShoot)
        {
            reloadTime -= Time.deltaTime;
        }

        if(EnemyManager.Instance.enemiesLeft > 0 && timeTillCooldown > 0 && canShoot && towerType == TowerType.Minigun && currentShots > 0)
        {
            timeTillCooldown -= Time.deltaTime;
        }
        else if(timeTillCooldown <= 0 && canShoot && towerType == TowerType.Minigun)
        {
            StartCoroutine(CoolingDown());
            timeTillCooldown = o_timeTillCooldown;
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

    void LateUpdate()
    {
        if(towerType == TowerType.Farm || towerType == TowerType.Village)
        {
            return;
        }   

        if(needsUpdate)
        {
            UpdateValues();
            SelectEnemy();
            needsUpdate = false;
        }
    }


    void FixedUpdate()
    {
        if(towerType == TowerType.Farm || towerType == TowerType.Village || isShocked)
        {
            return;
        } 

        if(lockOnEnemy == null || towerType == TowerType.Freezer)
        {
            return;
        }

        if(indexOfEnemy < 0 || indexOfEnemy >= enemy.Count || enemy[indexOfEnemy].enemy == null)
        {
            return;
        }

        Enemy trackedEnemy = enemy[indexOfEnemy].enemy;

        if(trackedEnemy.waypoint == null || trackedEnemy.waypointIndex >= trackedEnemy.waypoint.Count)
        {
            return;
        }

        Vector3 predictedPosition = Vector3.MoveTowards(trackedEnemy.transform.position, trackedEnemy.waypoint[trackedEnemy.waypointIndex], Time.deltaTime * trackedEnemy.speed * predictionMultiplyer);
        Vector3 direction = (predictedPosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed * 100f);

        float difference = Quaternion.Angle(transform.rotation, targetRotation);
        if (difference <= angleOffset && reloadTime <= 0)
        {
            reloadTime = o_reloadTime;
            Shoot();
            if(towerType == TowerType.Minigun)
            {
                currentShots++;
                timeTillCooldown = o_timeTillCooldown;
                if(currentShots >= shotsTillHeated)
                {
                    StartCoroutine(CoolingDown());
                }
            }
        }
    }


    void Shoot()
    {
        if(isShocked)
        {
            return;
        }
        
        if (towerType == TowerType.Basic || towerType == TowerType.Splash || towerType == TowerType.Minigun || towerType == TowerType.Knockback)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            bulletObj.transform.position = firePoint.position;
            bulletObj.transform.rotation = transform.rotation;

            if (bulletObj.TryGetComponent(out Rigidbody2D rb) && bulletObj.TryGetComponent(out Bullet bullet))
            {
                rb.velocity = Vector3.zero;
                bullet.damage = bulletDamage;
                bullet.pierce = bulletPierce;
                bullet.tower = this;
                if(towerType == TowerType.Splash)
                {
                    bullet.isSplash = true;
                    bullet.visualColor = splashColor;
                    bullet.explosionRadius = explosionRadius;
                    bullet.spreadTransfer = spreadTransfer;
                    bullet.visual = splashPool;
                }
                rb.AddForce(firePoint.transform.right * fireForce, ForceMode2D.Impulse);
                towerAnim.SetTrigger(Animator.StringToHash("Shoot"));

                Destroy(bulletObj, bulletLifespan);

                if(towerType == TowerType.Basic || towerType == TowerType.Minigun)
                {
                    sound.PlayClip(sound.basicShoot, 1f);
                }
                else
                {
                    sound.PlayClip(sound.splashShoot, 1f);
                }
            }
        }
        else if (towerType == TowerType.Sniper)
        {
            if (lockOnEnemy.TryGetComponent(out Enemy script))
            {
                float multiplyer = script.PI_Shield;
                if(multiplyer == 0 && script.immunities[script.cacheImmunity].immuneAgainst == towerType)
                {
                    if(PauseSystem.Instance.graphics == 1)
                    {
                        GameObject immunity = manager.immunityPool.GetFromPool();
                        immunity.transform.position = lockOnEnemy.transform.position;
                        OutsideCallPool(immunity, immunityDuration, manager.immunityPool);
                    }
                    towerAnim.SetTrigger(Animator.StringToHash("Shoot"));
                    sound.PlayClip(sound.immunity, 1f);
                    return;
                }
                else if(script.immunities[script.cacheImmunity].immuneAgainst != towerType)
                {
                    multiplyer = 1;
                }
                //If all is false then this partial immunity is for us

                if(PauseSystem.Instance.graphics == 1)
                {
                    hitLine.transform.SetParent(null);
                    float distance = Vector3.Distance(firePoint.transform.position, lockOnEnemy.transform.position);
                    hitLine.SetActive(false);
                    hitLine.transform.rotation = transform.rotation;
                    hitLine.transform.position = firePoint.position + firePoint.transform.right * distance * 0.4f;
                    hitLine.SetActive(true);
                    hitLine.transform.localScale = new Vector3(distance, hitLine.transform.localScale.y, hitLine.transform.localScale.z);
                }

                float randomCrit = Random.Range(0, 100f);
                float damage = bulletDamage;
                if (randomCrit <= criticalChance)
                {
                    damage *= criticalMultiplter;
                    script.VisualCritical(criticalColor, flashDuration, fadeDuration);

                    if(PauseSystem.Instance.graphics == 1)
                    {
                        GameObject critObj = criticalPool.GetFromPool();
                        critObj.transform.position = lockOnEnemy.transform.position + Vector3.up * 0.7f;
                        StartCoroutine(ReturnToPoolAfter(critObj, criticalVisualDuration, criticalPool));
                    }

                    sound.PlayClip(sound.criticalShoot, 1f);
                    if(PauseSystem.Instance.screenShake)
                    {
                        CameraShake.Instance.Shake(time, magnitude);
                    }
                }

                if(damage == bulletDamage)
                {
                    sound.PlayClip(sound.sniperShoot, 1f);
                }
                script.HurtEnemy(damage * multiplyer);
                script.Refresh();
                towerAnim.SetTrigger(Animator.StringToHash("Shoot"));
            }
        }
    }

    public void OutsideCallPool(GameObject obj, float delay, Pool pool)
    {
        StartCoroutine(ReturnToPoolAfter(obj, delay, pool));
    }


    IEnumerator ReturnToPoolAfter(GameObject obj, float delay, Pool pool)
    {
        yield return new WaitForSeconds(delay);
        if(pool != null && pool.storageParent.transform.childCount > 0 && obj.transform.IsChildOf(pool.storageParent.transform))
        {
            pool.ReturnToPool(obj);
        }
    }

    //Getting enemy info
    public void GatherEnemy(Enemy enemyScript)
    {
        EnemyInfo colScript = new EnemyInfo();
        colScript.enemy = enemyScript;
        enemy.Add(colScript);
        UpdateValues(); 
    }
    public void RemoveEnemy(Enemy enemyScript)
    {
        for (int i = 0; i < enemy.Count; i++)
        {
            if (enemy[i].enemy == enemyScript)
            {
                enemy.RemoveAt(i);
                break;
            }
        }

        UpdateValues();
        SelectEnemy();
    }
    public void UpdateValues()
    {
        if(towerType == TowerType.Farm || towerType == TowerType.Village)
        {
            return;
        }   
        if (enemy.Count == 0) 
        {
            return;
        }

        List<EnemyInfo> safeList = new List<EnemyInfo>(enemy);

        for (int i = 0; i < safeList.Count; i++)
        {
            EnemyInfo info = safeList[i];

            if (info.enemy != null)
            {
                info.placement = info.enemy.waypointIndex;

                if (info.enemy.waypoint != null && info.enemy.waypointIndex < info.enemy.waypoint.Count)
                {
                    info.distance = Vector2.Distance(info.enemy.transform.position, info.enemy.waypoint[info.enemy.waypointIndex]);
                }
                else
                {
                    info.distance = float.MaxValue;
                }

                info.distanceToPlayer = Vector2.Distance(info.enemy.transform.position, transform.position);
                info.health = info.enemy.health;
            }
            else
            {
                enemy.RemoveAll(e => e.enemy == null);
            }
        }
    }
    public void SelectEnemy()
    {
        if(towerType == TowerType.Farm || towerType == TowerType.Village)
        {
            return;
        }   

        lockOnEnemy = null;
        indexOfEnemy = 0;
        if(enemy.Count == 0)
        {
            return;
        }

        if(towerType == TowerType.Basic || towerType == TowerType.Sniper || towerType == TowerType.Splash || towerType == TowerType.Minigun || towerType == TowerType.Knockback)
        {
            switch(targetting)
            {
                case Targetting.First:
                    int firstEnemy = 0; 
                    float closestDistance = float.MaxValue;
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
                    int priority = 0;
                    float _closestDistance = 999;
                    int _highestPlacement = 0;
                    for(int i = 0; i < enemy.Count; i++)
                    {
                        if(enemy[i].enemy.priority > priority)
                        {
                            strongEnemy = i;
                            priority = enemy[i].enemy.priority;
                            _closestDistance = enemy[i].distance;
                            _highestPlacement = enemy[i].placement;
                        }
                        else if(enemy[i].enemy.priority == priority)
                        {
                            if(enemy[i].placement > _highestPlacement || (enemy[i].placement == _highestPlacement && enemy[i].distance < _closestDistance))
                            {
                                strongEnemy = i;
                                priority = enemy[i].enemy.priority;
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
                            if(enemy[i].placement >= highestPlacement_ || (enemy[i].placement == highestPlacement_ && enemy[i].distance < closestDistance_))
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
            bool hasFrozen = false;
            foreach(EnemyInfo script in enemy)
            {
                float random = Random.Range(0, 100);
                if(random <= freezeChance && script.enemy.immunities[script.enemy.cacheImmunity].immuneAgainst != towerType)
                {
                    hasFrozen = true;
                    script.enemy.Freeze(freezeDuration, script.enemy.o_speed * slowPercentage, cold, freeze);
                }
            }
            
            if(hasFrozen)
            {
                sound.PlayClip(sound.freeze, 1f);
            }
        }
        else if(towerType == TowerType.Debuff)
        {
            if(passiveDamage <= 0)
            {
                return;
            }

            foreach(EnemyInfo script in new List<EnemyInfo>(enemy)) //So we don't get into errors with the refresh function thingy
            {
                script.enemy.HurtEnemy(script.enemy.o_health * passiveDamage * manager.searchDelay);
                script.enemy.Refresh();
            }

        }

        if(towerType == TowerType.Minigun && lockOnEnemy != null && !canShoot)
        {
            StartCoroutine(CoolingDown());
        }
    }

    //Farm Specific
    public void GenerateMoney()
    {
        EnemyManager enemyManager = EnemyManager.Instance;
        if(wavePercentage < 0.01)
        {
            wavePercentage = 0;
        }
        if(incomePercentage < 0.01)
        {
            incomePercentage = 0;
        }

        float waveMoney = Mathf.RoundToInt(enemyManager.waveReward * (1 + (enemyManager.rewardScale * enemyManager.currentWave)) * enemyManager.coinMultipler[enemyManager.index].multiplyer);
        int finalMoney = Mathf.RoundToInt(incomeGenerated + (waveMoney * wavePercentage) + ((manager.farm.Count - 1) * incomeGenerated * incomePercentage));
        Debug.Log(finalMoney);
        Settings.Instance.money += finalMoney;
        Settings.Instance.UpdateVisual();
        incomeVisual.text = "$" + finalMoney.ToString();
        StartCoroutine(HideVisual(incomeVisual.gameObject, durationTillHide));
    }

    //Minigun Specific
    IEnumerator CoolingDown()
    {
        canShoot = false;
        reloadTime = o_reloadTime;
        rend.color = coolingColor;
        yield return new WaitForSeconds(chargingTime);
        canShoot = true;
        rend.color = idleColor;
        currentShots = 0;
        timeTillCooldown = o_timeTillCooldown;
    }

    //Village Specific
    public void CheckTowers()
    {
        LayerMask towerLayer = LayerMask.GetMask("Tower");
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, towerLayer);

        HashSet<ExtraStats> currentlyInRange = new HashSet<ExtraStats>();

        for(int i = 0; i < hits.Length; i++)
        {
            if(hits[i].gameObject.TryGetComponent(out ExtraStats stats))
            {
                currentlyInRange.Add(stats);

                if(!towersNear.Contains(stats) && !stats.isUnderRange)
                {
                    towersNear.Add(stats);
                    stats.KeepTrack();
                    stats.isUnderRange = true;
                    stats.rangeIncrease = rangeIncrease;
                    stats.damageIncrease = damageIncrease;
                    stats.reloadDecrease = reloadDecrease;
                    stats.pierceIncrease = pierceIncrease;
                    stats.ApplyToTower();
                }
                else if(towersNear.Contains(stats) && stats.isUnderRange)
                {
                    stats.isUnderRange = true;
                    stats.rangeIncrease = rangeIncrease;
                    stats.damageIncrease = damageIncrease;
                    stats.reloadDecrease = reloadDecrease;
                    stats.pierceIncrease = pierceIncrease;
                    stats.ApplyToTower();
                }
            }
        }

        for(int i = towersNear.Count - 1; i >= 0; i--)
        {
            ExtraStats tower = towersNear[i];
            if (!currentlyInRange.Contains(tower) || tower == null)
            {
                tower?.ReturnToDefaults();
                tower.isUnderRange = false;

                towersNear.RemoveAt(i);
            }
        }
    }


    public void DefaultTowers()
    {
        foreach(ExtraStats stats in towersNear)
        {
            stats.isUnderRange = false;
            stats.ReturnToDefaults();
        }
    }

    IEnumerator HideVisual(GameObject obj, float duration)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
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
            upgradeManager.debugRange.SetActive(false);
        }
    }
}
