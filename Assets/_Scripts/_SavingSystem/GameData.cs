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

    //WaveRandomizer.cs -- Save this
    public int waveCount;
    public int maxWaveWeight;

    //Settings.cs
    public Difficulty difficulty;
    public Factions enemyFaction;
    public Factions playerFaction;

    //WaveResources.cs -- Save this
    public int coins;
    public int grain;
    public int steel;
    public int oil;
    public int uranium;
    public bool hasWon;
    public bool makeWarsHappen;

    //ResourcesStorage.cs
    public int _coins;
    public int _grain;
    public int _steel;
    public int _oil;
    public int _uranium;

    //InteractionSystem.cs
    public int a_waves;
    public int a_waveWeight;
    public int a_coins;
    public int a_grains;
    public int a_steel;
    public int a_oil;
    public int a_uranium;
    public float resourceMultiplyer;
    public int[] levelPlace = new int[3];
    public int[] slotIndex = new int[0];
    public bool checkedPlayer;
    public bool hasCheckedNews;

    //LandConquerer.cs
    public int[] landPlaces = new int[0];
    public Factions[] landFaction = new Factions[0];

    //Inventory
    public int[] towerIndex;

    //PerlinNoise.cs
    public int seed;
    public int width;
    public int height;

    //Relationships.cs
    public int[] circleRelationPoints;
    public int[] rectangleRelationPoints;
    public int[] triangleRelationPoints;
    public int[] squareRelationPoints;

    //FactionConquer.cs
    public int[] glowInt;
    public int[] glowPlayerInt;

    //FactionPower.cs
    public int[] strength;

    //Trust.cs
    public int circleTrust;
    public int rectangleTrust;
    public int triangleTrust;
    public int squareTrust;

    //Trading.cs
    public int reqIndex;
    public int inviteChance;
    
    //Alliances.cs
    public bool[] isUnderAlliance;
    public Factions[] factionsA;
    public Factions[] factionsB;
    public Factions[] factionsC;
    public bool[] isUnderTemp1;
    public int[] turnsLeft1;
    public bool[] isUnderTemp2; 
    public int[] turnsLeft2;
    public bool[] isUnderTemp3; 
    public int[] turnsLeft3;
    public bool[] isUnderTemp4; 
    public int[] turnsLeft4;
    public int turnsTillattempt;

    //AIFights.cs
    public int[] placesInt;
    public int[] lastFight;
    public int[] strength_fights;
    public int[] relationIndex;

    //News.cs
    public NewsType[] savingType;
    public string[] savingInfo;
    public string[] savingHeader;
    public int[] savingIconIndex;
    public int[] savingDays;
    public int days;

    //Shop.cs
    public int[] shopSaved;

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
        this.waveCount = 1;
        this.maxWaveWeight = 1;

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
        this.hasWon = false;
        this.makeWarsHappen = false;

        //ResourcesStorage.cs
        this._coins = 100;
        this._grain = 0;
        this._steel = 0;
        this._oil = 0;
        this._uranium = 0;

        //InteractionSystem.cs
        this.a_coins = 10;
        this.a_grains = 5;
        this.a_steel = 3;
        this.a_oil = 2;
        this.a_uranium = 1;
        this.a_waves = 2;
        this.a_waveWeight = 30;
        this.resourceMultiplyer = 1f;
        this.levelPlace = new int[3];
        this.slotIndex = new int[0];
        this.checkedPlayer = false;
        this.hasCheckedNews = false;

        //LandConquerer.cs
        this.landPlaces = new int[0];
        this.landFaction = new Factions[0];

        //Inventory.cs
        this.towerIndex = new int[0];

        //PerlinNoise.cs
        this.seed = 0; //Forces a random seed
        this.width = 32;
        this.height = 32;

        //Relationships.cs
        this.circleRelationPoints = new int[3] {85,10,60}; //Rectangle, Triangle, Square
        this.rectangleRelationPoints = new int[3] {85,10,85}; //Circle, Triangle, Square
        this.triangleRelationPoints = new int[3] {10,60,10}; //Circle, Rectangle, Square
        this.squareRelationPoints = new int[3] {60,85,10}; //Circle, Rectangle, Triangle

        //FactionConquer.cs
        this.glowInt = new int[0];
        this.glowPlayerInt = new int[0];

        //FactionPower.cs
        this.strength = new int[4];

        //Trust.cs
        this.circleTrust = 40;
        this.rectangleTrust = 40;
        this.triangleTrust = 40;
        this.squareTrust = 40;

        //Trading.cs
        this.reqIndex = -1;
        this.inviteChance = 0;

        //Alliances.cs
        this.isUnderAlliance = new bool[4];
        this.factionsA = new Factions[0];
        this.factionsB = new Factions[0];
        this.factionsC = new Factions[0];
        this.isUnderTemp1 = new bool[4]; 
        this.isUnderTemp2 = new bool[4]; 
        this.isUnderTemp3 = new bool[4]; 
        this.isUnderTemp4 = new bool[4]; 
        this.turnsLeft1 = new int[4]; 
        this.turnsLeft2 = new int[4]; 
        this.turnsLeft3 = new int[4]; 
        this.turnsLeft4 = new int[4]; 
        this.turnsTillattempt = 5;

        //AIFights.cs
        this.placesInt = new int[0];
        this.lastFight = new int[0];
        this.strength_fights = new int[0];
        this.relationIndex = new int[0];
        
        //News.cs
        this.savingType = new NewsType[0];
        this.savingInfo = new string[0];
        this.savingHeader = new string[0];
        this.savingIconIndex = new int[0];
        this.savingDays = new int[0];
        this.days = 0;

        //Shop.cs
        this.shopSaved = new int[0];
    }
}
