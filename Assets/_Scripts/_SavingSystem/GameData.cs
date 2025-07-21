using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    //PauseSystem.cs
    public bool screenShake;
    public bool autoPlay;
    public int graphics;
    public float master;
    public float background;

    public GameData()
    {   
        this.screenShake = true;
        this.autoPlay = false;
        this.graphics = 1;
        this.master = 1;
        this.background = 1;
    }
}
