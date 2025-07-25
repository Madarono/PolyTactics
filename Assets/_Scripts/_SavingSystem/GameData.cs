using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    //PauseSystem.cs
    public bool screenShake;
    public bool autoPlay;
    public bool showRange;
    public int graphics;
    public float master;
    public float background;

    //BattleMaps.cs
    public int mapIndex;
    public bool randomizeMapIndex;

    //WaveRandomizer.cs
    public int waveCount;
    public int maxWaveWeight;

    //Settings.cs
    public Difficulty difficulty;
    public Factions enemyFaction;
    public Factions playerFaction;

    public GameData()
    {   
        //PauseSystem.cs
        this.screenShake = true;
        this.autoPlay = false;
        this.showRange = true;
        this.graphics = 1;
        this.master = 1;
        this.background = 1;

        //BattleMaps.cs
        this.mapIndex = 0;
        this.randomizeMapIndex = true;

        //WaveRandomizer.cs
        this.waveCount = 7;
        this.maxWaveWeight = 400;

        //Settings.cs
        this.difficulty = Difficulty.Normal;
        this.playerFaction = Factions.Circle;
        this.enemyFaction = Factions.Square;
    }
}
