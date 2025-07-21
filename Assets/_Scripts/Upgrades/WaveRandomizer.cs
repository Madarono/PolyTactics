using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveRandomizer : MonoBehaviour
{
    public static WaveRandomizer Instance {get; private set;} 
    public int waveCount = 5;
    public int maxWaveWeight = 250;

    [Header("For EnemyManager.cs")]
    public EnemyManager enemyManager;
    public int[] waveWeight;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetWaves();
    }

    public void SetWaves()
    {
        waveWeight = new int[waveCount];
        int sumOfWeights = 0;
        for(int i = 1; i <= waveCount; i++)
        {
            int weight = Mathf.RoundToInt((2f * i * maxWaveWeight) / (waveCount * (waveCount + 1)));
            waveWeight[i - 1] = weight;
            sumOfWeights += weight;
        }

        int difference = maxWaveWeight - sumOfWeights;
        waveWeight[waveWeight.Length - 1] += difference;

        enemyManager.waveWeight = waveWeight;
    }
}
