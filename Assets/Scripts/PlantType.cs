using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GrowthStage
{
    Seed,
    Seedling,
    Sapling,
    Adult,
    Dead
}

public enum PlantType
{
    Grass,
    Dandelion,
    Bush,
    Tree
}

[CreateAssetMenu(menuName = "Plant Type")]
public class PlantSpecs : ScriptableObject
{
    public PlantType plantType;
    public string commonName;
    public string latinName;
    public string description;
    public string distribution;
    public Image seedIcon;
    public int maxHealth;
    public GameObject plantPrefab;
    
    public GrowthStageParameters seedStage;
    public GrowthStageParameters seedlingStage;
    public GrowthStageParameters saplingStage;
    public GrowthStageParameters adultStage;
}

[System.Serializable]
public class GrowthStageParameters
{
    public Range growthTimeRangeDesert; //in game days
    public Range growthTimeRangeAlive; 
    public Range growthTimeRangeVivid;
    public Range growthTimeRangeLush;
    public float waterRequirement;
    public float evaporationEffect;
    public float waterUsage;
}

[System.Serializable]
public class Range
{
    public float min;
    public float max;
}
