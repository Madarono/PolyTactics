using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trust : MonoBehaviour, IDataPersistence
{
    public static Trust Instance {get; private set;}
    [Header("Trust to player")]
    public int circleTrust;
    public int rectangleTrust;
    public int triangleTrust;
    public int squareTrust;

    private bool hasRecieved = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.circleTrust = data.circleTrust;
        this.rectangleTrust = data.rectangleTrust;
        this.triangleTrust = data.triangleTrust;
        this.squareTrust = data.squareTrust;
        hasRecieved = true;
    }

    public void SaveData(GameData data)
    {
        if(hasRecieved)
        {
            data.circleTrust = this.circleTrust;
            data.rectangleTrust = this.rectangleTrust;
            data.triangleTrust = this.triangleTrust;
            data.squareTrust = this.squareTrust;
        }
    }
}
