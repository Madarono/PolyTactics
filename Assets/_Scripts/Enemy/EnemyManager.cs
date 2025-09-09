using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Factions
{
    Square,
    Circle,
    Rectangle,
    Triangle,
    Universal,
    Neutral
}

[System.Serializable]
public class EnemyWeight
{
    public GameObject enemy;
    public int weight = 1;
    public float durationTillPut = 1f;

    [Header("Required wave")]
    public int requiredWave;
}

[System.Serializable]
public class EnemyFaction
{
    public Factions faction;
    public EnemyWeight[] enemy;
}

[System.Serializable]
public class DifficultyMultiplier
{
    public Difficulty difficulty;
    public float multiplyer;
}

public enum Difficulty
{
    Easy,
    Normal,
    Hard,
    Extreme
}

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public SoundManager soundManager;
    public Settings settings;
    public EnemyFaction[] enemy;
    public int index;
    public int enemyIndex;

    
    [Header("Wave")]
    public int currentWave;
    public int[] waveWeight;
    public int spawnLeft;
    public int enemiesLeft;
    public List<GameObject> currentEnemy = new List<GameObject>();
    public DifficultyMultiplier[] multiplyer;

    [Header("Scaling")]
    public DifficultyMultiplier[] scalingMultiplyer;
    public DifficultyMultiplier[] coinMultipler;
    public float speedScale = 0.1f;
    public float healthScale = 0.1f;
    public float immunityScale = 0.2f;

    [Header("Immunity")]
    public DifficultyMultiplier[] immunityMultipler;
    public DifficultyMultiplier[] PI_Multiplyer;

    [Header("End of wave reward")]
    public int waveReward = 100;
    public float rewardScale = 0.1f;

    [Tooltip("The enemies pending to be sent in the wave. Determined by difficulty and weight from wave")]
    public List<GameObject> enemyWave = new List<GameObject>();
    public List<float> enemyDelay = new List<float>();

    [Header("Waypoints and spawn")]
    public Transform[] waypoints;
    public Transform spawnPoint;
    public Transform enemyParent;

    [Header("For Enemy - Explosion")]
    public Pool explosivePool;
    public Color shockColor;

    private Coroutine _sendRoutine;


    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        for(int i = currentEnemy.Count - 1; i >= 0; i--)
        {
            Enemy enemyScript = null;

            if(currentEnemy[i].TryGetComponent(out Enemy enemy))
            {
                enemyScript = enemy;
            }
            else
            {
                continue;
            }

            if(waypoints.Length == 0 || enemyScript.waypointIndex == waypoints.Length)
            {
                continue;
            }

            if(enemyScript.reverseMovement)
            {
                int reverseIndex = Mathf.Max(enemyScript.waypointIndex - 1, 0);
                currentEnemy[i].transform.position = Vector3.MoveTowards(currentEnemy[i].transform.position, waypoints[reverseIndex].position, Time.deltaTime * enemyScript.speed * enemyScript.reverseMultipler);

                Vector3 direction1 = waypoints[reverseIndex].position - currentEnemy[i].transform.position;
                float angle1 = Mathf.Atan2(direction1.y, direction1.x) * Mathf.Rad2Deg;
                Quaternion rot1 = Quaternion.Euler(0f, 0f, angle1);

                enemyScript.visual.rotation = Quaternion.Lerp(enemyScript.visual.rotation, rot1, Time.deltaTime * enemyScript.rotationSpeed);

                float distance1 = Vector2.Distance(currentEnemy[i].transform.position, waypoints[reverseIndex].position);
                if((distance1 <= enemyScript.requirementDistance))
                {
                    enemyScript.waypointIndex = Mathf.Max(enemyScript.waypointIndex - 1, 0);
                }
                if (enemyScript.waypointIndex == 0 && distance1 <= enemyScript.requirementDistance)
                {
                    enemyScript.reverseMovement = false;
                }

                return;
            }

            currentEnemy[i].transform.position = Vector3.MoveTowards(currentEnemy[i].transform.position, waypoints[enemyScript.waypointIndex].position, Time.deltaTime * enemyScript.speed);

            Vector3 direction = waypoints[enemyScript.waypointIndex].position - currentEnemy[i].transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rot = Quaternion.Euler(0f, 0f, angle);

            enemyScript.visual.rotation = Quaternion.Lerp(enemyScript.visual.rotation, rot, Time.deltaTime * enemyScript.rotationSpeed);

            float distance = Vector2.Distance(currentEnemy[i].transform.position, waypoints[enemyScript.waypointIndex].position);
            if((distance <= enemyScript.requirementDistance) && enemyScript.waypointIndex <= waypoints.Length - 1)
            {
                enemyScript.waypointIndex++;

                if(enemyScript.waypointIndex == waypoints.Length)
                {
                    settings.health -= enemyScript.damageToBase;
                    settings.CheckHealth();
                    int randomIndex = Random.Range(0, soundManager.baseHit.Length);
                    soundManager.PlayClip(soundManager.baseHit[randomIndex], 1f);
                    if(PauseSystem.Instance.screenShake)
                    {
                        CameraShake.Instance.Shake(enemyScript.time, enemyScript.magnitude);
                    }
                    DestroyEnemy(currentEnemy[i]);
                }
            }
        }
    }

    public void InitiateStart()
    {
        for(int i = 0; i < multiplyer.Length; i++)
        {
            if(settings.difficulty == multiplyer[i].difficulty)
            {
                index = i;
                break;
            }
        }
        for(int i = 0; i < enemy.Length; i++)
        {
            if(settings.enemyFaction == enemy[i].faction)
            {
                enemyIndex = i;
                break;
            }
        }
    }


    public void StartWave()
    {
        SetEnemyWaves();
        if (_sendRoutine != null) StopCoroutine(_sendRoutine);
        _sendRoutine = StartCoroutine(SendWaves());
    }

    IEnumerator SendWaves()
    {
        for (int i = 0; i < enemyWave.Count; i++)
        {
            SendEnemy(enemyWave[i]);
            spawnLeft--;
            currentEnemy.Add(enemyWave[i]);

            if (i + 1 != enemyWave.Count)
                yield return new WaitForSeconds(enemyDelay[i + 1]); // timescale-aware
        }
        _sendRoutine = null;
    }


    void SetEnemyWaves()
    {
        if (WaveResources.Instance.finishedBattle)
        {
            return;
        }

        if (enemyParent.childCount > 0)
        {
            for (int i = enemyParent.childCount - 1; i >= 0; i--)
            {
                Destroy(enemyParent.GetChild(i).gameObject);
            }
        }

        enemyWave.Clear();
        enemyDelay.Clear();

        if (_sendRoutine != null)
        {
            StopCoroutine(_sendRoutine);
            _sendRoutine = null;
        }

        float weight = 0f;
        for (int i = 0; i < multiplyer.Length; i++)
        {
            if (settings.difficulty == multiplyer[i].difficulty)
            {
                weight = waveWeight[currentWave] * multiplyer[i].multiplyer;
                break;
            }
        }

        List<EnemyWeight> possibleEnemies = new List<EnemyWeight>();
        foreach (EnemyWeight ew in enemy[enemyIndex].enemy)
        {
            if (currentWave >= ew.requiredWave)
            {
                possibleEnemies.Add(ew);
            }
        }

        if (possibleEnemies.Count == 0)
        {
            Debug.LogWarning("[EnemyManager] No eligible enemies for this wave. Ending wave generation.");
            spawnLeft = 0;
            return;
        }


        int minWeight = int.MaxValue;
        foreach (var ew in possibleEnemies)
        {
            minWeight = Mathf.Min(minWeight, ew.weight);
        }


        if (weight < minWeight)
        {
            Debug.LogWarning("[EnemyManager] Remaining weight < min enemy weight, skipping wave content.");
            spawnLeft = 0;
            return;
        }

        int safety = 100000;
        while (weight >= minWeight && safety-- > 0)
        {
            List<EnemyWeight> affordable = new List<EnemyWeight>();
            foreach (var ew in possibleEnemies)
            {
                if (ew.weight <= weight)
                {
                    affordable.Add(ew);
                }
            }

            if (affordable.Count == 0)
            {
                break;
            }

            int r = Random.Range(0, affordable.Count);
            EnemyWeight choice = affordable[r];

            GameObject go = Instantiate(choice.enemy, spawnPoint.position, Quaternion.identity, enemyParent);
            go.SetActive(false);

            if (go.TryGetComponent(out Enemy goScript))
            {
                goScript.enabled = false;
            }

            enemyWave.Add(go);
            enemyDelay.Add(choice.durationTillPut);

            weight -= choice.weight;
            spawnLeft = enemyWave.Count;
        }

        if (safety <= 0)
        {
            Debug.LogError("[EnemyManager] Safety tripped while building wave. Check loop conditions.");
        }
    }

    public void SendEnemy(GameObject enemy)
    {
        if(enemy.TryGetComponent(out Enemy goScript))
        {
            goScript.enabled = true;
            goScript.SetWaypoints(waypoints);
            goScript.manager = this;
            goScript.settings = settings;
            goScript.health = goScript.health * (1 + (healthScale * currentWave)) * scalingMultiplyer[index].multiplyer;
            goScript.shieldHealth = goScript.shieldHealth * (1 + (healthScale * currentWave)) * scalingMultiplyer[index].multiplyer;
            goScript.speed = goScript.speed * (1 + (speedScale * currentWave)) * scalingMultiplyer[index].multiplyer;
            goScript.moneyReward *= Mathf.RoundToInt(coinMultipler[index].multiplyer);
        }
        enemy.SetActive(true);
    }

    public void DestroyEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        currentEnemy.Remove(enemy);
        if(spawnLeft <= 0 && currentEnemy.Count == 0)
        {
            currentEnemy.Clear();
            currentWave = Mathf.Min(currentWave + 1, waveWeight.Length);
            if(currentWave == waveWeight.Length && settings.health > 0)
            {
                WaveResources.Instance.FinishedBattle(true, true);
                return;
            }
            spawnLeft = 0;
            settings.money += Mathf.RoundToInt(waveReward * (1 + (rewardScale * currentWave)) * coinMultipler[index].multiplyer);
            settings.UpdateVisual();
            TowerManager towerManager = TowerManager.Instance;
            if(PauseSystem.Instance.autoPlay)
            {
                settings.ShowWave(1);
                soundManager.PlayClip(soundManager.endOfRound, 1f);
                towerManager.EndOfRoundChecks();
                StartWave();
            }
            else
            {
                settings.SetNormalSpeed();
            }
        }
    }


    //For enemy
    public void CallExplosiveRange(float duration, float range, float shockDuration, Transform placement)
    {
        GameObject explosion = explosivePool.GetFromPool();
        explosion.transform.position = placement.position;
        explosion.transform.localScale = new Vector3(range, range, explosion.transform.localScale.z);

        StartCoroutine(ReturnToPoolAfter(explosion, duration, explosivePool));

        //Physics
        LayerMask towerLayer = LayerMask.GetMask("Tower");
        Collider2D[] hits = Physics2D.OverlapCircleAll(placement.position, range / 2, towerLayer);
        List<Tower> towers = new List<Tower>();
        List<SpriteRenderer> towerRend = new List<SpriteRenderer>();

        foreach(Collider2D hit in hits)
        {
            if(hit.TryGetComponent(out Tower tower) && hit.TryGetComponent(out SpriteRenderer rend))
            {
                towers.Add(tower);
                tower.isShocked = true;
                tower.HideInfo();
                tower.rangeRend.enabled = false;
                rend.color = shockColor;
                towerRend.Add(rend);
            }
        }

        StartCoroutine(StopShock(towers, towerRend, shockDuration));
    }

    IEnumerator StopShock(List<Tower> towersList, List<SpriteRenderer> rend, float duration)
    {
        yield return new WaitForSeconds(duration);
        Color white = new Color(1,1,1,1);
        for(int i = 0; i < rend.Count; i++)
        {
            towersList[i].isShocked = false;
            rend[i].color = white;
        }
    }

    IEnumerator ReturnToPoolAfter(GameObject obj, float delay, Pool pool)
    {
        yield return new WaitForSeconds(delay);
        if(pool != null && pool.storageParent.transform.childCount > 0 && obj.transform.IsChildOf(pool.storageParent.transform))
        {
            pool.ReturnToPool(obj);
        }
    }

}
