using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Relation
{
    Hate,
    Neutral,
    Like
}

[System.Serializable]
public class RelationShipRequirement
{
    public Relation relation;
    public int requirement;
}

[System.Serializable]
public class FactionRelation
{
    public Factions faction;
    public Relation relation = Relation.Neutral;
    public int relationPoints = 60;
}

public class Relationships : MonoBehaviour, IDataPersistence
{
    public static Relationships Instance {get; private set;}

    public RelationShipRequirement[] req;
    
    [Header("Circle Faction")]
    public FactionRelation[] circleRelation;

    [Header("Rectangle Faction")]
    public FactionRelation[] rectangleRelation;

    [Header("Triangle Faction")]
    public FactionRelation[] triangleRelation;

    [Header("Square Faction")]
    public FactionRelation[] squareRelation;

    [Header("Saving")]
    public int[] circleRelationPoints = new int[3];
    public int[] rectangleRelationPoints = new int[3];
    public int[] triangleRelationPoints = new int[3];
    public int[] squareRelationPoints = new int[3];

    [Header("Debug")]
    public Factions faction;
    public Factions factionAffected;
    public int amount;
    public bool debug = false;

    bool hasRecieved = false;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.circleRelationPoints = data.circleRelationPoints; 
        this.rectangleRelationPoints = data.rectangleRelationPoints;
        this.triangleRelationPoints = data.triangleRelationPoints;
        this.squareRelationPoints = data.squareRelationPoints;
        EmbedBack();
        hasRecieved = true;
    }

    public void SaveData(GameData data)
    {
        if(hasRecieved)
        {
            EmbedToArray();
            data.circleRelationPoints = this.circleRelationPoints; 
            data.rectangleRelationPoints = this.rectangleRelationPoints;
            data.triangleRelationPoints = this.triangleRelationPoints;
            data.squareRelationPoints = this.squareRelationPoints;
        }
    }


    //Saving and Loading
    void EmbedToArray()
    {
        for(int i = 0; i < circleRelationPoints.Length; i++)
        {
            circleRelationPoints[i] = circleRelation[i].relationPoints;
            rectangleRelationPoints[i] = rectangleRelation[i].relationPoints;
            squareRelationPoints[i] = squareRelation[i].relationPoints;
            triangleRelationPoints[i] = triangleRelation[i].relationPoints;
        }
    }
    void EmbedBack()
    {
        for(int i = 0; i < circleRelation.Length; i++)
        {
            circleRelation[i].relationPoints = circleRelationPoints[i];
            rectangleRelation[i].relationPoints = rectangleRelationPoints[i];
            triangleRelation[i].relationPoints = triangleRelationPoints[i];
            squareRelation[i].relationPoints = squareRelationPoints[i];
        }
        CheckRelation(circleRelation);
        CheckRelation(rectangleRelation);
        CheckRelation(triangleRelation);
        CheckRelation(squareRelation);
    }

    void Update()
    {
        if(!debug)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            ChangeRelation(faction, factionAffected, amount);
        }
    }

    public void ChangeRelation(Factions faction, Factions affectedFaction, int amount)
    {
        FactionRelation[] factionRelation = null;

        switch(faction) //Both will be linked, no need for copy since we target it anyway
        {
            case Factions.Circle:
                factionRelation = circleRelation;
                break;

            case Factions.Rectangle:
                factionRelation = rectangleRelation;
                break;

            case Factions.Triangle:
                factionRelation = triangleRelation;
                break;

            case Factions.Square:
                factionRelation = squareRelation;
                break;
        }

        for(int i = 0; i < factionRelation.Length; i++)
        {
            if(affectedFaction == factionRelation[i].faction)
            {
                factionRelation[i].relationPoints = Mathf.Clamp(factionRelation[i].relationPoints + amount, 0, 100);
                CheckRelation(factionRelation);
                break;
            }
        }
    }

    public void CheckRelation(FactionRelation[] factionRelation)
    {
        foreach(FactionRelation relation in factionRelation)
        {
            for(int i = 0; i < req.Length - 1; i++)
            {
                if(relation.relationPoints >= req[i].requirement && relation.relationPoints < req[i + 1].requirement)
                {
                    relation.relation = req[i].relation;
                    break;
                }
            }
        }
    }
}
