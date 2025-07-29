using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesStorage : MonoBehaviour, IDataPersistence
{
    public int coins;
    public int grain;
    public int steel;
    public int oil;
    public int uranium;
    
    public void LoadData(GameData data)
    {
        this.coins = data._coins;
        this.grain = data._grain;
        this.steel = data._steel;
        this.oil = data._oil;
        this.uranium = data._uranium;
    }

    public void SaveData(GameData data)
    {
        data._coins = this.coins;
        data._grain = this.grain;
        data._steel = this.steel;
        data._oil = this.oil;
        data._uranium = this.uranium;
    }
}