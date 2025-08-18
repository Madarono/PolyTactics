using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public enum NewsType
{
    Conflict,
    AllianceFormed,
    AllianceBroken,
    RelationshipsFalling,
    RelationshipsReturning,
    TemporaryAllianceFormed,
    TemporaryAllianceBroken
}

[System.Serializable]
public class NewsInfo
{
    public NewsType type;
    public string info;
    public int day;
    public int iconIndex;
    public string header;
}

[System.Serializable]
public class InfoStructure
{
    public NewsType type;
    public Sprite[] icon;
    public string header;
}

public class News : MonoBehaviour, IDataPersistence
{
    public static News Instance {get; private set;}
    public GameObject windowCanvas;
    public GameObject window;
    public Animator windowAnim;
    public float duration = 0.17f;

    [Header("News")]
    public int days = 0;
    public GameObject prefab;
    public Transform parent;
    public List<NewsInfo> newsInfo = new List<NewsInfo>();
    public InfoStructure[] newsStructure;

    [Header("Presets")]
    public string[] conflictPresets;
    public string[] alliancePresets;
    public string[] brokenAlliancePresets;
    public string[] relationshipsForming;
    public string[] relationshipsFalling;
    public string[] temporaryAlliancePresets;
    public string[] brokenTemporaryAlliancePresets;

    public Color[] factionColors = new Color[4];

    public string replaceFactionA = "[FactionA]";
    public string replaceFactionB = "[FactionB]";

    [Header("Going down")]
    public GameObject goingDown;
    public ScrollRect scrollRect;
    public float goDownDuration;

    private NewsType[] savingType;
    private string[] savingInfo;
    private string[] savingHeader;
    private int[] savingIconIndex;
    private int[] savingDays;
    private bool hasRecieved = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.savingType = data.savingType;
        this.savingInfo = data.savingInfo;
        this.savingIconIndex = data.savingIconIndex;
        this.savingHeader = data.savingHeader;
        this.savingDays = data.savingDays;
        this.days = data.days;
        PutIntoLoad();
        hasRecieved = true;
    }

    public void SaveData(GameData data)
    {
        if(hasRecieved)
        {
            PutIntoSave();
            data.savingType = this.savingType;
            data.savingInfo = this.savingInfo;
            data.savingHeader = this.savingHeader;
            data.savingIconIndex = this.savingIconIndex;
            data.savingDays = this.savingDays;
            data.days = this.days;
        }
    }

    void PutIntoSave()
    {
        List<NewsType> type = new List<NewsType>();
        List<string> info = new List<string>();
        List<string> header = new List<string>();
        List<int> icon = new List<int>();
        List<int> days = new List<int>();
        
        foreach(var news in newsInfo)
        {
            type.Add(news.type);
            info.Add(news.info);
            header.Add(news.header);
            icon.Add(news.iconIndex);
            days.Add(news.day);
        }

        savingType = type.ToArray();
        savingInfo = info.ToArray();
        savingHeader = header.ToArray();
        savingIconIndex = icon.ToArray();
        savingDays = days.ToArray();
    }

    void PutIntoLoad()
    {
        for(int i = 0; i < savingType.Length; i++)
        {
            // Debug.Log($"{savingDays[i]}, {this.days}");
            if(savingDays[i] < this.days - 1)
            {
                continue;
            }

            NewsInfo info = new NewsInfo();
            info.type = savingType[i];
            info.info = savingInfo[i];
            info.iconIndex = savingIconIndex[i];
            info.header = savingHeader[i];
            info.day = savingDays[i];
            newsInfo.Add(info);
        }
    }

    void Start()
    {
        window.SetActive(false);
        windowCanvas.SetActive(false);
    }

    void Update()
    {
        bool wentDown = scrollRect.verticalNormalizedPosition <= 0.05f || parent.childCount < 5;
        goingDown.SetActive(!wentDown);
    }

    public void OpenWindow()
    {
        Time.timeScale = 0f;
        window.SetActive(true);
        windowCanvas.SetActive(true);
        UpdateWindow();
    }

    public void CloseWindow()
    {
        Time.timeScale = 1f;
        StartCoroutine(closeWindow(window, windowAnim));
    }

    void UpdateWindow()
    {
        if(parent.childCount > 0)
        {
            for(int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                Destroy(child.gameObject);
            }
        }


        for(int i = 0; i < newsInfo.Count; i++)
        {
            int index = -1;
            switch(newsInfo[i].type)
            {
                case NewsType.Conflict:
                    index = 0;
                    break;

                case NewsType.AllianceFormed:
                    index = 1;
                    break;

                case NewsType.AllianceBroken:
                    index = 2;
                    break;

                case NewsType.RelationshipsFalling:
                    index = 3;
                    break;

                case NewsType.RelationshipsReturning:
                    index = 4;
                    break;

                case NewsType.TemporaryAllianceFormed:
                    index = 5;
                    break;

                case NewsType.TemporaryAllianceBroken:
                    index = 6;
                    break;
            }
            InstantiatePrefab(newsStructure[index].icon[newsInfo[i].iconIndex], newsInfo[i].header, newsInfo[i].info);
        }
    }

    public void GoDown()
    {
        goingDown.SetActive(false);
        StartCoroutine(goDown());
    }

    //Putting News
    public string ReplaceStrings(Factions faction1, Factions faction2, string info)
    {
        string newInfo = info;
        Color faction1Color = new Color();
        Color faction2Color = new Color();

        switch(faction1)
        {
            case Factions.Circle:
                faction1Color = factionColors[0];
                break;

            case Factions.Rectangle:
                faction1Color = factionColors[1];
                break;

            case Factions.Triangle:
                faction1Color = factionColors[2];
                break;

            case Factions.Square:
                faction1Color = factionColors[3];
                break;   
        }
        switch(faction2)
        {
            case Factions.Circle:
                faction2Color = factionColors[0];
                break;

            case Factions.Rectangle:
                faction2Color = factionColors[1];
                break;

            case Factions.Triangle:
                faction2Color = factionColors[2];
                break;

            case Factions.Square:
                faction2Color = factionColors[3];
                break;   
        }

        string color = ColorToHex(faction1Color);
        string color2 = ColorToHex(faction2Color);
        
        newInfo = newInfo.Replace("[FactionA]", $"<b><color={color}>{faction1}</color></b>");
        newInfo = newInfo.Replace("[FactionB]", $"<b><color={color2}>{faction2}</color></b>");

        return newInfo;
    }

    public void PutNewInfo(NewsType type, string info, int iconIndex)
    {
        NewsInfo news = new NewsInfo();
        news.type = type;
        news.info = info;
        news.iconIndex = iconIndex;
        news.day = this.days;
        foreach(var structure in newsStructure)
        {
            if(news.type == structure.type)
            {
                news.header = $"{structure.header} - Day {days}";
                break;
            }
        }
        newsInfo.Add(news); 
    }

    void InstantiatePrefab(Sprite icon, string header, string info)
    {
        GameObject go = Instantiate(prefab, parent.position, Quaternion.identity);
        go.transform.SetParent(parent);
        go.transform.position = parent.position;
        go.transform.rotation = parent.rotation;
        go.transform.localScale = Vector3.one;

        if(go.TryGetComponent(out NewsSlot slot))
        {
            slot.Refresh(icon, header, info);
        }
    }

    IEnumerator closeWindow(GameObject window, Animator anim)
    {
        anim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(duration);
        window.SetActive(false);
        windowCanvas.SetActive(false);
    }

    IEnumerator goDown()
    {
        float t = 0;

        while(t < duration)
        {
            t += Time.unscaledDeltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(scrollRect.verticalNormalizedPosition, 0f, t / duration);
            yield return null;
        }
    }

    string ColorToHex(Color color)
    {
        Color32 c32 = color;
        return $"#{c32.r:X2}{c32.g:X2}{c32.b:X2}";
    }

}