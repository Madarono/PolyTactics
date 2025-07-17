using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonListener : MonoBehaviour
{
    public AudioClip overrideClip;
    public SoundManager sound;
    public Button myButton;

    void Start()
    {
        if(myButton != null)
        {
            myButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            myButton = GetComponent<Button>();
            myButton.onClick.AddListener(OnButtonClick);
        }

        if(sound == null)
        {
            sound = SoundManager.Instance;
        }
    }

    void OnButtonClick()
    {
        if(overrideClip != null)
        {
            sound.PlayClip(overrideClip, 1f);
            return;
        }
        
        int randomIndex = Random.Range(0, sound.buttonClicks.Length);
        sound.PlayClip(sound.buttonClicks[randomIndex], 1f);
    }
}
