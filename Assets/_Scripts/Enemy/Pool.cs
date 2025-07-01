using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Pool : MonoBehaviour
{
    public GameObject prefab;
    public Transform storageParent;
    public int initialSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(storageParent);
        pool.Enqueue(obj);
        return obj;
    }

    public GameObject GetFromPool()
    {
        if (pool.Count == 0)
        {
            CreateNewObject();
        }

        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
