using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveRandomizer : MonoBehaviour, IDataPersistence
{
    public static WaveRandomizer Instance {get; private set;} 
    public int waveCount = 5;
    public int maxWaveWeight = 250;

    [Header("Debug purposes")]
    public bool saveToData = false;

    [Header("For EnemyManager.cs")]
    public int[] waveWeight;

    public void LoadData(GameData data)
    {
        this.waveCount = data.waveCount;
        this.maxWaveWeight = data.maxWaveWeight;
        SetWaves();
        Settings.Instance.ShowWave(0);
    }

    public void SaveData(GameData data)
    {
        if(saveToData)
        {
            data.waveCount = this.waveCount;
            data.maxWaveWeight = this.maxWaveWeight;
        }
    }

    void Awake()
    {
        Instance = this;
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

        EnemyManager.Instance.waveWeight = waveWeight;
    }
}
