using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FactionBasedType
{
    Player,
    Enemy
}

[System.Serializable]
public class Tabs 
{
    public GameObject window;
    public Transform spawnParent;
    public Image background;
    public ManualItem[] items;

    [Header("Faction based")]
    public bool factionBased;
    public FactionBasedType factionType;

    public ManualItem[] blueItems;
    public ManualItem[] redItems;
    public ManualItem[] greenItems;
    public ManualItem[] yellowItems;
}

public class ManualSystem : MonoBehaviour
{
    public GameObject manualWindow;
    public Animator manualAnim;
    public Tabs[] tabs;
    public Color[] bgColors;
    public GameObject itemPrefab;

    void Start()
    {
        manualWindow.SetActive(false);
        MakeManual();
    }

    public void MakeManual()
    {
        foreach(Tabs tab in tabs)
        {
            if(!tab.factionBased)
            {
                for(int i = 0; i < tab.items.Length; i++)
                {
                    GameObject go = Instantiate(itemPrefab, tab.spawnParent.position, Quaternion.identity);
                    go.transform.SetParent(tab.spawnParent);
                    go.transform.localScale = Vector3.one;

                    if(go.TryGetComponent(out ManualDisplay goScript))
                    {
                        goScript.item = tab.items[i];
                        goScript.ApplyItem();
                    }
                }
            }
            else
            {
                Settings script = Settings.Instance;
                Factions faction = new Factions();
                ManualItem[] factionItems = new ManualItem[0];

                switch(tab.factionType)
                {
                    case FactionBasedType.Player:
                        faction = script.playerFaction;
                        break;

                    case FactionBasedType.Enemy:
                        faction = script.enemyFaction;
                        break;
                }

                switch(faction)
                {
                    case Factions.Circle:
                        factionItems = (ManualItem[])tab.blueItems.Clone();
                        break;

                    case Factions.Rectangle:
                        factionItems = (ManualItem[])tab.redItems.Clone();
                        break;

                    case Factions.Square:
                        factionItems = (ManualItem[])tab.greenItems.Clone();
                        break;

                    case Factions.Triangle:
                        factionItems = (ManualItem[])tab.yellowItems.Clone();
                        break;
                }

                for(int i = 0; i < factionItems.Length; i++)
                {
                    GameObject go = Instantiate(itemPrefab, tab.spawnParent.position, Quaternion.identity);
                    go.transform.SetParent(tab.spawnParent);
                    go.transform.localScale = Vector3.one;

                    if(go.TryGetComponent(out ManualDisplay goScript))
                    {
                        goScript.item = factionItems[i];
                        goScript.ApplyItem();
                    }
                }
            }
        }
    }

    public void GoToTab(int index)
    {
        foreach(Tabs tab in tabs)
        {
            tab.window.SetActive(false);
            tab.background.color = bgColors[0];
        }

        tabs[index].window.SetActive(true);
        tabs[index].background.color = bgColors[1];
    }
    
    public void OpenWindow()
    {
        manualWindow.SetActive(true);
        GoToTab(0);
    }

    public void CloseWindow()
    {
        PauseSystem script = PauseSystem.Instance;
        script.StartCoroutine(script.AnimationCloseWindow(manualAnim, manualWindow));
    }

    
}
