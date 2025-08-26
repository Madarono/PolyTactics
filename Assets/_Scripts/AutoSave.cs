using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AutoSaveManager : MonoBehaviour
{
    private void SaveGame()
    {
        Debug.Log("Game Saved!");
        DataPersistenceManager.instance.SaveGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SaveGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
