using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffedTowerManager : MonoBehaviour
{
    public static BuffedTowerManager Instance {get; private set;}
    public List<ExtraStats> buffedTowers = new List<ExtraStats>();

    void Awake()
    {
        Instance = this;
    }

    public void SearchForAll()
    {
        TowerManager manager = TowerManager.Instance;
        foreach(var village in manager.villages)
        {
            village.CheckTowers();
        }
    }
}
