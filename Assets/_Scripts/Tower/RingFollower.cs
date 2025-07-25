using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingFollower : MonoBehaviour
{
    public float speed;
    public float distanceToStop;
    public GameObject ring;
    public SpriteRenderer rend;

    public void FixedUpdate()
    {
        if(!PauseSystem.Instance.showRange)
        {
            rend.enabled = false;
            return;
        }

        rend.enabled = true;

        float distance = ring.transform.localScale.x - transform.localScale.x;
        if(distance <= distanceToStop || PauseSystem.Instance.graphics == 0)
        {
            transform.localScale = ring.transform.localScale;
            return;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, ring.transform.localScale, Time.deltaTime * speed);
    }
}
