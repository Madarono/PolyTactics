using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualFaction : MonoBehaviour
{
    public Image img;
    public Sprite[] sprites;

    public void ChangeSprite(int index)
    {
        img.sprite = sprites[index];
    }
}