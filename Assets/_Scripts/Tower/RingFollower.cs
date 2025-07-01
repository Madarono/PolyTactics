using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingFollower : MonoBehaviour
{
    public float speed;
    public float distanceToStop;
    public GameObject ring;

    public void FixedUpdate()
    {
        float distance = ring.transform.localScale.x - transform.localScale.x;
        if(distance <= distanceToStop)
        {
            transform.localScale = ring.transform.localScale;
            return;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, ring.transform.localScale, Time.deltaTime * speed);
    }
}
