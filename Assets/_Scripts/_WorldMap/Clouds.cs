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

    // void Update()
    // {
    //     float multiplyer = originalSize / cam.orthographicSize; 
    //     Vector3 newScale = new Vector3(originalLocalScale.x * multiplyer, originalLocalScale.y * multiplyer, originalLocalScale.z * multiplyer);
    //     transform.localScale = Vector3.Lerp(transform.localScale, newScale, Time.deltaTime * scaleSpeed);

    //     float alpha = Mathf.InverseLerp(disappearZoom, appearZoom, cam.orthographicSize); 
    //     Color color = icon.color;
    //     color.a = baseAlpha * alpha; 
    //     icon.color = color;
    //     icon.enabled = alpha == 0 ? false : true;

    //     transform.position = Vector3.MoveTowards(transform.position, destination.position, Time.deltaTime * speed);
    //     if(Vector2.Distance(transform.position, destination.position) <= 0f)
    //     {
    //         gameObject.SetActive(false);
    //     }
    // }
}
