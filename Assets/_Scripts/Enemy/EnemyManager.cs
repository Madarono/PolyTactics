using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Factions
{
    Square,
    Circle,
    Rectangle,
    Triangle
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
    public Settings settings;
    public EnemyFaction[] enemy;
    private int index;

    
    [Header("Wave")]
    public int currentWave;
    public int[] waveWeight;
    private int spawnLeft;
    public int enemiesLeft;
    public DifficultyMultiplier[] multiplyer;

    [Header("Scaling")]
    public DifficultyMultiplier[] scalingMultiplyer;
    public DifficultyMultiplier[] coinMultipler;
    public float speedScale = 0.1f;
    public float healthScale = 0.1f;

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

    public void StartWave()
    {
        SetEnemyWaves();
        StartCoroutine(SendWaves());
    }

    void SetEnemyWaves()
    {
        enemyWave.Clear();
        enemyDelay.Clear();
        StopAllCoroutines();
        
        float weight = 0;

        for(int i = 0; i < multiplyer.Length; i++)
        {
            if(settings.difficulty == multiplyer[i].difficulty)
            {
                weight = waveWeight[currentWave] * multiplyer[i].multiplyer;
                break;
            }
        }

        List<EnemyWeight> possibleEnemies = new List<EnemyWeight>();
        foreach(EnemyWeight enemyWeight in enemy[index].enemy)
        {
            if(currentWave >= enemyWeight.requiredWave)
            {
                possibleEnemies.Add(enemyWeight);
            }
        }

        while(weight > 0)
        {
            int random = Random.Range(0, possibleEnemies.Count);
            if(weight >= possibleEnemies[random].weight)
            {
                weight -= possibleEnemies[random].weight;
                GameObject go = Instantiate(possibleEnemies[random].enemy, spawnPoint.position, Quaternion.identity);
                go.transform.SetParent(enemyParent);
                go.SetActive(false);
                enemyWave.Add(go);
                enemyDelay.Add(possibleEnemies[random].durationTillPut);
            }
            else
            {
                for(int i = 0; i < possibleEnemies.Count; i++)
                {
                    if(i == random)
                    {
                        continue;
                    }

                    if(weight >= possibleEnemies[i].weight)
                    {
                        weight -= possibleEnemies[i].weight;
                        GameObject go = Instantiate(possibleEnemies[i].enemy, spawnPoint.position, Quaternion.identity);
                        go.transform.SetParent(enemyParent);
                        enemyWave.Add(go);
                        enemyDelay.Add(possibleEnemies[i].durationTillPut);
                        break;
                    }
                }
            }

            spawnLeft = enemyWave.Count;
        }
    }

    IEnumerator SendWaves()
    {
        for(int i = 0; i < enemyWave.Count; i++)
        {
            SendEnemy(enemyWave[i]);
            spawnLeft--;
            enemiesLeft++;
            if(i + 1 != enemyWave.Count)
            {
                yield return new WaitForSeconds(enemyDelay[i + 1]);
            }
        }
    }

    public void SendEnemy(GameObject enemy)
    {
        enemy.SetActive(true);
        List<Vector3> points = new List<Vector3>();
        foreach(Transform trans in waypoints)
        {
            points.Add(trans.position);
        }
        if(enemy.TryGetComponent(out Enemy goScript))
        {
            goScript.SetWaypoints(points.ToArray());
            goScript.manager = this;
            goScript.settings = settings;
            goScript.health += goScript.health * (1 + (healthScale * currentWave)) * scalingMultiplyer[index].multiplyer;
            goScript.speed += goScript.speed * (1 + (speedScale * currentWave)) * scalingMultiplyer[index].multiplyer;
            goScript.moneyReward += Mathf.RoundToInt(coinMultipler[index].multiplyer);
        }
    }

    public void DestroyEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
        enemiesLeft--;
        if(enemiesLeft <= 0 && spawnLeft <= 0)
        {
            currentWave++;
            settings.money += Mathf.RoundToInt(waveReward * (1 + (rewardScale * currentWave)) * coinMultipler[index].multiplyer);
            settings.UpdateVisual();
            settings.SetNormalSpeed();
        }
    }

}
