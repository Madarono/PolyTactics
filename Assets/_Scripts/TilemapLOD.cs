using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapLOD : MonoBehaviour
{
    public Camera cam;
    public float fadeStart = 8f; //Zoom size where fade begins
    public float fadeEnd = 12f; //Zoom size where it’s fully invisible
    public TilemapRenderer blue;
    public TilemapRenderer red;
    public TilemapRenderer yellow;
    public TilemapRenderer green;
    public TilemapRenderer glow;
    public TilemapRenderer shadow;
    public TilemapRenderer land;
    public SpriteRenderer LOD;

    private Color baseColor;

    void Start()
    {
        baseColor = green.material.color;
    }

    void Update()
    {
        float size = cam.orthographicSize;
        float alpha = Mathf.InverseLerp(fadeEnd, fadeStart, size);
        Color color = baseColor;
        color.a = alpha;
        blue.material.color = color;
        red.material.color = color;
        yellow.material.color = color;
        green.material.color = color;
        glow.material.color = color;
        shadow.material.color = color;

        bool isZero = alpha == 0;

        blue.enabled = !isZero;
        red.enabled = !isZero;
        yellow.enabled = !isZero;
        green.enabled = !isZero;
        glow.enabled = !isZero;
        shadow.enabled = !isZero;
        land.enabled = !isZero;
        LOD.enabled = isZero;
    }
}
