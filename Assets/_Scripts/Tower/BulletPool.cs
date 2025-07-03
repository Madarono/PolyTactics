using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Pool pool;

    public void GoToPool(float duration)
    {
        Invoke("Pool", duration);
    }

    void Pool()
    {
        pool.ReturnToPool(gameObject);
    }
}