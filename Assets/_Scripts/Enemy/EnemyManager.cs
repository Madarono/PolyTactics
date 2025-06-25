using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Factions
{
    Square,
    Circle,
    Rectangle,
    Triangle
}

[System.Serializable]
public class EnemyFaction
{
    public Factions faction;
    public GameObject normal;
}

public class EnemyManager : MonoBehaviour
{
    public Factions enemyFaction;
    public EnemyFaction[] enemy;
    private int index;

    public Transform[] waypoints;
    public Transform spawnPoint;

    void Start()
    {
        for(int i = 0; i < enemy.Length; i++)
        {
            if(enemyFaction == enemy[i].faction)
            {
                index = i;
                break;
            }
        }
        
        SendEnemy();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            SendEnemy();
        }
    }

    public void SendEnemy()
    {
        GameObject go = Instantiate(enemy[index].normal , spawnPoint.position, Quaternion.identity);
        Enemy goScript = go.GetComponent<Enemy>();
        List<Vector3> points = new List<Vector3>();
        foreach(Transform trans in waypoints)
        {
            points.Add(trans.position);
        }
        goScript.SetWaypoints(points.ToArray());
    }

}
