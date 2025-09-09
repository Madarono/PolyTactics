using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LoadMusic : MonoBehaviour, IDataPersistence
{
    public void LoadData(GameData data)
    {
        StartCoroutine(WaitForMusic());
    }

    public void SaveData(GameData data)
    {
        
    }

    IEnumerator WaitForMusic()
    {
        yield return null;
        MusicLoader.Instance.InstantiateStart();
    }
}