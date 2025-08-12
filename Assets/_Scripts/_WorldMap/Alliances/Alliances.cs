using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class FactionAlliance
{
    public Factions faction;
    public bool isUnderAlliance;
    [Tooltip("Neutral or Like, Neutral gives 1 tower and Like gives 2")]
    public Relation relation;
    public int trust;
    
    [Header("Benefits")]
    public TowerSlotSO[] neutralTowers;
    public int[] neutralIndexes;
    public TowerSlotSO[] likeTowers;
    public int[] likeIndexes;
}

[System.Serializable]
public class TemporaryAI 
{
    public Factions faction;
    public bool isUnderAlliance;
    public int turnsLeft;
}

public class Alliances : MonoBehaviour, IDataPersistence
{
    public static Alliances Instance {get; private set;}

    [Header("Player Manual Alliances")]
    public FactionAlliance[] factionAlliances;
    public bool[] isUnderAlliance;
    private Factions playerFaction;
    private bool hasRecieved = false;

    [Header("AI Temporary ALliances")]
    public TemporaryAI[] circleTempAI;
    public TemporaryAI[] rectangleTempAI;
    public TemporaryAI[] triangleTempAI;
    public TemporaryAI[] squareTempAI;
    public int turnsForAlliance = 6;

    private bool[] isUnderTemp1 = new bool[4];
    private int[] turnsLeft1 = new int[4];
    private bool[] isUnderTemp2 = new bool[4]; 
    private int[] turnsLeft2 = new int[4];
    private bool[] isUnderTemp3 = new bool[4]; 
    private int[] turnsLeft3 = new int[4];
    private bool[] isUnderTemp4 = new bool[4]; 
    private int[] turnsLeft4 = new int[4];
    public int turnsTillattempt = 5;
    
    private List<Factions> factionsA = new List<Factions>();
    private List<Factions> factionsB = new List<Factions>();
    private List<Factions> factionsC = new List<Factions>();

    [Header("Debug")]
    public bool debug = false;
    public Factions otherFaction;

    void Awake()
    {
        Instance = this;
    }

    public void LoadData(GameData data)
    {
        this.playerFaction = data.playerFaction;
        this.isUnderAlliance = data.isUnderAlliance;
        this.factionsA = data.factionsA.ToList();
        this.factionsB = data.factionsB.ToList();
        this.factionsC = data.factionsC.ToList();
        this.isUnderTemp1 = data.isUnderTemp1;
        this.isUnderTemp2 = data.isUnderTemp2;
        this.isUnderTemp3 = data.isUnderTemp3;
        this.isUnderTemp4 = data.isUnderTemp4;
        this.turnsLeft1 = data.turnsLeft1;
        this.turnsLeft2 = data.turnsLeft2;
        this.turnsLeft3 = data.turnsLeft3;
        this.turnsLeft4 = data.turnsLeft4;
        this.turnsTillattempt = data.turnsTillattempt;
        ConvertBack();
        hasRecieved = true;
    }

    public void SaveData(GameData data)
    {
        if(hasRecieved)
        {
            ConvertToBool();
            data.isUnderAlliance = this.isUnderAlliance;
            data.factionsA = this.factionsA.ToArray();
            data.factionsB = this.factionsB.ToArray();
            data.factionsC = this.factionsC.ToArray();
            data.isUnderTemp1 = this.isUnderTemp1;
            data.isUnderTemp2 = this.isUnderTemp2;
            data.isUnderTemp3 = this.isUnderTemp3;
            data.isUnderTemp4 = this.isUnderTemp4;
            data.turnsLeft1 = this.turnsLeft1;
            data.turnsLeft2 = this.turnsLeft2;
            data.turnsLeft3 = this.turnsLeft3;
            data.turnsLeft4 = this.turnsLeft4;
            data.turnsTillattempt = this.turnsTillattempt;
        }
    }

    void ConvertToBool()
    {
        for(int i = 0; i < circleTempAI.Length; i++)
        {
            isUnderTemp1[i] = circleTempAI[i].isUnderAlliance;
            turnsLeft1[i] = circleTempAI[i].turnsLeft;
            isUnderTemp2[i] = rectangleTempAI[i].isUnderAlliance;
            turnsLeft2[i] = rectangleTempAI[i].turnsLeft;
            isUnderTemp3[i] = triangleTempAI[i].isUnderAlliance;
            turnsLeft3[i] = triangleTempAI[i].turnsLeft;
            isUnderTemp4[i] = squareTempAI[i].isUnderAlliance;
            turnsLeft4[i] = squareTempAI[i].turnsLeft;
        }
    }

    void ConvertBack()
    {
        for(int i = 0; i < circleTempAI.Length; i++)
        {
            circleTempAI[i].isUnderAlliance = isUnderTemp1[i];
            circleTempAI[i].turnsLeft = turnsLeft1[i];
            rectangleTempAI[i].isUnderAlliance = isUnderTemp2[i];
            rectangleTempAI[i].turnsLeft = turnsLeft2[i];
            triangleTempAI[i].isUnderAlliance = isUnderTemp3[i];
            triangleTempAI[i].turnsLeft = turnsLeft3[i];
            squareTempAI[i].isUnderAlliance = isUnderTemp4[i];
            squareTempAI[i].turnsLeft = turnsLeft4[i];
        }
    }

    public void ReduceRounds()
    {
        for(int i = 0; i < circleTempAI.Length; i++)
        {
            if(circleTempAI[i].isUnderAlliance)
            {
                circleTempAI[i].turnsLeft--;
            }
            if(rectangleTempAI[i].isUnderAlliance)
            {
                rectangleTempAI[i].turnsLeft--;
            }
            if(triangleTempAI[i].isUnderAlliance)
            {
                triangleTempAI[i].turnsLeft--;
            }
            if(squareTempAI[i].isUnderAlliance)
            {
                squareTempAI[i].turnsLeft--;
            }

            Factions faction1 = Factions.Neutral;
            Factions faction2 = Factions.Neutral;

            if(circleTempAI[i].turnsLeft == 0)
            {
                circleTempAI[i].isUnderAlliance = false;
                faction1 = Factions.Circle;
                faction2 = circleTempAI[i].faction;

                for(int o = 0; o < factionsA.Count; o++)
                {
                    if(faction1 == factionsA[o] && faction2 == factionsB[o])
                    {
                        factionsA.RemoveAt(o);
                        factionsB.RemoveAt(o);
                        factionsC.RemoveAt(o);
                        break;
                    }
                }
            }
            
            if(rectangleTempAI[i].turnsLeft == 0)
            {
                rectangleTempAI[i].isUnderAlliance = false;
                faction1 = Factions.Rectangle;
                faction2 = rectangleTempAI[i].faction;

                for(int o = 0; o < factionsA.Count; o++)
                {
                    if(faction1 == factionsA[o] && faction2 == factionsB[o])
                    {
                        factionsA.RemoveAt(o);
                        factionsB.RemoveAt(o);
                        factionsC.RemoveAt(o);
                        break;
                    }
                }
            }

            if(triangleTempAI[i].turnsLeft == 0)
            {
                triangleTempAI[i].isUnderAlliance = false;
                faction1 = Factions.Triangle;
                faction2 = triangleTempAI[i].faction;

                for(int o = 0; o < factionsA.Count; o++)
                {
                    if(faction1 == factionsA[o] && faction2 == factionsB[o])
                    {
                        factionsA.RemoveAt(o);
                        factionsB.RemoveAt(o);
                        factionsC.RemoveAt(o);
                        break;
                    }
                }
            }

            if(squareTempAI[i].turnsLeft == 0)
            {
                squareTempAI[i].isUnderAlliance = false;
                faction1 = Factions.Square;
                faction2 = squareTempAI[i].faction;

                for(int o = 0; o < factionsA.Count; o++)
                {
                    if(faction1 == factionsA[o] && faction2 == factionsB[o])
                    {
                        factionsA.RemoveAt(o);
                        factionsB.RemoveAt(o);
                        factionsC.RemoveAt(o);
                        break;
                    }
                }
            }
        }

        turnsTillattempt--;
        if(turnsTillattempt <= 0)
        {
            turnsTillattempt = 5;
            CheckForTemporaryAI();
            FormTemporaryAlliances();
        }
    }

    public void RevertToAlliances()
    {
        RefreshRelations();
        for(int i = 0; i < factionAlliances.Length; i++)
        {
            if(factionAlliances[i].relation == Relation.Hate)
            {
                factionAlliances[i].isUnderAlliance = false;
                isUnderAlliance[i] = false;
                continue;
            }

            factionAlliances[i].isUnderAlliance = isUnderAlliance[i];
        }
        CheckAlliances();
    }

    void Update()
    {
        if(!debug)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            MakeManualAlliance(this.otherFaction);
        }
        if(Input.GetKeyDown(KeyCode.P))
        {
            RemoveManualAlliance(this.otherFaction);
        }
        if(Input.GetKeyDown(KeyCode.D))
        {
            CheckForTemporaryAI();
            FormTemporaryAlliances();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            CheckTemporaryAlliancesAI();
        }
    }

    void RefreshRelations()
    {
        Relationships relationships = Relationships.Instance;

        foreach(var alliance in factionAlliances)
        {
            FactionRelation[] relation = new FactionRelation[0];
            Factions otherFaction = Factions.Neutral;
            switch(alliance.faction)
            {
                case Factions.Circle:
                    relation = relationships.circleRelation;
                    otherFaction = Factions.Circle;
                    break;

                case Factions.Rectangle:
                    relation = relationships.rectangleRelation;
                    otherFaction = Factions.Rectangle;
                    break;

                case Factions.Triangle:
                    relation = relationships.triangleRelation;
                    otherFaction = Factions.Triangle;
                    break;

                case Factions.Square:
                    relation = relationships.squareRelation;
                    otherFaction = Factions.Square;
                    break;
            }

            for(int i = 0; i < relation.Length; i++)
            {
                if(playerFaction == relation[i].faction)
                {
                    alliance.relation = relation[i].relation;
                    if(relation[i].relationPoints < 50)
                    {
                        RemoveManualAlliance(otherFaction);
                    }
                    break;
                }
            }
        }

        Trust trust = Trust.Instance;

        factionAlliances[0].trust = trust.circleTrust;
        factionAlliances[1].trust = trust.rectangleTrust;
        factionAlliances[2].trust = trust.triangleTrust;
        factionAlliances[3].trust = trust.squareTrust;
    }

    void CheckAlliances()
    {
        Inventory inventory = Inventory.Instance;
        inventory.allianceTowers.Clear();

        List<TowerSlotSO> allianceSlots = new List<TowerSlotSO>();
        List<int> allianceIndex = new List<int>();
        foreach(var alliance in factionAlliances)
        {
            if(alliance.isUnderAlliance)
            {
                if((alliance.relation == Relation.Neutral && alliance.trust >= AllianceSystem.Instance.neutralTrust) || (alliance.relation == Relation.Like && alliance.trust >= AllianceSystem.Instance.neutralTrust && alliance.trust < AllianceSystem.Instance.likeTrust))
                {
                    allianceSlots.AddRange(alliance.neutralTowers);
                    allianceIndex.AddRange(alliance.neutralIndexes);
                }
                else if(alliance.relation == Relation.Like && alliance.trust >= AllianceSystem.Instance.likeTrust)
                {
                    allianceSlots.AddRange(alliance.likeTowers);
                    allianceIndex.AddRange(alliance.likeIndexes);
                }
            }
        }

        inventory.allianceTowers = allianceSlots;
        inventory.allianceIndex = allianceIndex;
    }

    //AI-AI Temporary
    void CheckForTemporaryAI()
    {
        FactionPower power = FactionPower.Instance;
        FactionStrength[] strength = new FactionStrength[0];
        List<Factions> factionsA = new List<Factions>(this.factionsA);
        List<Factions> factionsB = new List<Factions>(this.factionsB);
        List<Factions> factionsC = new List<Factions>(this.factionsC);

        HashSet<(Factions, Factions)> seenPairs = new HashSet<(Factions, Factions)>();
        (Factions, Factions) Normalize(Factions a, Factions b)
        {
            return a.CompareTo(b) < 0 ? (a, b) : (b, a);
        }

        strength = (FactionStrength[])power.factionStrength.Clone();

        foreach(var faction in strength)
        {
            for(int i = 0; i < strength.Length; i++)
            {
                if(strength[i].faction == faction.faction)
                {
                    continue;
                }
                int strength1 = faction.strength;
                int strength2 = strength[i].strength;
                int strength3 = 0;
                int strength3Index = -1;
                for(int o = 0; o < strength.Length; o++)
                {
                    if(strength[o].faction == faction.faction || strength[o].faction == strength[i].faction)
                    {
                        continue;
                    }

                    if(strength[o].strength > strength3 && strength[o].faction != faction.faction && strength[o].faction != strength[i].faction)
                    {
                        strength3 = strength[o].strength;
                        strength3Index = o;
                    }
                }

                int combinedStrengths = strength1 + strength2;

                if(combinedStrengths < strength3)
                {
                    Relationships relationships = Relationships.Instance;
                    Relation relation1 = Relation.Like; //FactionA
                    Relation relation2 = Relation.Like; //FactionB
                    FactionRelation[] factionRelation1 = new FactionRelation[0]; //FactionA
                    FactionRelation[] factionRelation2 = new FactionRelation[0]; //FactionB
                    Factions faction1 = faction.faction; //FactionA
                    Factions faction2 = strength[i].faction; //FactionB
                    Factions faction3 = strength[strength3Index].faction; //FactionC

                    //Finding their FactionRelations
                    switch(faction1)
                    {
                        case Factions.Circle:
                            factionRelation1 = relationships.circleRelation;
                            break;

                        case Factions.Rectangle:
                            factionRelation1 = relationships.rectangleRelation;
                            break;

                        case Factions.Triangle:
                            factionRelation1 = relationships.triangleRelation;
                            break;

                        case Factions.Square:
                            factionRelation1 = relationships.squareRelation;
                            break;
                    }
                    switch(faction2)
                    {
                        case Factions.Circle:
                            factionRelation2 = relationships.circleRelation;
                            break;

                        case Factions.Rectangle:
                            factionRelation2 = relationships.rectangleRelation;
                            break;

                        case Factions.Triangle:
                            factionRelation2 = relationships.triangleRelation;
                            break;

                        case Factions.Square:
                            factionRelation2 = relationships.squareRelation;
                            break;
                    }

                    //Taking the relation part of the equation
                    foreach(var relation in factionRelation1)
                    {
                        if(faction3 == relation.faction)
                        {
                            relation1 = relation.relation;
                            break;
                        }
                    }
                    foreach(var relation_2 in factionRelation2)
                    {
                        if(faction3 == relation_2.faction)
                        {
                            relation2 = relation_2.relation;
                            break;
                        }
                    }

                    if(relation1 == Relation.Hate && relation2 == Relation.Hate)
                    {
                        var pair = Normalize(faction1, faction2);
                        if(seenPairs.Add(pair) && faction1 != playerFaction && faction2 != playerFaction)
                        {
                            factionsA.Add(faction1);
                            factionsB.Add(faction2);
                            factionsC.Add(faction3);
                        }
                    }
                }
            }
        }

        for(int i = 0; i < factionsA.Count; i++)
        {
            Debug.Log($"{factionsA[i]} and {factionsB[i]}'s strengths are weaker than {factionsC[i]}");
        }

        this.factionsA = factionsA;
        this.factionsB = factionsB;
        this.factionsC = factionsC;
    }

    void FormTemporaryAlliances()
    {
        Relationships relationships = Relationships.Instance;
        for(int i = 0; i < factionsA.Count; i++)
        {
            Factions factionA = factionsA[i];
            Factions factionB = factionsB[i];
            TemporaryAI[] temp = new TemporaryAI[0];
            TemporaryAI[] temp2 = new TemporaryAI[0];
            FactionRelation[] factionRelation1 = new FactionRelation[0];
            FactionRelation[] factionRelation2 = new FactionRelation[0];
            int index = -1;

            switch(factionA)
            {
                case Factions.Circle:
                    index = 0;
                    temp = circleTempAI;
                    factionRelation1 = relationships.circleRelation;
                    break;

                case Factions.Rectangle:
                    index = 1;
                    temp = rectangleTempAI;
                    factionRelation1 = relationships.rectangleRelation;
                    break;

                case Factions.Triangle:
                    index = 2;
                    temp = triangleTempAI;
                    factionRelation1 = relationships.triangleRelation;
                    break;

                case Factions.Square:
                    index = 3;
                    temp = squareTempAI;
                    factionRelation1 = relationships.squareRelation;
                    break;
            }
            switch(factionB)
            {
                case Factions.Circle:
                    temp2 = circleTempAI;
                    factionRelation2 = relationships.circleRelation;
                    break;

                case Factions.Rectangle:
                    temp2 = rectangleTempAI;
                    factionRelation2 = relationships.rectangleRelation;
                    break;

                case Factions.Triangle:
                    temp2 = triangleTempAI;
                    factionRelation2 = relationships.triangleRelation;
                    break;

                case Factions.Square:
                    temp2 = squareTempAI;
                    factionRelation2 = relationships.squareRelation;
                    break;
            }

            for(int o = 0; o < temp.Length; o++)
            {
                if(factionB == temp[o].faction)
                {
                    temp[o].isUnderAlliance = true;
                    temp[o].turnsLeft = turnsForAlliance;
                    break;
                }
            }

            for(int o = 0; o < temp2.Length; o++)
            {
                if(factionA == temp2[o].faction)
                {
                    temp2[o].isUnderAlliance = true;
                    temp2[o].turnsLeft = turnsForAlliance;
                    break;
                }
            }

            //Put something here in the News;
            News news = News.Instance;
            news.PutNewInfo(NewsType.TemporaryAllianceFormed, news.ReplaceStrings(factionA, factionB, news.temporaryAlliancePresets[Random.Range(0, news.temporaryAlliancePresets.Length)]), index);

            foreach(var relation in factionRelation1)
            {
                if(factionB == relation.faction)
                {
                    if(relation.relationPoints < 50)
                    {
                        relation.relationPoints = 50;
                        relationships.CheckRelation(factionRelation1);
                    }
                    break;
                }
            }

            foreach(var relation2 in factionRelation2)
            {
                if(factionA == relation2.faction)
                {
                    if(relation2.relationPoints < 50)
                    {
                        relation2.relationPoints = 50;
                        relationships.CheckRelation(factionRelation2);
                    }
                    break;
                }
            }
        }
    }
    
    public void CheckTemporaryAlliancesAI()
    {
        FactionPower power = FactionPower.Instance;
        FactionStrength[] strength = (FactionStrength[])power.factionStrength.Clone();
        for(int i = factionsA.Count - 1; i >= 0; i--)
        {
            int strengthA = 0;
            int strengthB = 0;
            int strengthC = 0;

            foreach(var faction in strength)
            {
                if(factionsA[i] == faction.faction)
                {
                    strengthA = faction.strength;
                }
                else if(factionsB[i] == faction.faction)
                {
                    strengthB = faction.strength;
                }
                else if(factionsC[i] == faction.faction)
                {
                    strengthC = faction.strength;
                }
            }

            int combinedStrengths = strengthA + strengthB;
            if(combinedStrengths >= strengthC)
            {
                TemporaryAI[] temp = new TemporaryAI[0];
                TemporaryAI[] temp2 = new TemporaryAI[0];
                int index = -1;

                switch(factionsA[i])
                {
                    case Factions.Circle:
                        index = 0;
                        temp = circleTempAI;
                        break;

                    case Factions.Rectangle:
                        index = 1;
                        temp = rectangleTempAI;
                        break;

                    case Factions.Triangle:
                        index = 2;
                        temp = triangleTempAI;
                        break;

                    case Factions.Square:
                        index = 3;
                        temp = squareTempAI;
                        break;
                }
                switch(factionsB[i])
                {
                    case Factions.Circle:
                        temp2 = circleTempAI;
                        break;

                    case Factions.Rectangle:
                        temp2 = rectangleTempAI;
                        break;

                    case Factions.Triangle:
                        temp2 = triangleTempAI;
                        break;

                    case Factions.Square:
                        temp2 = squareTempAI;
                        break;
                }

                for(int o = 0; o < temp.Length; o++)
                {
                    if(factionsB[i] == temp[o].faction)
                    {
                        temp[o].isUnderAlliance = false;
                        break;
                    }
                }

                for(int o = 0; o < temp2.Length; o++)
                {
                    if(factionsA[i] == temp2[o].faction)
                    {
                        temp2[o].isUnderAlliance = false;
                        break;
                    }
                }

                //Put something here in the News;
                News news = News.Instance;
                news.PutNewInfo(NewsType.TemporaryAllianceBroken, news.ReplaceStrings(factionsA[i], factionsB[i], news.brokenTemporaryAlliancePresets[Random.Range(0, news.brokenTemporaryAlliancePresets.Length)]), index);

                factionsA.RemoveAt(i);
                factionsB.RemoveAt(i);
                factionsC.RemoveAt(i);
            }
        }
    }

    //Player-AI Manual Alliance
    public void MakeManualAlliance(Factions otherFaction)
    {
        News news = News.Instance;
        RefreshRelations();
        for(int i = 0; i < factionAlliances.Length; i++)
        {
            if(otherFaction == factionAlliances[i].faction && factionAlliances[i].relation != Relation.Hate)
            {
                factionAlliances[i].isUnderAlliance = true;
                int index = -1;
                switch(playerFaction)
                {
                    case Factions.Circle:
                        index = 0;
                        break;

                    case Factions.Rectangle:
                        index = 1;
                        break;

                    case Factions.Triangle:
                        index = 2;
                        break;

                    case Factions.Square:
                        index = 3;
                        break;
                }
                news.PutNewInfo(NewsType.AllianceFormed, news.ReplaceStrings(playerFaction, factionAlliances[i].faction, news.alliancePresets[Random.Range(0, news.alliancePresets.Length)]), index);
                break;
            }
        }

        List<bool> underAlliance = new List<bool>();
        
        foreach(var alliance in factionAlliances)
        {
            underAlliance.Add(alliance.isUnderAlliance);
        }

        isUnderAlliance = underAlliance.ToArray();

        CheckAlliances();
        //Put something here in the News;
    }

    public void RemoveManualAlliance(Factions otherFaction)
    {
        News news = News.Instance;
        // RefreshRelations();
        for(int i = 0; i < factionAlliances.Length; i++)
        {
            if(otherFaction == factionAlliances[i].faction && factionAlliances[i].isUnderAlliance)
            {
                factionAlliances[i].isUnderAlliance = false;
                int index = -1;
                switch(playerFaction)
                {
                    case Factions.Circle:
                        index = 0;
                        break;

                    case Factions.Rectangle:
                        index = 1;
                        break;

                    case Factions.Triangle:
                        index = 2;
                        break;

                    case Factions.Square:
                        index = 3;
                        break;
                }
                news.PutNewInfo(NewsType.AllianceBroken, news.ReplaceStrings(playerFaction, factionAlliances[i].faction, news.brokenAlliancePresets[Random.Range(0, news.brokenAlliancePresets.Length)]), index);
                break;
            }
        }

        List<bool> underAlliance = new List<bool>();
        
        foreach(var alliance in factionAlliances)
        {
            underAlliance.Add(alliance.isUnderAlliance);
        }

        isUnderAlliance = underAlliance.ToArray();

        CheckAlliances();
        //Put something here in the News;
    }
}