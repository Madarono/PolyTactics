using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleInformation
{
    public int battleNumber;
    public string faction;
    public int dayNumber;
    public string endTalk;
}

public class BattleLog : MonoBehaviour, IDataPersistence
{
    [Header("New Game")]
    public GameObject window;
    public Animator anim;
    public GameObject canvas;
    public float closeDuration = 0.17f;

    [Header("Visual")]
    public GameObject logPrefab;
    public Transform logParent;
    public Color circle;
    public Color rectangle;
    public Color triangle;
    public Color square;
    public Sprite[] icons;
    public List<BattleInformation> info;

    public void LoadData(GameData data)
    {
        for (int i = 0; i < data.battleNumber.Length; i++)
        {
            BattleInformation battleInfo = new BattleInformation();
            battleInfo.battleNumber = data.battleNumber[i];
            battleInfo.faction = data.faction[i];
            battleInfo.dayNumber = data.dayNumber[i];
            battleInfo.endTalk = data.endTalk[i];
            info.Add(battleInfo);
        }

        LoadInfo();
    }

    public void SaveData(GameData data)
    {
        
    }

    void LoadInfo()
    {
        if (logParent.childCount > 0)
        {
            for (int i = logParent.childCount - 1; i >= 0; i--)
            {
                Transform child = logParent.GetChild(i);
                Destroy(child.gameObject);
            }
        }
        
        foreach (var info in info)
        {
            GameObject go = Instantiate(logPrefab, logParent.position, Quaternion.identity);
            go.transform.SetParent(logParent);
            go.transform.localScale = Vector3.one;
            if (go.TryGetComponent(out Log log))
            {
                log.Refresh(info.battleNumber, info.faction, EndColor(info.faction), info.dayNumber, info.endTalk, IndexIcon(info.faction));
            }
        }
    }

    void Start()
    {
        window.SetActive(false);
        canvas.SetActive(false);
    }

    public void OpenWindow()
    {
        window.SetActive(true);
        canvas.SetActive(true);
    }

    public void CloseWindow()
    {
        StartCoroutine(closeWindow(window, canvas, anim, true));
    }

    IEnumerator closeWindow(GameObject window, GameObject canvas, Animator animator, bool closeCanvas)
    {
        animator.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(closeDuration);
        window.SetActive(false);

        canvas.SetActive(!closeCanvas);
    }

    string EndColor(string faction)
    {
        Color color = new Color();
        switch (faction)
        {
            case "Circle Faction":
                color = circle;
                break;

            case "Rectangle Faction":
                color = rectangle;
                break;

            case "Triangle Faction":
                color = triangle;
                break;

            case "Square Faction":
                color = square;
                break;
        }

        return ColorToHex(color);
    }

    Sprite IndexIcon(string faction)
    {
        Sprite endIcon = icons[0];
        switch (faction)
        {
            case "Circle Faction":
                endIcon = icons[0];
                break;

            case "Rectangle Faction":
                endIcon = icons[1];
                break;

            case "Triangle Faction":
                endIcon = icons[2];
                break;

            case "Square Faction":
                endIcon = icons[3];
                break;
        }

        return endIcon;
    }

    string ColorToHex(Color color)
    {
        Color32 c32 = color;
        return $"#{c32.r:X2}{c32.g:X2}{c32.b:X2}";
    }
}
