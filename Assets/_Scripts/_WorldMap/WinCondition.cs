using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    public static WinCondition Instance { get; private set; }
    private Alliances alliances;
    public Tilemap ground;

    [Header("Requirements")]
    private int tilesRequired;
    public float percentageRequired = 80f;
    public int alliancesRequired = 3;

    [Header("Progress")]
    public float percentageTiles;
    public int alliancesMade;

    [Header("Visual")]
    public bool hasWon;
    public GameObject winButton;
    public GameObject window;
    public GameObject loseWindow;
    public Animator windowAnim;
    public float windowCloseDuration = 0.17f;
    public GameObject canvas;

    [Header("Transition")]
    public GameObject blackTransition;
    public float sceneLeaveDuration;

    [Header("Debug")]
    public bool debug;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        window.SetActive(false);
        canvas.SetActive(false);
        alliances = Alliances.Instance;
    }

    void Update()
    {
        if (!debug)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            hasWon = false;
            OpenWindow();
        }
    }

    public void CalculateWinRate()
    {
        BoundsInt bounds = ground.cellBounds;
        int totalTiles = 0;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (ground.GetTile(pos))
                {
                    totalTiles++;
                }
            }
        }

        tilesRequired = Mathf.FloorToInt((float)totalTiles * 0.8f);

        FactionPower power = FactionPower.Instance;

        int strength = 0;

        Factions playerFaction = InteractionSystem.Instance.playerFaction;

        switch (playerFaction)
        {
            case Factions.Circle:
                strength = power.factionStrength[0].strength;
                break;

            case Factions.Rectangle:
                strength = power.factionStrength[1].strength;
                break;

            case Factions.Triangle:
                strength = power.factionStrength[2].strength;
                break;

            case Factions.Square:
                strength = power.factionStrength[3].strength;
                break;
        }

        float percentage = Mathf.Round(((float)strength / tilesRequired) * 10000f) / 100;
        percentageTiles = percentage;

        FactionAlliance[] factionAlliances = alliances.factionAlliances;
        int numberOfAlliances = 0;

        foreach (var alliance in factionAlliances)
        {
            if (alliance.isUnderAlliance && alliance.faction != playerFaction)
            {
                numberOfAlliances++;
            }
        }

        Debug.Log(numberOfAlliances);
        if (numberOfAlliances >= alliancesRequired || percentageTiles >= percentageRequired)
        {
            hasWon = true;
        }
        else
        {
            hasWon = false;
        }


        Refresh();
    }

    void Refresh()
    {
        winButton.SetActive(hasWon);
        
        if (percentageTiles <= 0) //hasWon is already false, since percentageTiles is 0; not over 80 percent
        {
            OpenWindow();
        }
    }

    public void OpenWindow()
    {
        if (hasWon)
        {
            window.SetActive(true);
        }
        else
        {
            loseWindow.SetActive(true);
        }
        canvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseWindow()
    {
        Time.timeScale = 1f;
        StartCoroutine(closeWindow(window, windowAnim, true));
    }

    public void ConfirmLeave()
    {
        StartCoroutine(LeaveGame());
    }

    IEnumerator closeWindow(GameObject window, Animator windowAnim, bool closeCanvas)
    {
        windowAnim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(windowCloseDuration);
        window.SetActive(false);

        canvas.SetActive(!closeCanvas);
    }

    IEnumerator LeaveGame() //Check if won or not here using a bool
    {
        blackTransition.SetActive(true);
        yield return new WaitForSecondsRealtime(sceneLeaveDuration);
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
}