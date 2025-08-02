using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveTransition : MonoBehaviour
{
    public GameObject transition;
    public float duration;

    void Start()
    {
        transition.SetActive(true);
        StartCoroutine(Remove());
    }

    IEnumerator Remove() //Disables the Transition obj after a while
    {
        yield return new WaitForSeconds(duration);
        transition.SetActive(false);
    }
}
