using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseSystem : MonoBehaviour, IDataPersistence
{
    public static PauseSystem Instance { get; private set; }

    public GameObject window;

    [Header("Modifications")]
    public int graphics = 1;
    public bool screenShake = true;
    public bool autoPlay = false;
    public bool showRange = true;
    public Slider masterSlider;
    public float master;
    public Slider backgroundSlider;
    public float background;

    public float amplifier = 1.5f;

    [Header("Animations")]
    public Animator windowAnim;
    public float closeDuration = 0.5f;
    public Animator leaveWindowAnim;

    [Header("Visuals")]
    public Image[] graphicButtons;
    public Color[] graphicsStates = new Color[2];
    public GameObject screenShakeTick;
    public GameObject autoPlayTick;
    public GameObject showRangeTick;
    public GameObject postprocessing;

    [Header("Scripts")]
    public Settings settings;
    public SoundManager soundManager;
    public bool canSound = true;
    public float cooldownSound = 0.1f;

    [Header("Leaving")]
    public GameObject leaveWindow;

    public void LoadData(GameData data)
    {
        this.graphics = data.graphics;
        this.screenShake = data.screenShake;
        this.master = data.master;
        this.background = data.background;
        this.autoPlay = data.autoPlay;
        this.showRange = data.showRange;
        window.SetActive(false);
        leaveWindow.SetActive(false);
        Refresh();
    }

    public void SaveData(GameData data)
    {
        data.graphics = this.graphics;
        data.screenShake = this.screenShake;
        data.master = this.master;
        data.background = this.background;
        data.autoPlay = this.autoPlay;
        data.showRange = this.showRange;
    }
    
    void Awake()
    {
        Instance = this;
    } 
    
    public void OpenWindow()
    {
        canSound = true;
        window.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseWindow()
    {
        canSound = false;
        Time.timeScale = settings.isSpeeding ? settings.speedValue : 1f;
        StartCoroutine(AnimationCloseWindow(windowAnim, window));
    }

    public IEnumerator AnimationCloseWindow(Animator anim, GameObject win)
    {
        anim.SetTrigger("Close");
        yield return new WaitForSecondsRealtime(closeDuration);
        win.SetActive(false);
    }

    void Refresh()
    {
        StartCoroutine(Cooldown());
        masterSlider.value = master;
        backgroundSlider.value = background;
        soundManager.masterVolume = this.master;
        soundManager.backgroundVolume = this.background;
        screenShakeTick.SetActive(screenShake);
        autoPlayTick.SetActive(autoPlay);
        showRangeTick.SetActive(showRange);

        if(graphics == 1)
        {
            postprocessing.SetActive(true);
        }
        else
        {
            postprocessing.SetActive(false);
        }

        if(graphicButtons.Length == 0)
        {
            return;
        }

        foreach(Image img in graphicButtons)
        {
            img.color = graphicsStates[0];
            img.raycastTarget = false;
        }
        graphicButtons[graphics].color = graphicsStates[1];
    }

    //Graphics
    public void LowGraphics()
    {
        ChangeGraphics(0);
    }
    public void HighGraphics()
    {
        ChangeGraphics(1);
    }

    void ChangeGraphics(int index)
    {
        graphics = index;
        if(graphics == 1)
        {
            postprocessing.SetActive(true);
        }
        else
        {
            postprocessing.SetActive(false);
        }
        DataPersistenceManager.instance.SaveGame();
        
        if(graphicButtons.Length == 0)
        {
            return;
        }

        foreach(Image img in graphicButtons)
        {
            img.color = graphicsStates[0];
            img.raycastTarget = false;
        }
        graphicButtons[index].color = graphicsStates[1];
    }

    //Audio
    public void UpdateValues(bool isMaster)
    {
        master = masterSlider.value;
        background = backgroundSlider.value;
        soundManager.masterVolume = this.master;
        soundManager.backgroundVolume = this.background;
        if(canSound)
        {
            soundManager.PlayClip(soundManager.changingSliders, isMaster ? master * amplifier: background * amplifier);
            StartCoroutine(Cooldown());
        }
        DataPersistenceManager.instance.SaveGame();
    }

    IEnumerator Cooldown()
    {
        canSound = false;
        yield return new WaitForSecondsRealtime(cooldownSound);
        canSound = true;
    }

    //Screen shake
    public void ChangeCameraShake()
    {
        screenShake = !screenShake;
        screenShakeTick.SetActive(screenShake);
        DataPersistenceManager.instance.SaveGame();
    }

    //Auto-Play
    public void ChangeAutoPlay()
    {
        autoPlay = !autoPlay;
        autoPlayTick.SetActive(autoPlay);
        DataPersistenceManager.instance.SaveGame();
    }

    //ShowRange
    public void ChangeShowRange()
    {
        showRange = !showRange;
        showRangeTick.SetActive(showRange);
        DataPersistenceManager.instance.SaveGame();
    }

    //Leaving battle
    public void OpenLeaveWindow()
    {
        leaveWindow.SetActive(true);
    }
    
    public void CloseLeaveWindow()
    {
        StartCoroutine(AnimationCloseWindow(leaveWindowAnim, leaveWindow));
    }

    public void ConfirmLeave()
    {
        Debug.Log("Go to world map and lose this battle");
    }
}
