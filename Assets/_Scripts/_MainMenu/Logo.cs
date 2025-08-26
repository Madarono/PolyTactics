using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    public Sprite[] logoSprites;
    public int numberIndex = 0;
    public Image image;
    public float delayChange = 2f;

    void Start()
    {
        StartCoroutine(ChangeLogo());
    }

    IEnumerator ChangeLogo()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(delayChange);
            numberIndex++;
            if (numberIndex >= logoSprites.Length)
            {
                numberIndex = 0;
            }
            image.sprite = logoSprites[numberIndex];
        }
    }
}
