using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clouds : MonoBehaviour
{
    public Image icon;
    public float speed;
    public float scaleSpeed = 4f;
    public float disappearZoom = 3f;
    public float appearZoom = 5f;
    public Vector3 originalLocalScale;
    public float originalSize;
    public float baseAlpha;

    void Start()
    {
        originalLocalScale = transform.localScale;
        baseAlpha = icon.color.a;
    }
}
