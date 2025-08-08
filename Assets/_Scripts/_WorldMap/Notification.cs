using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour
{
    public static Notification Instance {get; private set;}
    public GameObject notificationWindow;
    public TextMeshProUGUI notificationVisual;
    public float durationOfAnimation = 2.5f;

    void Awake()
    {
        Instance = this;
    }

    public void PutNotification(string information)
    {
        StopAllCoroutines();
        notificationWindow.SetActive(false);
        notificationVisual.text = information;
        StartCoroutine(PutNoti());
    }

    IEnumerator PutNoti()
    {
        notificationWindow.SetActive(true);
        yield return new WaitForSeconds(durationOfAnimation);
        notificationWindow.SetActive(false);
    }
}
