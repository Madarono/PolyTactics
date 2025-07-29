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

    //WaveResources.cs
    public int coins;
    public int grain;
    public int steel;
    public int oil;
    public int uranium;

    //ResourcesStorage.cs
    public int _coins;
    public int _grain;
    public int _steel;
    public int _oil;
    public int _uranium;

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

        //WaveResources.cs
        this.coins = 0;
        this.grain = 0;
        this.steel = 0;
        this.oil = 0;
        this.uranium = 0;

        //ResourcesStorage.cs
        this._coins = 100;
        this._grain = 0;
        this._steel = 0;
        this._oil = 0;
        this._uranium = 0;
    }
}
