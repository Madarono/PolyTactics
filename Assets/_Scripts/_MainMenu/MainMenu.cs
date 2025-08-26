using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class FactionOptions
{
    public Image image;
    public Factions faction;
}

[System.Serializable]
public class WorldOptions
{
    public Image image;
    public int worldSize;
}

[System.Serializable]
public class DifficultyOptions
{
    public Image image;
    public Difficulty difficulty;
}

public class MainMenu : MonoBehaviour, IDataPersistence
{
    public static MainMenu Instance { get; private set; }
    public Image continueForeground;
    public Button continueButton;
    public Color enableContinue;
    public Color disableContinue;

    [Header("New Game")]
    public GameObject gameWindow;
    public Animator gameAnimator;
    public GameObject gameCanvas;
    public float closeDuration = 0.17f;

    public FactionOptions[] factionOptions;
    public Factions faction;
    public WorldOptions[] worldOptions;
    public int worldSize;
    public DifficultyOptions[] difficultyOptions;
    public Difficulty difficulty;

    [Header("Ready Game")]
    public GameObject readyWindow;
    public Animator readyAnimator;
    public TextMeshProUGUI readyVisual;
    public GameObject leaveTransition;
    public float leaveDuration = 1.5f;

    public int lastOptionFaction;
    public int lastOptionWorld;
    public int lastOptionDifficulty;

    public bool hasMadeGame;
    public bool canSave;

    void Awake()
    {
        Time.timeScale = 1f;
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.hasMadeGame = data.hasMadeGame;
        Refresh();
    }

    public void SaveData(GameData data)
    {
        if (canSave)
        {
            data.playerFaction = this.faction;
            data.width = this.worldSize;
            data.height = this.worldSize;
            data.seed = 0;
            data.difficulty = this.difficulty;
        }
    }

    void Refresh()
    {
        lastOptionDifficulty = -1;
        lastOptionFaction = -1;
        lastOptionWorld = -1;
        if (hasMadeGame)
        {
            continueButton.interactable = true;
            continueForeground.color = enableContinue;
        }
        else
        {
            continueButton.interactable = false;
            continueForeground.color = disableContinue;
        }
    }

    void Start()
    {
        gameWindow.SetActive(false);
        gameCanvas.SetActive(false);
        readyWindow.SetActive(false);
    }

    public void OpenNewGameWindow()
    {
        gameWindow.SetActive(true);
        gameCanvas.SetActive(true);
    }

    public void CloseNewGameWindow()
    {
        StartCoroutine(closeWindow(gameWindow, gameCanvas, gameAnimator, true));
    }

    public void ChangeFactionOptions(int index)
    {
        if (lastOptionFaction != -1)
        {
            factionOptions[lastOptionFaction].image.color = enableContinue;
        }

        factionOptions[index].image.color = disableContinue;
        faction = factionOptions[index].faction;
        lastOptionFaction = index;
    }

    public void ChangeWorldOptions(int index)
    {
        if (lastOptionWorld != -1)
        {
            worldOptions[lastOptionWorld].image.color = enableContinue;
        }

        worldOptions[index].image.color = disableContinue;
        worldSize = worldOptions[index].worldSize;
        lastOptionWorld = index;
    }

    public void ChangeDifficultyOptions(int index)
    {
        if (lastOptionDifficulty != -1)
        {
            difficultyOptions[lastOptionDifficulty].image.color = enableContinue;
        }
        difficultyOptions[index].image.color = disableContinue;
        difficulty = difficultyOptions[index].difficulty;
        lastOptionDifficulty = index;
    }

    public void OpenReadyWindow()
    {
        if (lastOptionDifficulty == -1 || lastOptionFaction == -1 || lastOptionWorld == -1)
        {
            return;
        }

        readyWindow.SetActive(true);
        readyVisual.text = hasMadeGame ? "(This will override the existing world)" : "(This will create a new world)";
    }

    public void CloseReadyWindow()
    {
        StartCoroutine(closeWindow(readyWindow, gameCanvas, readyAnimator, false));
    }

    public void MakeGame()
    {
        StartCoroutine(MakeNewGame());
    }

    public void ContinueMap()
    {
        StartCoroutine(Continue());
    }

    IEnumerator closeWindow(GameObject window, GameObject canvas, Animator animator, bool closeCanvas)
    {
        animator.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(closeDuration);
        window.SetActive(false);

        canvas.SetActive(!closeCanvas);
    }

    IEnumerator MakeNewGame()
    {
        leaveTransition.SetActive(true);
        MusicLoader.Instance.CallFadeOut();
        yield return new WaitForSecondsRealtime(leaveDuration);
        DataPersistenceManager.instance.NewWorld();
        canSave = true;
        DataPersistenceManager.instance.SaveGame();
        LoadSceneAsync("WorldMap");
    }

    IEnumerator Continue()
    {
        leaveTransition.SetActive(true);
        MusicLoader.Instance.CallFadeOut();
        yield return new WaitForSecondsRealtime(leaveDuration);
        DataPersistenceManager.instance.SaveGame();
        LoadSceneAsync("WorldMap");
    }

    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            //Make loading here or smth
            yield return null;
        }
    }

}
