using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public bool screenShake;
    public int graphics;
    public float master;
    public float background;

    public GameData()
    {   
        this.screenShake = true;
        this.graphics = 1;
        this.master = 1;
        this.background = 1;
    }
}
